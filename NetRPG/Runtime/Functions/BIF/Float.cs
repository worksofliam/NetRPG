using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Float : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is string)
            {
                return float.Parse(Parameters[0].ToString());
            }
            else
            {
                Error.ThrowRuntimeError("%Float", "Only supports converting string to float.");
            }

            return 0.0f;
        }
    }
}
