using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions.BIF
{
    class Dec : Function
    {
        public override object Execute(object[] Parameters)
        {
            //TODO: handle precision
            if (Parameters[0] is string || Parameters[0] is int)
            {
                return Convert.ToDouble(Parameters[0]);
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
