using DataConcentrator.Utils;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.Model
{
    public class AnalogOutput : Tag, IOutputTags, INotifyPropertyChanged
    {
        private double currentValue;

        // Promeni iz InitialValue u InitValue da odgovara interfejsu
        public double InitValue { get; set; }

        [NotMapped]
        public double CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                OnPropertyChanged("CurrentValue");
            }
        }

        public override ETagType Type => ETagType.AO;

        public bool SetValue(double value)
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