using System;
using System.IO;
using System.Runtime.InteropServices;
using NetRPG.Language;
using NetRPG.Runtime;

namespace NetRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) {
                Testing.RunTests("op_reset");
            } else {
                Application.Execute(args[0]);
            }

            //Console.ReadLine();
        }
    }
}
