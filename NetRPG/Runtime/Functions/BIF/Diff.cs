using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Diff : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is int && Parameters[1] is int && Parameters[2] is string) {
                long high, low;

                high = Math.Max(Convert.ToInt64(Parameters[0]), Convert.ToInt64(Parameters[1]));
                low = Math.Min(Convert.ToInt64(Parameters[0]), Convert.ToInt64(Parameters[1]));

                DateTimeOffset a = DateTimeOffset.FromUnixTimeSeconds(high);
                DateTimeOffset b = DateTimeOffset.FromUnixTimeSeconds(low);
                TimeSpan span = a - b;

                switch (Parameters[2].ToString()) {
                    case "*SECONDS":
                    case "*S":
                        return span.TotalSeconds;
                    case "*MINUTES":
                    case "*MN":
                        return span.Minutes;
                    case "*HOURS":
                    case "*H":
                        return span.TotalHours;
                    case "*DAYS":
                    case "*D":
                        return span.TotalDays;
                    case "*MONTHS":
                    case "*M":
                        return span.TotalDays / 30;
                    case "*YEARS":
                    case "*Y":
                        return span.TotalDays / 365;
                }

                Error.ThrowRuntimeError("%Diff", "Unit " + Parameters[2].ToString() + " not supported.");
                return 0;
            } else {
                Error.ThrowRuntimeError("%Diff", "Requires two integer parameters and a unit.");
                return 0;
            }
        }
    }
}
