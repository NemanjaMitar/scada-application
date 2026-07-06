using DataConcentrator.Utils;

namespace DataConcentrator.Model
{
    public class AnalogOutput : Tag, IOutputTags
    {
        // Promeni iz InitialValue u InitValue da odgovara interfejsu
        public double InitValue { get; set; }

        public override ETagType Type => ETagType.AO;
    }
}