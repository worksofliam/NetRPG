using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;

namespace NetRPG.Runtime.Functions.System
{
    class QSNDDTAQ : Function
    {
        public override object Execute(object[] Parameters)
        {
            string lib = (Parameters[0] as Character).Get();
            string obj = (Parameters[1] as Character).Get();
            int length = Convert.ToInt32((Parameters[2] as FixedDecimal).Get());
            string data = (Parameters[3] as Character).Get();

            DataQueue.Push(lib.Trim() + obj.Trim(), data.Substring(0, Math.Min(length, data.Length)));

            return null;
        }
    }
}
