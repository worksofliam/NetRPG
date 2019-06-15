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
                Testing.RunTests("dcl_file_exfmt3");
                Console.ReadKey();
            } else {
                ApplicationRuntime.Execute(args[0]);
            }
        }
    }
}
