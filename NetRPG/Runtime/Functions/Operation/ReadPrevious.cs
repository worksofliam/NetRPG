using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.Operation
{
    [RPGFunctionAlias("READP")]
    class ReadPrevious : Function
    {
        public override object Execute(object[] Parameters)
        {

            if (Parameters[0] is Structure && Parameters[1] is JSONTable)
            {
                JSONTable table = Parameters[1] as JSONTable;
                table.ReadPrevious(Parameters[0] as Structure);
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
