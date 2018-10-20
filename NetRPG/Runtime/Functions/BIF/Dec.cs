using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Dec : Function
    {
        public override object Execute(object[] Parameters)
        {
            double Result;
            if (Parameters[0] is string || Parameters[0] is int)
            {
                Result = Convert.ToDouble(Parameters[0]);

                if (Parameters.Length >= 3)
                    Result = Math.Round(Result, (int)Parameters[2], MidpointRounding.AwayFromZero);

                return Result;
            }
            else
            {
                //TODO: throw error because incorrect type in
                Error.ThrowRuntimeError("%Dec", "Incorrect type passed into function.");
            }
            //TODO: Handle DateTime stuff
            //https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_72/rzasd/bbdec.htm#bbdec

            return 0.0;
        }
    }
}
