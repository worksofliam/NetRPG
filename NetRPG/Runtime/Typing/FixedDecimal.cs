using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Typing
{
    class FixedDecimal : DataValue
    {
        private int Precision;
        public FixedDecimal(string name, Types type, int precision, double initialValue = 0)
        {
            this.Name = name;

            this.Type = type;
            this.Precision = precision;
            this.InitValue = initialValue;

            this.Dimentions = 1;
            this.Value = new object[this.Dimentions];

            this.DoInitialValue();
        }

        public override void Set(object value, int index = 0)
        {
            double valueIn = Convert.ToDouble(value);

            valueIn = Math.Round(valueIn, this.Precision, MidpointRounding.AwayFromZero);

            this.Value[index] = valueIn;
        }
    }
}
