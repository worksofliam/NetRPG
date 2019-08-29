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
            Console.Write(Parameters[0]);
            return null;
        }
    }
}
