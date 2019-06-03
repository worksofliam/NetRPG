using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using System.IO;

namespace NetRPG.Runtime.Functions.Operation
{
    class Clear : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is DataValue)
            {
                (Parameters[0] as DataValue).DoInitialValue(true);
            }
            else
            {
                Error.ThrowRuntimeError("IN", "Variable reference needed for CLEAR.");
            }
            return null;
        }
    }
}
