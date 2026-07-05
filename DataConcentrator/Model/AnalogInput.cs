using DataConcentrator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator.Model
{
    public class AnalogInput : Tag, IInputTags
    {
        private TimeSpan scanTime;
        private bool isOnScan;

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

        public double LowLimit { get; }
        public double HighLimit { get; }
        public string Units { get; }
        public double Deadband { get; }
        public double Hysteresis { get; }
        public override ETagType Type => ETagType.AI;
        public double Value { get; set; }
    }
}
