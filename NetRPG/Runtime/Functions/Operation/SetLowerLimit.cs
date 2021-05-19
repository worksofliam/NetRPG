using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.Operation
{
    [RPGFunctionAlias("SETLL")]
    class SetLowerLimit : Function
    {
        public override object Execute(object[] Parameters)
        {

            if (Parameters[0] is JSONTable)
            {
                JSONTable table = Parameters[0] as JSONTable;
                table.SetLowerLimit(Parameters[1] as dynamic[]);
            }
            else
            {
                //TODO: throw error: incorrect type
                Error.ThrowRuntimeError("SETLL", "Table is required.");
            }
            return null;
        }
    }
}
