using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.Operation
{
    [RPGFunctionAlias("%EOF")]
    class EndOfFile : Function
    {
        public override object Execute(object[] Parameters)
        {

            if (Parameters[0] is ODBCTable)
            {
                ODBCTable table = Parameters[0] as ODBCTable;
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
