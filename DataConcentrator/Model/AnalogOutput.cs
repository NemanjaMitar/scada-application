using DataConcentrator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator.Model
{
    public class AnalogOutput : Tag, IOutputTags
    {
        private double initValue;

        public double InitValue
        {
            get => initValue;
            set => initValue = value;
        }

        public override ETagType Type => ETagType.AO;
    }
}
