using DataConcentrator.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataConcentrator.Model
{
    public class AnalogOutput : Tag, IOutputTags
    {
        private double value;

        public double InitValue { get; set; }

        public override ETagType Type => ETagType.AO;

        // Trenutna upisana vrednost (runtime)
        [NotMapped]
        public double Value
        {
            get => value;
            set { this.value = value; OnPropertyChanged("Value"); }
        }

        // Upisivanje vrednosti u analognu izlaznu veličinu (PLC interno zaključava pristup)
        public void WriteValue(double newValue)
        {
            if (string.IsNullOrWhiteSpace(Address))
                return;

            PLC.Instance.SetAnalogValue(Address, newValue);
            Value = newValue;
        }
    }
}
