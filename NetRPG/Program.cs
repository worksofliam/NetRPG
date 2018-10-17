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
            string SourcePath = Path.Combine(Environment.CurrentDirectory, "RPGCode", "test1.rpgle");
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string NewLine = (isWindows ? Environment.NewLine : "");
            
            Preprocessor prep = new Preprocessor();

            prep.ReadFile(SourcePath);

            RPGLex lexer = new RPGLex();
            lexer.Lex(String.Join(NewLine, prep.GetLines()));

            Statement[] Statements = Statement.ParseDocument(lexer.GetTokens());

            Reader reader = new Reader();
            reader.ReadStatements(Statements);
            reader.GetModule().Print();

            VM vm = new VM();
            vm.AddModule(reader.GetModule());
            vm.Run();
        }
    }
}
