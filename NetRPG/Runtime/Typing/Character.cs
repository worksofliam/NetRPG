using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Typing
{
    class Character : DataValue
    {
        public Character(string name, int length, string initialValue = "")
        {
            if (initialValue == null)
                initialValue = "";

            this.Name = name;
            this.Type = Types.Character;
            this.Properties = new Dictionary<string, dynamic>();
            this.Properties.Add("length", length);
            this.Properties.Add("initialValue", initialValue);

            this.Dimentions = 1;
            this.Value = new object[this.Dimentions];

            this.InitialValue();
        }

        public override void Set(object value, int index = 0)
        {
            string NewValue = (string)value;
            int Length = this.Properties["length"];

            if (NewValue.Length > Length)
                NewValue = NewValue.Substring(0, Length);

            this.Value[index] = NewValue;
        }
    }
}
