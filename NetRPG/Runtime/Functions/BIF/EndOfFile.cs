using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.Operation
{
    class EndOfFile : Function
    {
        public override object Execute(object[] Parameters)
        {

            if (Parameters[0] is Table)
            {
                Table table = Parameters[0] as Table;
                return (table.isEOF() ? "1" : "0");
            }
            else
            {
                Error.ThrowRuntimeError("%EOF", "Table is required.");
            }
            return null;
        }
    }
}
