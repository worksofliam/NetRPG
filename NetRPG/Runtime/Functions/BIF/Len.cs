using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Len : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is string)
            {
                return Parameters[0].ToString().Length;
            }
            else
            {
                Error.ThrowRuntimeError("%Len", "Only supports determining the length of string types.");
                return 0;
            }
        }
    }
}
