using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    [RPGFunctionAlias("%TIMESTAMP")]
    [RPGFunctionAlias("%TIME")]
    class Timestamp : Function
    {
        public override object Execute(object[] Parameters)
        {
            switch (Parameters.Length) {
                case 0:
                    return (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                case 1:
                    if (Parameters[0] is string) {
                        return (DateTime.Parse(Parameters[0].ToString()) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                    } else {
                        Error.ThrowRuntimeError("%Timestamp", "String required");
                        return null;
                    }

                case 2:
                    if (Parameters[0] is string && Parameters[1] is string) {
                        return (DateTime.ParseExact(Parameters[0].ToString(), Parameters[1].ToString(), null) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                    } else {
                        Error.ThrowRuntimeError("%Timestamp", "Strings required");
                        return null;
                    }
                default:
                    Error.ThrowRuntimeError("%Timestamp", "Too many parameters passed.");
                    return null;
            }
        }
    }
}
