using DataConcentrator.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;

namespace DataConcentrator.Model
{
    public class AnalogInput : Tag, IInputTags
    {
        // Polja sa privatnim varijablama radi enkapsulacije
        private TimeSpan scanTime;
        private bool isOnScan;
        private double value;

        // Runtime stanje skeniranja / alarma (ne perzistira se)
        private volatile bool keepScanning;
        private bool firstRead = true;
        private double lastProcessed;
        private volatile bool acknowledged;
        private readonly HashSet<int> firingAlarms = new HashSet<int>();

        public TimeSpan ScanTime
        {
            get => scanTime;
            set => scanTime = value;
        }

        public bool IsOnScan
        {
            get => isOnScan;
            set => isOnScan = value;
        }

        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }
        public double Deadband { get; set; }
        public double Hysteresis { get; set; }

        public override ETagType Type => ETagType.AI;

        // Trenutno očitana vrednost sa PLC-a (runtime, ne perzistira se)
        [NotMapped]
        public double Value
        {
            get => value;
            set { this.value = value; OnPropertyChanged("Value"); }
        }

        // In-memory istorija očitavanja (za Report funkcionalnost). Ograničena veličina.
        [NotMapped]
        public List<KeyValuePair<DateTime, double>> History { get; } = new List<KeyValuePair<DateTime, double>>();

        // Okida se pri prelasku vrednosti u alarmnu zonu (GUI: popup + zvuk).
        public event Action<Alarm, double> AlarmActivated;

        #region Scan engine

        public void StartScan()
        {
            if (!IsOnScan || string.IsNullOrWhiteSpace(Address))
                return;

            keepScanning = true;
            Thread scanThread = new Thread(ScanLoop) { IsBackground = true };
            PLC.tagThreads[this.Name] = scanThread;
            scanThread.Start();
        }

        public void StopScan()
        {
            keepScanning = false;
            if (PLC.tagThreads.ContainsKey(this.Name))
                PLC.tagThreads.Remove(this.Name);
        }

        private void ScanLoop()
        {
            while (keepScanning)
            {
                try
                {
                    // 1. Čitanje sa PLC-a po I/O adresi (PLC interno zaključava pristup)
                    double raw = PLC.Instance.GetAnalogValue(this.Address);

                    // 2. Deadband: reagujemo samo ako je promena dovoljno velika
                    if (firstRead || Deadband <= 0 || Math.Abs(raw - lastProcessed) >= Deadband)
                    {
                        firstRead = false;
                        lastProcessed = raw;
                        Value = raw;

                        lock (History)
                        {
                            History.Add(new KeyValuePair<DateTime, double>(DateTime.Now, raw));
                            if (History.Count > 10000)
                                History.RemoveAt(0);
                        }

                        CheckAlarms(raw);
                    }
                }
                catch (Exception)
                {
                    // Ako je tag u međuvremenu obrisan iz baze, tiho gasimo nit
                    break;
                }

                Thread.Sleep((int)Math.Max(1, ScanTime.TotalMilliseconds));
            }
        }

        #endregion

        #region Alarm detection

        private void CheckAlarms(double currentValue)
        {
            List<Alarm> alarms;
            lock (ContextClass.Instance)
            {
                alarms = ContextClass.Instance.Alarms.Where(a => a.TagName == this.Name).ToList();
            }

            foreach (Alarm alarm in alarms)
            {
                bool firing = firingAlarms.Contains(alarm.Id);

                // Ulazak/izlazak iz alarmne zone uz histerezu (sprečava treperenje na granici)
                bool inZone;
                if (alarm.Direction == EAlarmDirection.Above)
                    inZone = firing ? currentValue >= alarm.LimitValue - Hysteresis
                                    : currentValue >= alarm.LimitValue;
                else
                    inZone = firing ? currentValue <= alarm.LimitValue + Hysteresis
                                    : currentValue <= alarm.LimitValue;

                if (inZone && !firing)
                {
                    firingAlarms.Add(alarm.Id);

                    lock (ContextClass.Instance)
                    {
                        alarm.State = EAlarmState.Active;
                        ContextClass.Instance.ActivatedAlarms.Add(new ActivatedAlarm
                        {
                            AlarmId = alarm.Id,
                            TagName = this.Name,
                            Message = alarm.Message,
                            Timestamp = DateTime.Now
                        });
                        ContextClass.Instance.SaveChanges();
                    }

                    AlarmActivated?.Invoke(alarm, currentValue);
                }
                else if (!inZone && firing)
                {
                    firingAlarms.Remove(alarm.Id);
                    lock (ContextClass.Instance)
                    {
                        alarm.State = EAlarmState.Inactive;
                        ContextClass.Instance.SaveChanges();
                    }
                }
            }

            // Agregatni status taga za bojenje reda u tabeli
            if (firingAlarms.Count > 0)
                AlarmStatus = acknowledged ? "Acknowledged" : "Active";
            else
            {
                AlarmStatus = "Normal";
                acknowledged = false;
            }
        }

        // Poziva GUI kada korisnik prihvati (acknowledge) alarm na ovom tagu.
        public void AcknowledgeAlarms()
        {
            if (firingAlarms.Count == 0)
                return;

            acknowledged = true;
            AlarmStatus = "Acknowledged";

            lock (ContextClass.Instance)
            {
                var acts = ContextClass.Instance.Alarms
                    .Where(a => a.TagName == this.Name && a.State == EAlarmState.Active)
                    .ToList();
                foreach (var a in acts)
                    a.State = EAlarmState.Acknowledged;
                ContextClass.Instance.SaveChanges();
            }
        }

        // Da li ovaj tag ima bar jedan aktivan (neacknowledge-ovan) alarm - koristi GUI za zvuk.
        public bool HasActiveAlarm => firingAlarms.Count > 0 && !acknowledged;

        #endregion
    }
}
