using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;

namespace NetRPG.Runtime.Functions.BIF
{
    class Elem : Function
    {
        public override object Execute(object[] Parameters)
        {
            if (Parameters[0] is DataValue) {
                return (Parameters[0] as DataValue).GetDimentions();
            } else {
                Error.ThrowRuntimeError("%Elem", "Only accepts data structures.");
                return null;
            }
        }
    }
}
