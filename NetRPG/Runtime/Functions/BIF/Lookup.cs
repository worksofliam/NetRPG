using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Lookup : Function
    {
        //TODO: handle LT, LE, GT, GE
        public override object Execute(object[] Parameters)
        {
            dynamic value = Parameters[0];
            object[] array = (Parameters[1] as object[]);
            int start = 0;
            int length = array.Length;

            if (Parameters.Length >= 3)
                start = Convert.ToInt32(Parameters[2]);
            if (Parameters.Length >= 4)
                start = Convert.ToInt32(Parameters[3]);

            for (int x = start; x < start + length; x++)
            {
                if ((bool)VM.Operate(Instructions.EQUAL, array[x], value) == true)
                    return (x + 1); //RPG arrays <_<
            }

            return 0;
        }
    }
}
