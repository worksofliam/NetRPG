using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.Operation
{
    class Dsply : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is string)
            {
                string Result = (Parameters[0] as string);
                Console.WriteLine(Result);
            }
            else
            {
                //TODO: throw error: incorrect type
                Error.ThrowRuntimeError("DSPLY", "Only string type is accepted.");
            }
            return null;
        }
    }
}
