using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Xlate : Function
    {
        public override object Execute(object[] Parameters)
        {
            int startFrom = 0;
            if (Parameters.Length == 4) {
                startFrom = Convert.ToInt32(Parameters[3]);
                startFrom--; //RPG is zero-indexed
            }

            if (Parameters[0] is string && Parameters[1] is string && Parameters[2] is string) {
                string result = Parameters[2].ToString().Substring(startFrom);
                int charLength = Math.Max(Parameters[0].ToString().Length, Parameters[1].ToString().Length);
                char[] fromChar = Parameters[0].ToString().ToCharArray(), toChar = Parameters[1].ToString().ToCharArray();
                
                for (var i = 0; i < charLength; i++)
                  result = result.Replace(fromChar[i], toChar[i]);

                return Parameters[2].ToString().Substring(0, startFrom) + result;

            } else {
                Error.ThrowRuntimeError("%Scan", "Requires strings.");
                return 0;
            }
        }
    }
}
