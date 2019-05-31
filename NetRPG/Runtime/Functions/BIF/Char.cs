using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Char : Function
    {
        public override object Execute(object[] Parameters)
        {
            dynamic value = Parameters[0];
            if (Parameters.Length == 1) {
                return Convert.ToString(value);
            } else {
                if (Parameters[0] is int) {
                    DateTimeOffset time = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(value));
                    return time.ToString(Parameters[1].ToString());
                } else {
                    Error.ThrowRuntimeError("%CHAR", "Unsure how to handle input data.");
                    return null;
                }
            }
        }
    }
}
