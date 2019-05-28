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
            { "dcl_ind.rpgle", "101" },
            { "dcl_char_array.rpgle", 6 },
            { "dcl_ds_qualified.rpgle", "Hello     World     10" },
            { "dcl_ds_qualified_array.rpgle", "Hello     World     3" },
            { "dcl_ds_subf_array.rpgle", "Hello     World     " },
            { "dcl_ds_nonqualified.rpgle", "Hello123.45" },
            { "dcl_array_subf_array.rpgle", "Hello     World     " },
            { "dcl_proc_1.rpgle", "World" },
            { "dcl_proc_2.rpgle", "World" },
            { "dcl_proc_3.rpgle", "World" },
            { "dcl_proc_4.rpgle", "Hello!" },
            { "dcl_proc_5.rpgle", "Hello     " },
            { "dcl_proc_6.rpgle", "001011" },
            { "dcl_proc_7.rpgle", "World" },
            { "dcl_proc_8.rpgle", "Hello" },
            { "dcl_inz.rpgle", "HelloWorld421234.5655" },
            { "dcl_likeds.rpgle", "     Hello" },
            { "assignment.rpgle", "Hello world    23421234.56" },
            { "addition.rpgle", "Hello world    23421234.56" },
            { "subtraction.rpgle", "5304.33" },
            { "division.rpgle", "44.1" },
            { "multiplication.rpgle", "20648.66" },
            { "bif_abs.rpgle", 5 },
            { "bif_char.rpgle", "5512.34"},
            { "bif_dec.rpgle", 1234.56 },
            { "bif_decpos.rpgle", 2 },
            { "bif_float.rpgle", 5f },
            { "bif_int.rpgle", 10 },
            { "bif_len.rpgle", 50 },
            { "bif_lookup.rpgle", 4 },
            { "bif_trim.rpgle", "Hello" },
            { "bif_trim_2.rpgle", "Hello          " },
            { "bif_trimr.rpgle", "    Hello"},
            { "bif_triml.rpgle", "Hello           "},
            { "op_if.rpgle", 3 },
            { "op_else.rpgle", 2 },
            { "op_elseif.rpgle", 2 },
            { "op_select.rpgle", 3 },
            { "op_dow.rpgle", 11 },
            { "op_dow2.rpgle", 11},
            { "op_dsply.rpgle", 1 },

            { "op_file_open.rpgle", 1 },
            { "op_file_read.rpgle", "My first p" },
            { "op_file_read2.rpgle", "My second "},
            { "op_file_read_qualified.rpgle", "My second " },
            { "op_file_readp.rpgle", "My first p" },
        };

        public static void RunTests(string testsStarting = "")
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            string SourcePath;
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string NewLine = (isWindows ? Environment.NewLine : "");
            Preprocessor prep;
            RPGLex lexer;
            Statement[] Statements;
            Reader reader;
            VM vm;

            int run = 0, passed = 0, failed = 0;
            Exception lastError = null;

            dynamic result;

            foreach (string file in TestCases.Keys)
            {
                if (testsStarting == "" || file.StartsWith(testsStarting))
                {
                    run++;

                    Console.Write("Testing " + file.PadRight(35) + " ... ");
                    SourcePath = Path.Combine(Environment.CurrentDirectory, "RPGCode", file);

                    prep = new Preprocessor();
                    prep.ReadFile(SourcePath);

                    lexer = new RPGLex();
                    lexer.Lex(String.Join(NewLine, prep.GetLines()));

                    Statements = Statement.ParseDocument(lexer.GetTokens());

                    reader = new Reader();
                    reader.ReadStatements(Statements);

                    vm = new VM(true);

                    try
                    {
                        vm.AddModule(reader.GetModule());
                        result = vm.Run();
                    }
                    catch (Exception e)
                    {
                        lastError = e;
                        result = null;
                    }

                    if (result != null && result == TestCases[file])
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("successful.");
                        Console.ForegroundColor = originalColor;

                        passed++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("failed.");
                        Console.ForegroundColor = originalColor;
                        Console.WriteLine();
                        if (lastError != null)
                        {
                            Console.WriteLine(lastError.Message);
                            Console.WriteLine(lastError.StackTrace);
                            Console.WriteLine();
                        }
                        reader.GetModule().Print();
                        Console.WriteLine("\tExpected: '" + Convert.ToString(TestCases[file]) + "'");
                        Console.WriteLine("\tReturned: '" + Convert.ToString(result) + "'");
                        Console.WriteLine();

                        lastError = null;
                        failed++;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine(run.ToString() + " ran, " + passed.ToString() + " passed, " + failed.ToString() + " failed");
        }
    }
}
