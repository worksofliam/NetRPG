using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Typing
{
    class Ind : DataValue
    {
        public Ind(string name, string initialValue = "0")
        {
            this.Name = name;
            this.Type = Types.Character;
            this.InitValue = initialValue;

            this.Dimentions = 1;
            this.Value = new object[this.Dimentions];

            this.DoInitialValue();
        }

        public override void Set(object value, int index = 0)
        {
            if (value is string)
                if (value.ToString() == "1" || value.ToString() == "0")
                    this.Value[index] = value;
                else
                    Error.ThrowRuntimeError("Indicator type", "Cannot assign '" + value.ToString() + "' to an indicator.");
            else if (value is bool)
                this.Value[index] = ((bool)value == true ? "1" : "0");
            else
                Error.ThrowRuntimeError("Indicator type", "Cannot assign " + value.GetType().ToString() + " to an indicator.");
        }
    }
}
