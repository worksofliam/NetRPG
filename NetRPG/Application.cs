using NetRPG.Language;
using System;
using System.Collections.Generic;
using NetRPG.Runtime;
using System.Runtime.InteropServices;
using System.IO;

namespace NetRPG {
    public class ApplicationRuntime {
        public static void Execute(string[] paths) {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string NewLine = (isWindows ? Environment.NewLine : "");
            VM vm = new VM(false);

            foreach (string path in paths) {
                Preprocessor prep;
                Statement[] Statements;
                Reader reader;
                
                prep = new Preprocessor();
                prep.ReadFile(path);

                Statements = Statement.ParseDocument(Preprocessor.GetTokens(prep.GetLines()));

                reader = new Reader();
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