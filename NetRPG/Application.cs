using NetRPG.Language;
using System;
using System.Collections.Generic;
using NetRPG.Runtime;
using System.Runtime.InteropServices;
using System.IO;

namespace NetRPG {
    public class ApplicationRuntime {
        public static void Execute(string path) {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string NewLine = (isWindows ? Environment.NewLine : "");
            Preprocessor prep;
            RPGLex lexer;
            Statement[] Statements;
            Reader reader;
            VM vm;

            prep = new Preprocessor();
            prep.ReadFile(path);

            lexer = new RPGLex();
            lexer.Lex(String.Join(NewLine, prep.GetLines()));

            Statements = Statement.ParseDocument(lexer.GetTokens());

            reader = new Reader();
            reader.ReadStatements(Statements);

            vm = new VM(true);

            try {
                vm.AddModule(reader.GetModule());
                vm.Run();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}