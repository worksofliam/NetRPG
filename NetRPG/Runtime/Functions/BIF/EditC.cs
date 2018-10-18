using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime.Functions.BIF
{
    class EditC : Function
    {
        public override object Execute(object[] Parameters)
        {
            //TODO: does not handle X, Y, Z editcodes

            string[] CommaEdits = { "1", "2", "A", "B", "J", "K", "N", "O" };
            string[] CREdits = { "A", "B", "C", "D" };

            string[] NoSignEdits = { "1", "2", "3", "4" };
            string[] SignOnRightEdits = { "N", "O", "P", "Q" };
            bool SignOnRight = false;

            double value = Convert.ToDouble(Parameters[0]);
            string editcode = (Parameters[1] as string);
            string curSym = (Parameters?[2] as string);
            if (curSym == "") curSym = "£";

            decimal valueD = Convert.ToDecimal(value);
            int DecimalPlaces = 0;
            
            DecimalPlaces = BitConverter.GetBytes(decimal.GetBits(valueD)[3])[2];

            string Result = "";
            string Sign = "";

            if (CommaEdits.Contains(editcode))
                if (DecimalPlaces > 0)
                    Result = String.Format("{0:n}", value);
                else
                    Result = String.Format("{0:n0}", value);
            else
                Result = value.ToString();

            Result = Result.TrimStart('-');
            
            if (value < 0)
                if (CREdits.Contains(editcode))
                {
                    Sign = "CR";
                    SignOnRight = true;
                }
                else
                    Sign = "-";

            if (SignOnRightEdits.Contains(editcode))
                SignOnRight = true;

            if (NoSignEdits.Contains(editcode))
                Sign = "";

            Result = curSym + Result;

            if (SignOnRight)
                Result += Sign;
            else
                Result = Sign + Result;

            return Result;
        }
    }
}
