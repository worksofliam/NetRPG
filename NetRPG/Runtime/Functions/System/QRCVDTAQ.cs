using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.System
{
    class QRCVDTAQ : Function
    {
        public override object Execute(object[] Parameters)
        {
            string lib = (Parameters[0] as Character).Get();
            string obj = (Parameters[1] as Character).Get();
            FixedDecimal length = (Parameters[2] as FixedDecimal);
            Character data = (Parameters[3] as Character);

            string output = DataQueue.Pop(lib.Trim() + obj.Trim());
            length.Set(output.Length);
            data.Set(output);

            return null;
        }
    }
}
