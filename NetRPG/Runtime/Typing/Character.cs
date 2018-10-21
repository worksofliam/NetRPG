using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Typing
{
    class Character : DataValue
    {
        private int Length;
        public Character(string name, int length, string initialValue = "")
        {
            if (initialValue == null)
                initialValue = "";

            this.Name = name;
            this.Type = Types.Character;
            this.InitValue = initialValue.PadRight(length);

            this.Length = length;

            this.Dimentions = 1;
            this.Value = new object[this.Dimentions];

            this.DoInitialValue();
        }

        public override void Set(object value, int index = 0)
        {
            string NewValue = (string)value;
            
            if (NewValue.Length > this.Length)
                NewValue = NewValue.Substring(0, this.Length);
            else
                NewValue = NewValue.PadRight(this.Length);

            this.Value[index] = NewValue;
        }
    }
}
