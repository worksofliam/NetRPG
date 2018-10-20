using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Int : Function
    {
        public override object Execute(object[] Parameters)
        {
            return Convert.ToInt32(Parameters[0]);
        }
    }
}
