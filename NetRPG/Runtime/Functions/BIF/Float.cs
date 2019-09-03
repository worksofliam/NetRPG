using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Float : Function
    {
        public override object Execute(object[] Parameters)
        {
            return Convert.ToDouble(Parameters[0]);
        }
    }
}
