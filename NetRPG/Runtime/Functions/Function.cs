using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions
{
    class Function
    {
        private static Dictionary<string, Function> List = new Dictionary<string, Function>
        {
            { "DSPLY", new Operation.Dsply() },
            { "%ABS", new BIF.Abs() },
            { "%CHAR", new BIF.Char() },
            { "%DEC", new BIF.Dec() },
            { "%DECH", new BIF.Dec() },
            { "%DECPOS", new BIF.DecPos() },
            { "%EDITC", new BIF.EditC() },
            { "%FLOAT", new BIF.Float() },
            { "%INT", new BIF.Int() },
            { "%LEN", new BIF.Len() },
            { "%LOOKUP", new BIF.Lookup() },
            { "%TRIM", new BIF.Trim() },
        };

        public static Function GetFunction(string Name)
        {
            Name = Name.ToUpper();

            if (List.ContainsKey(Name))
                return List[Name];
            else
                return null; //TODO: throw error
        }
        
        public virtual object Execute(object[] Parameters)
        {
            return null;
        }
    }
}
