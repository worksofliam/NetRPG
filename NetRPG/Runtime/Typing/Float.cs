using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Typing
{
    class Float : DataValue
    {
        private int Precision;
        public Float(string name, Types type, double initialValue = 0)
        {
            this.Name = name;
            this.Type = type;
            this.InitValue = initialValue;

            this.Dimentions = 1;
            this.Value = new object[this.Dimentions];

            this.DoInitialValue();
        }

        public override void Set(object value, int index = 0)
        {
            switch (this.Type) {
                case Types.Float:
                    this.Value[index] = Convert.ToSingle(value);
                    break;
                case Types.Double:
                    this.Value[index] = Convert.ToDouble(value);
                    break;
            }
        }
    }
}
