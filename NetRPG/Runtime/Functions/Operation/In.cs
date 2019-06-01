using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using System.IO;

namespace NetRPG.Runtime.Functions.Operation
{
    class In : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is DataValue)
            {
                string dataAreaName = (Parameters[0] as DataValue).GetDataArea();
                if (dataAreaName != null) {
                    return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "objects", dataAreaName));
                } else {
                    Error.ThrowRuntimeError("IN", "IN parameter requires DTAARA keyword.");
                }
            }
            else
            {
                Error.ThrowRuntimeError("IN", "Variable reference needed for IN.");
            }
            return null;
        }
    }
}
