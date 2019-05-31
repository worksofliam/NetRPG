using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;

namespace NetRPG.Runtime.Functions.Operation
{
    class Chain : Function
    {
        public override object Execute(object[] Parameters)
        {

            if (Parameters[0] is Structure && Parameters[1] is Table)
            {
                Table table = Parameters[1] as Table;
                table.Chain(Parameters[0] as Structure, Parameters[2] as dynamic[]);
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
