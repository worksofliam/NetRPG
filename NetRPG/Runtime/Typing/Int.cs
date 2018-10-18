using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Language;

namespace NetRPG.Runtime.Typing
{
    class Int : DataValue
    {
        public Int(string name, Types type, int initialValue = 0)
        {
            this.Name = name;

            this.Type = type;
            
            this.Properties = new Dictionary<string, dynamic>();
            this.Properties.Add("initialValue", initialValue);

            this.Dimentions = 1;
            this.Value = new object[this.Dimentions];

            this.InitialValue();
        }

        public override void Set(object value, int index = 0)
        {
            dynamic NewValue = null;

            switch (this.Type)
            {
                case Types.Int8:
                    NewValue = Convert.ToSByte(value);
                    break;
                case Types.Int16:
                    NewValue = Convert.ToInt16(value);
                    break;
                case Types.Int32:
                    NewValue = Convert.ToInt32(value);
                    break;
                case Types.Int64:
                    NewValue = Convert.ToInt64(value);
                    break;
            }
            
            this.Value[index] = NewValue;
        }
    }
}
