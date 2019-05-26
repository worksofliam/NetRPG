using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;

namespace NetRPG.Runtime.Functions.Operation
{
    class Open : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is Table)
            {
                Table table = Parameters[0] as Table;
                table.Open();
            }
            else
            {
                //TODO: throw error: incorrect type
                Error.ThrowRuntimeError("OPEN", "Table is required.");
            }
            return null;
        }
    }
}
