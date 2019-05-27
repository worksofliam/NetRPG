using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;

namespace NetRPG.Runtime.Functions.Operation
{
    class Read : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is Table)
            {
                Table table = Parameters[0] as Table;
                table.Read();
            }
            else
            {
                //TODO: throw error: incorrect type
                Error.ThrowRuntimeError("READ", "Table is required.");
            }
            return null;
        }
    }
}
