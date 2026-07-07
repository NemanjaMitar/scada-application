using DataConcentrator.Utils;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.Model
{
    public class DigitalInput : Tag, IInputTags, INotifyPropertyChanged
    {
        private TimeSpan scanTime;
        private bool isOnScan;
        private int value;

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

        [NotMapped]
        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
