using DataConcentrator.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.Model
{
    public class DigitalOutput : Tag, IOutputTags
    {
        private double value;

        public double InitValue { get; set; }

        public override ETagType Type => ETagType.DO;

        // Trenutna upisana vrednost (0 ili 1) - runtime
        [NotMapped]
        public double Value
        {
            get => value;
            set { this.value = value; OnPropertyChanged("Value"); }
        }

        // Upisivanje vrednosti u digitalnu izlaznu veličinu
        public void WriteValue(double newValue)
        {
            if (string.IsNullOrWhiteSpace(Address))
                return;

            PLC.Instance.SetDigitalValue(Address, newValue);
            Value = newValue;
        }
    }
}
