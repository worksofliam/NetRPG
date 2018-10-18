using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Char : Function
    {
        public Char()
        {
            this._ParametersCount = 1;
        }

        public override object Execute(object[] Parameters)
        {
            dynamic value = Parameters[0];
            return Convert.ToSingle(value);
        }
    }
}
