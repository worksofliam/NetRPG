using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.Operation
{
    class Write : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is Structure)
            {
                if (Parameters[1] is Display || Parameters[1] is Table) {
                    (Parameters[1] as FileT).Write(Parameters[0] as Structure);
                } else {
                    Error.ThrowRuntimeError("WRITE", "Display or table required.");
                }
            }
            else
            {
                Error.ThrowRuntimeError("WRITE", "Structure required.");
            }
            return null;
        }
    }
}
