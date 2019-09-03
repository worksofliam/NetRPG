using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.System
{
    class printf : Function
    {
        public override object Execute(object[] Parameters)
        {
            string output = Parameters[0] as string;
            Console.Write(output);
            return output.Length;
        }
    }
}
