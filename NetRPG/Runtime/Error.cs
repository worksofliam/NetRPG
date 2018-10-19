
namespace NetRPG.Runtime
{
    public class Error
    {
        public static void ThrowError(string function, string message, int instruction = 0) {
            if (instruction == 0)
                throw new System.Exception(function + ": " + message);
            else
                throw new System.Exception(function + ": " + message + " (inst " + instruction.ToString() + ")");
        }
    }
}