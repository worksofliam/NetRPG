using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Scan : Function
    {
        public override object Execute(object[] Parameters)
        {
            int startFrom = 0;
            if (Parameters.Length == 3) {
                startFrom = Convert.ToInt32(Parameters[2]);
                startFrom--; //RPG is zero-indexed
            }

            if (Parameters[0] is string && Parameters[1] is string) {
              return Parameters[1].ToString().IndexOf(Parameters[0].ToString(), startFrom) + 1;
            } else {
                Error.ThrowRuntimeError("%Scan", "Requires strings.");
                return 0;
            }
        }
    }
}
