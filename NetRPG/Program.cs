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
                Testing.RunTests();
            } else {
                ApplicationRuntime.Execute(args[0]);
            }

            // DisplayParse parser = new DisplayParse();

            // parser.ParseFile(Path.Combine("objects", "ex1.dspf"));

            //Console.ReadLine();
        }
    }
}
