using DataConcentrator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator.Model
{
    public class DigitalInput : Tag, IInputTags
    {
        private TimeSpan scanTime;
        private bool isOnScan;

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
    }
}
