using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    [RPGFunctionAlias("%SCANRPL")]
    class ScanReplace : Function
    {
        public override object Execute(object[] Parameters)
        {
            int startFrom = 0;
            if (Parameters.Length == 4) {
                startFrom = Convert.ToInt32(Parameters[2]);
                startFrom--; //RPG is zero-indexed
            }

            if (Parameters[0] is string && Parameters[1] is string && Parameters[2] is string) {
              return Parameters[2].ToString().Substring(startFrom).Replace(Parameters[0].ToString(), Parameters[1].ToString());
            } else {
                Error.ThrowRuntimeError("%Scan", "Requires strings.");
                return 0;
            }
        }
    }
}
