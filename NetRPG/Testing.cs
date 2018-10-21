using NetRPG.Language;
using System;
using System.Collections.Generic;
using NetRPG.Runtime;
using System.Runtime.InteropServices;
using System.IO;

namespace NetRPG
{
    class Testing
    {
        private static Dictionary<string, dynamic> TestCases = new Dictionary<string, dynamic>()
        {
            { "dcl_char.rpgle", "Hello world    " },
            { "dcl_int.rpgle", 200 },
            { "dcl_fixed.rpgle", 24691.35 },
            { "dcl_char_array.rpgle", 6 },
            { "dcl_ds_qualified.rpgle", "Hello     World     10" },
            { "dcl_ds_qualified_array.rpgle", "Hello     World     3" }
        };

        public static void RunTests()
        {
            string SourcePath;
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string NewLine = (isWindows ? Environment.NewLine : "");
            Preprocessor prep;
            RPGLex lexer;
            Statement[] Statements;
            Reader reader;
            VM vm;

            dynamic result;

            foreach (string file in TestCases.Keys)
            {
                Console.Write("Testing " + file.PadRight(20) + " ... ");
                SourcePath = Path.Combine(Environment.CurrentDirectory, "RPGCode", file);

                prep = new Preprocessor();
                prep.ReadFile(SourcePath);

                lexer = new RPGLex();
                lexer.Lex(String.Join(NewLine, prep.GetLines()));

                Statements = Statement.ParseDocument(lexer.GetTokens());

                reader = new Reader();
                reader.ReadStatements(Statements);

                vm = new VM(true);
                vm.AddModule(reader.GetModule());
                try
                {
                    result = vm.Run();
                }
                catch (Exception e)
                {
                    result = null;
                }

                if (result == TestCases[file])
                {
                    Console.WriteLine("successful.");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(".. failed.");
                    Console.WriteLine();
                    reader.GetModule().Print();
                    Console.WriteLine();
                    Console.WriteLine("\tExpected: " + Convert.ToString(TestCases[file]));
                    Console.WriteLine("\tReturned: " + Convert.ToString(result));
                    Console.WriteLine();
                }
            }
        }
    }
}
