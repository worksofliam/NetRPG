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
            { "%CHAR", new BIF.Char() }
        };

        public static Function GetFunction(string Name)
        {
            Name = Name.ToUpper();

            if (List.ContainsKey(Name))
                return List[Name];
            else
                return null; //TODO: throw error
        }

        protected int _ParametersCount;

        public Function(int parameterCount = 0)
        {
            _ParametersCount = parameterCount;
        }

        public int GetParameterCount() => this._ParametersCount;

        public virtual object Execute(object[] Parameters)
        {
            return null;
        }
    }
}
