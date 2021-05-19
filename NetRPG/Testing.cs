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
            { "dcl_varchar.rpgle", "Hello world" },
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
            { "dcl_proc_9_value.rpgle", "hello     " }, 
            { "dcl_proc_10_value.rpgle", "10" }, 
            { "dcl_inz.rpgle", "HelloWorld421234.5655" },
            { "dcl_likeds.rpgle", "     Hello" },
            { "assignment.rpgle", "Hello world    23421234.56" },
            { "assignment2.rpgle", 12 },
            { "assignment3.rpgle", 12 },
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
            { "bif_elem1.rpgle", 2 },
            { "bif_elem2.rpgle", 4 },
            { "bif_scan.rpgle", "70" },
            { "bif_scanrpl.rpgle", "Hello friends  " },
            { "bif_xlate1.rpgle", "HELLO WORLD" },
            { "bif_xlate2.rpgle", "hello WORLD" },
            { "bif_subst.rpgle", "HelloWorldWordef" },
            { "bif_subst2.rpgle", "Wor" },
            { "bif_replace.rpgle", "Toronto, CanadaToronto, ONScarborough, Ontario, Canada" },

            { "time_dcl_timestamp.rpgle", 500 },
            { "time_bif_timestamp.rpgle", true },
            { "time_bif_timestamp2.rpgle", true },
            { "time_bif_timestamp3.rpgle", true },
            { "time_bif_date1.rpgle", true },
            { "time_bif_time1.rpgle", true },
            { "time_bif_days.rpgle", 172800 },
            { "time_bif_hours.rpgle", 7200 },
            { "time_bif_minutes.rpgle", 120 },
            { "time_bif_seconds.rpgle", 47 },
            { "time_date_literal.rpgle", true },
            { "time_time_literal.rpgle", true },
            { "time_dcl_date.rpgle", 860198400 },
            { "time_dcl_time.rpgle", true },
            { "time_bif_char1.rpgle", "05/04/97" },
            { "time_bif_char2.rpgle", "04/05/97" },
            { "time_bif_char3.rpgle", "97/04/05" },
            { "time_bif_char4.rpgle", "1997-04-05-12.00.00" },
            { "time_bif_diff_days.rpgle", 1 },
            { "time_bif_diff_seconds.rpgle", 86400 },
            { "time_bif_subdt.rpgle", "199745615" },

            { "op_if.rpgle", 3 },
            { "op_else.rpgle", 2 },
            { "op_elseif.rpgle", 2 },
            { "op_select.rpgle", 3 },
            { "op_other1.rpgle", 3 },
            { "op_other2.rpgle", 3 },
            { "op_dow.rpgle", 11 },
            { "op_dow2.rpgle", 11},
            { "op_iter1.rpgle", 9 },
            { "op_leave1.rpgle", 5 },
            { "op_leave2.rpgle", 5 },
            { "op_dsply.rpgle", 1 },
            { "op_eval1.rpgle", 10 },
            { "op_for1.rpgle", 11 },
            { "op_for2.rpgle", 0 },
            { "op_for3.rpgle", 5 },
            { "op_for4.rpgle", 10 },

            { "op_reset1.rpgle", "Hello world" },
            { "op_reset2.rpgle", 134 },
            { "op_reset3.rpgle", "Hello world134" },
            { "op_reset4.rpgle", 134 },
            { "op_reset5.rpgle", "Hello     Hello     2121" },

            { "op_clear1.rpgle", "           "},
            { "op_clear2.rpgle", 0 },
            { "op_clear3.rpgle", "           0" },
            { "op_clear4.rpgle", 0 },

            { "op_in1.rpgle", "Hello world    " },
            { "op_in2.rpgle", 2342359 },
            { "op_in3.rpgle", 12345.67 }, 

            //Classic JSON RLA tests
            { "op_file_open.rpgle", 1 },
            { "op_file_read.rpgle", "My first p" },
            { "op_file_read2.rpgle", "My second "},
            { "op_file_read3.rpgle", 56 },
            { "op_file_read_qualified.rpgle", "My second " },
            { "op_file_readp.rpgle", "My first p" },
            { "op_file_chain.rpgle", "My second "},
            { "op_file_chain2.rpgle", "My second My first p"},
            { "op_file_chain3.rpgle", "bethMy second"},
            { "op_file_setll1.rpgle", "101"},
            { "op_file_setll2.rpgle", "1E010"},

            //ODBC tests...
            // { "op_file_open.rpgle", true },
            // { "op_file_read.rpgle", "A00" },
            // { "op_file_read2.rpgle", "B01"},
            // { "op_file_read_qualified.rpgle", "B01" },
            // { "op_file_readp.rpgle", "A00" },
            // { "op_file_chain.rpgle", "DEVELOPMENT CENTER"},
            // { "op_file_chain2.rpgle", "000020000100"},
            // { "op_file_chain3.rpgle", "000020LOGIC"},
            
            // { "op_odbc_real_read1.rpgle", "SPIFFY COMPUTER SERVICE DIV." },
            // { "op_odbc_real_read2.rpgle", 14 },
            // { "op_odbc_real_chain.rpgle", "OPERATIONS" },

            { "ind1.rpgle", "1" },
            { "ind2.rpgle", "1" },
            { "ind3.rpgle", "1" },

            { "dcl_shared1.rpgle", "Hi        " },
            { "ile_module_a.rpgle,ile_module_b.rpgle", "You are Liam, you are 22 years old"},
            { "system_printf.rpgle", 12},
            { "system_dq.rpgle", "Hello world         "},

            { "hex1.rpgle", "Hello world£" },
            { "hex2.rpgle", "Hello world$" }
        };

        public static void RunTests(string testsStarting = "")
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            string SourcePath;
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string NewLine = (isWindows ? Environment.NewLine : "");
            Preprocessor prep;
            Statement[] Statements;
            Reader reader;
            VM vm;

            DateTime start, end;
            TimeSpan diff;

            int run = 0, passed = 0, failed = 0;
            Exception lastError = null;

            dynamic result;

            foreach (string files in TestCases.Keys)
            {
                if (testsStarting == "" || files.StartsWith(testsStarting))
                {
                    result = null;
                    lastError = null;

                    vm = new VM(true);
                    run++;

                    start = DateTime.Now;

                    foreach (string file in files.Split(',')) {
                        Console.Write("Testing " + file.PadRight(35) + " ... ");
                        SourcePath = Path.Combine(Environment.CurrentDirectory, "objects", file);

                        prep = new Preprocessor();
                        prep.ReadFile(SourcePath);

                        Statements = Statement.ParseDocument(Preprocessor.GetTokens(prep.GetLines()));

                        reader = new Reader();
                        reader.ReadStatements(Statements);

                        try {
                            vm.AddModule(reader.GetModule());
                        } catch (Exception e) {
                            lastError = e;
                        }
                    }

                    if (lastError == null) {
                        try
                        {
                            result = vm.Run();
                        }
                        catch (Exception e)
                        {
                            lastError = e;
                            result = null;
                        }
                    }
                    end = DateTime.Now;
                    diff = (end - start);

                    if (result != null && result == TestCases[files])
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("successful (" + diff.ToString(@"mm\:ss\:f\:ff") + ").");
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

                        Console.WriteLine("\tExpected: '" + Convert.ToString(TestCases[files]) + "'");
                        Console.WriteLine("\tReturned: '" + Convert.ToString(result) + "'");
                        Console.WriteLine();
                        vm.PrintModules();

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
