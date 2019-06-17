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
            if (Parameters[0] is Structure && Parameters[1] is Display)
            {
                Display display = Parameters[1] as Display;
                display.Write(Parameters[0] as Structure);
            }
            else
            {
                //TODO: throw error: incorrect type
                Error.ThrowRuntimeError("WRITE", "Record format is required.");
            }
            return null;
        }
    }
}
