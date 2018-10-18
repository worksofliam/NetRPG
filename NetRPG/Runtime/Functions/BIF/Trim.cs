using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Trim : Function
    {
        public override object Execute(object[] Parameters)
        {
            string Value = (Parameters[0] as string);
            return Value.Trim();
        }
    }
}
