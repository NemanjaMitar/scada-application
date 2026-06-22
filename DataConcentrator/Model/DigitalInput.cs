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
        public override ETagType Type => ETagType.DI;
        public TimeSpan ScanTime => ScanTime;
        public bool IsOnScan => IsOnScan;
    }
}
