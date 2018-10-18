using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class DecPos : Function
    {
        public override object Execute(object[] Parameters)
        {
            decimal argument = Convert.ToDecimal(Parameters[0]);
            return BitConverter.GetBytes(decimal.GetBits(argument)[3])[2];
        }
    }
}
