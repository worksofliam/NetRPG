using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using System.IO;

namespace NetRPG.Runtime.Functions.Operation
{
    class Reset : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is DataValue)
            {
                (Parameters[0] as DataValue).DoInitialValue();
            }
            else
            {
                Error.ThrowRuntimeError("IN", "Variable reference needed for RESET.");
            }
            return null;
        }
    }
}
