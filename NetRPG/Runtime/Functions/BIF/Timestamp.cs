using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Timestamp : Function
    {
        public override object Execute(object[] Parameters)
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}
