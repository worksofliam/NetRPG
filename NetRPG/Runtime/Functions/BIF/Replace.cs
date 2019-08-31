using System.Text;
namespace NetRPG.Runtime.Functions.BIF
{
    class Replace : Function
    {
        public override object Execute(object[] parameters)
        {
            string replacement = "";
            string source = "";
            int startPos = 0;
            int sourceLength = source.Length;


            if (parameters.Length >= 2)
            {
                if (parameters[0] is string)
                {
                    replacement = (string)parameters[0];
                }
                else
                {
                    Error.ThrowRuntimeError("%REPLACE", "Invalid replacement string: " + parameters[0]);
                    return 0;
                }

                if (parameters[1] is string)
                {
                    source = (string)parameters[1];
                    sourceLength = source.Length;
                }
                else
                {
                    Error.ThrowRuntimeError("%REPLACE", "Invalid source string: " + parameters[1]);
                    return 0;
                }
            }

            if (parameters.Length == 3)
            {
                if (parameters[2] is int)
                {
                    startPos = (int)parameters[2];

                    // Start position -1 because RPG start to count from 1 and C# from 0
                    startPos--;
                }
                else
                {
                    Error.ThrowRuntimeError("%REPLACE", "Invalid start position: " + parameters[2]);
                    return 0;
                }
            }

            if (parameters.Length == 4)
            {
                if (parameters[3] is int)
                {
                    sourceLength = (int)parameters[3];
                }
                else
                {
                    Error.ThrowRuntimeError("%REPLACE", "Invalid source length: " + parameters[3]);
                    return 0;
                }
            }

            string sourceToReplace = source.Substring(startPos, sourceLength-startPos);

            StringBuilder sbSourceToReplace = new StringBuilder(sourceToReplace);
            for (int i = sbSourceToReplace.Length - 1; i <= sourceLength + replacement.Length; i++) 
            {
                sbSourceToReplace.Append(" ");
            }
            sbSourceToReplace.Remove(0, replacement.Length);
            sbSourceToReplace.Insert(0, replacement);

            StringBuilder sbSource = new StringBuilder(source);
            sbSource.Remove(startPos, sourceLength-startPos);
            sbSource.Insert(startPos, sbSourceToReplace.ToString().Trim());
            
            return sbSource.ToString().Trim();

        }
    }
}
