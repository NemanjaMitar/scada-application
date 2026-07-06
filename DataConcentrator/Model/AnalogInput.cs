using DataConcentrator.Utils;
using System;

namespace DataConcentrator.Model
{
    public class AnalogInput : Tag, IInputTags
    {
        // Polja sa privatnim varijablama radi enkapsulacije
        private TimeSpan scanTime;
        private bool isOnScan;

        // Vraćen set pristupnik kako bi AddTagWindow mogao da dodeli vrednost
        public TimeSpan ScanTime
        {
            get => scanTime;
            set => scanTime = value;
        }

        // Vraćen set pristupnik
        public bool IsOnScan
        {
            get => isOnScan;
            set => isOnScan = value;
        }

        // Dodati set; pristupnici kako bi AddTagWindow mogao inicijalizovati objekte
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }
        public double Deadband { get; set; }
        public double Hysteresis { get; set; }

        public override ETagType Type => ETagType.AI;
        public double Value { get; set; }
    }
}