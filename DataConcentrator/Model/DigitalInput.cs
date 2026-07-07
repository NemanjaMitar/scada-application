using DataConcentrator.Utils;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace DataConcentrator.Model
{
    public class DigitalInput : Tag, IInputTags
    {
        private TimeSpan scanTime;
        private bool isOnScan;
        private double value;

        private volatile bool keepScanning;

        public override ETagType Type => ETagType.DI;

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

        // Trenutno očitana vrednost (0 ili 1) - runtime, ne perzistira se
        [NotMapped]
        public double Value
        {
            get => value;
            set { this.value = value; OnPropertyChanged("Value"); }
        }

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
                    // Digitalne vrednosti (0/1) se u simulatoru čitaju istom metodom
                    Value = PLC.Instance.GetAnalogValue(this.Address);
                }
                catch (Exception)
                {
                    break;
                }

                Thread.Sleep((int)Math.Max(1, ScanTime.TotalMilliseconds));
            }
        }
    }
}
