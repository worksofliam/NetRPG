
namespace NetRPG
{
    public class Error
    {
        public static void ThrowRuntimeError(string function, string message, int instruction = 0) {
            if (instruction == 0)
                throw new System.Exception(function + ": " + message);
            else
                throw new System.Exception(function + ": " + message + " (inst " + instruction.ToString() + ")");
        }

        public static void ThrowCompileError(string message, int line = 0) {
            if (line == 0)
                throw new System.Exception(message);
            else
                throw new System.Exception(message + " (inst " + line.ToString() + ")");
        }
    }
}