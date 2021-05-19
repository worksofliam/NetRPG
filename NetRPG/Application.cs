using NetRPG.Language;
using System;
using System.Collections.Generic;
using NetRPG.Runtime;
using System.Runtime.InteropServices;
using System.IO;

namespace NetRPG {
    public class ApplicationRuntime {
        public static void Execute(string[] args) {
            List<string> paths = new List<string>();
            bool isDebug = false;

            foreach (string arg in args) {
                switch (arg) {
                    case "-d":
                    case "--debug":
                        isDebug = true;
                        break;

                    default:
                        paths.Add(arg);
                        break;
                }
            }

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string NewLine = (isWindows ? Environment.NewLine : "");

            VM vm = new VM(isDebug);

            Preprocessor prep;

            foreach (string path in paths) {
                Statement[] Statements;
                Reader reader;
                
                prep = new Preprocessor();
                prep.ReadFile(path);

                vm.AddDebugView(path, prep.GetLinesAsStrings());

                Statements = Statement.ParseDocument(Preprocessor.GetTokens(prep.GetLines()));

                reader = new Reader(isDebug);
                reader.SetSourcePath(path);
                reader.ReadStatements(Statements);

                try {
                    vm.AddModule(reader.GetModule());
                } catch (Exception e) {
                    Console.WriteLine("Failed to add module: " + path);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                } 
            }

            try {
                vm.Run();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}