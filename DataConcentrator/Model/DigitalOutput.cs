using DataConcentrator.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.Model
{
    public class DigitalOutput : Tag, IOutputTags, INotifyPropertyChanged
    {
        private int currentValue;

        // Promeni iz InitialValue u InitValue da odgovara interfejsu
        public double InitValue { get; set; }

        [NotMapped]
        public int CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                OnPropertyChanged("CurrentValue");
            }
        }

        public override ETagType Type => ETagType.DO;

        public bool SetValue(int value)
        {
            CurrentValue = value;
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}