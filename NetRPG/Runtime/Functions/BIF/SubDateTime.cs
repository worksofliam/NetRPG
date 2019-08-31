using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    [RPGFunctionAlias("%SUBDT")]
    class SubDateTime : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is int && Parameters[1] is string) {
                DateTimeOffset a = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(Parameters[0]));

                switch (Parameters[1].ToString()) {
                    case "*SECONDS":
                    case "*S":
                        return a.Second;
                    case "*MINUTES":
                    case "*MN":
                        return a.Minute;
                    case "*HOURS":
                    case "*H":
                        return a.Hour;
                    case "*DAYS":
                    case "*D":
                        return a.Day;
                    case "*MONTHS":
                    case "*M":
                        return a.Month;
                    case "*YEARS":
                    case "*Y":
                        return a.Year;
                }

                Error.ThrowRuntimeError("%Subdt", "Unit " + Parameters[1].ToString() + " not supported.");
                return 0;
            } else {
                Error.ThrowRuntimeError("%Subdt", "Requires an integer parameter and a unit.");
                return 0;
            }
        }
    }
}
