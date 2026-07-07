using DataConcentrator.Utils;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DataConcentrator.Model
{
    public class AnalogInput : Tag, IInputTags, INotifyPropertyChanged
    {
        // Polja sa privatnim varijablama radi enkapsulacije
        private TimeSpan scanTime;
        private bool isOnScan;
        private double value;
        private string alarmColor = "Transparent";

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

        [NotMapped]
        public double Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        [NotMapped]
        public string AlarmColor
        {
            get => alarmColor;
            set
            {
                alarmColor = value;
                OnPropertyChanged("AlarmColor");
            }
        }

        [NotMapped]
        public virtual ICollection<Alarm> Alarms { get; set; }

        [NotMapped]
        public bool HasActiveUnacknowledgedAlarm
        {
            get
            {
                if (Alarms == null) return false;
                foreach (var alarm in Alarms)
                {
                    if (alarm.State == EAlarmState.Active)
                        return true;
                }
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}