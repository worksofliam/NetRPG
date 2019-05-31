using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Months : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is int) {
              return Convert.ToInt32(Parameters[0]) * 31536000;
            } else {
                Error.ThrowRuntimeError("%Months", "Requires integer parameter.");
                return 0;
            }
        }
    }
}
