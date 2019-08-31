namespace NetRPG.Runtime.Functions.BIF
{
    [RPGFunctionAlias("%SUBST")]
    class Substring : Function
    {
        public override object Execute(object[] parameters)
        {
            string str = "";
            int startPos = 0;
            int length = -1;

            if (parameters.Length < 2 || parameters.Length > 3)
            {
                Error.ThrowRuntimeError("%SUBST", "Invalid argument length: " + parameters.Length);
                return 0;
            }

            if (parameters.Length >= 2)
            {
                if (parameters[0] is string)
                {
                    str = (string)parameters[0];
                }
                else
                {
                    Error.ThrowRuntimeError("%SUBST", "Invalid string: " + parameters[0]);
                    return 0;
                }

                if (parameters[1] is int)
                {
                    startPos = (int)parameters[1];

                    // Start position -1 because RPG start to count from 1 and C# from 0
                    startPos--;
                }
                else
                {
                    Error.ThrowRuntimeError("%SUBST", "Invalid start position: " + parameters[1]);
                    return 0;
                }
            }

            if (parameters.Length == 3)
            {
                if (parameters[2] is int)
                {
                    length = (int)parameters[2];
                }
                else
                {
                    Error.ThrowRuntimeError("%SUBST", "Invalid length: " + parameters[2]);
                    return 0;
                }
            }

            if (length <= -1)
            {
                return str.Substring(startPos);
            }
            else
            {
                return str.Substring(startPos, length);
            }

        }
    }
}
