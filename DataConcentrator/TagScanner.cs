using DataConcentrator.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DataConcentrator
{
    public class TagScanner
    {
        private readonly Dictionary<string, Thread> _threads = new Dictionary<string, Thread>();
        private readonly Dictionary<string, bool> _running = new Dictionary<string, bool>();
        private readonly object _lock = new object();
        private readonly Action<ActivatedAlarm> _alarmCallback;

        public event Action<ActivatedAlarm> AlarmActivated;

        public TagScanner(Action<ActivatedAlarm> alarmCallback = null)
        {
            _alarmCallback = alarmCallback;
        }

        public void StartScanning(Tag tag)
        {
            lock (_lock)
            {
                if (_threads.ContainsKey(tag.Name))
                    return;

                _running[tag.Name] = true;

                var thread = new Thread(() => ScanLoop(tag))
                {
                    IsBackground = true,
                    Name = $"Scanner_{tag.Name}"
                };
                _threads[tag.Name] = thread;
                thread.Start();
            }
        }

        public void StopScanning(string tagName)
        {
            lock (_lock)
            {
                if (_running.ContainsKey(tagName))
                    _running[tagName] = false;

                if (_threads.TryGetValue(tagName, out var thread))
                {
                    try { thread.Join(TimeSpan.FromSeconds(1)); }
                    catch { }
                    _threads.Remove(tagName);
                }
            }
        }

        public void StopAll()
        {
            lock (_lock)
            {
                foreach (var name in new List<string>(_threads.Keys))
                {
                    StopScanning(name);
                }
            }
        }

        private bool IsRunning(string tagName)
        {
            lock (_lock)
            {
                return _running.TryGetValue(tagName, out bool value) && value;
            }
        }

        private void ScanLoop(Tag tag)
        {
            if (tag is AnalogInput ai)
            {
                ai.IsOnScan = true;
                DataManager.SaveChanges();

                while (IsRunning(tag.Name))
                {
                    lock (PLC.LockObject)
                    {
                        ai.Value = PLC.Instance.GetAnalogValue(ai.Address);
                    }

                    CheckAnalogInputAlarms(ai);
                    Thread.Sleep(ai.ScanTime);
                }
            }
            else if (tag is DigitalInput di)
            {
                di.IsOnScan = true;
                DataManager.SaveChanges();

                while (IsRunning(tag.Name))
                {
                    lock (PLC.LockObject)
                    {
                        di.Value = (int)PLC.Instance.GetAnalogValue(di.Address);
                    }
                    Thread.Sleep(di.ScanTime);
                }
            }
        }

        private void CheckAnalogInputAlarms(AnalogInput ai)
        {
            var alarms = DataManager.GetAlarmsForTag(ai.Name);

            foreach (var alarm in alarms)
            {
                bool triggered = alarm.Direction == EAlarmDirection.Above
                    ? ai.Value > alarm.LimitValue
                    : ai.Value < alarm.LimitValue;

                if (triggered)
                {
                    if (alarm.State != EAlarmState.Active)
                    {
                        alarm.State = EAlarmState.Active;
                        DataManager.SaveChanges();

                        var activated = new ActivatedAlarm
                        {
                            AlarmId = alarm.Id,
                            TagName = ai.Name,
                            Message = alarm.Message,
                            Timestamp = DateTime.Now
                        };

                        DataManager.SaveActivatedAlarm(activated);

                        ai.AlarmColor = "Red";

                        _alarmCallback?.Invoke(activated);
                        AlarmActivated?.Invoke(activated);
                    }
                }
                else
                {
                    if (alarm.State == EAlarmState.Active || alarm.State == EAlarmState.Acknowledged)
                    {
                        alarm.State = EAlarmState.Inactive;
                        DataManager.SaveChanges();
                        ai.AlarmColor = "Transparent";
                    }
                }
            }
        }
    }
}
