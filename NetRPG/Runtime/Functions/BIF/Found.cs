using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.Operation
{
    [RPGFunctionAlias("%FOUND")]
    class Found : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is FileT)
            {
                FileT rla = Parameters[0] as FileT;
                return (rla.isEOF() ? "0" : "1");
            }
            else
            {
                Error.ThrowRuntimeError("%FOUND", "Table or display is required.");
            }
            return null;
        }
    }
}
