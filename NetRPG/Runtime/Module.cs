using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime
{
    class Module
    {
        private Dictionary<string, DataSet> GlobalVariables;
        private List<Procedure> Procedures;

        //TODO: Does it need a name?
        public Module()
        {
            GlobalVariables = new Dictionary<string, DataSet>();
            Procedures = new List<Procedure>();
        }

        public void AddVariable(DataSet var)
        {
            GlobalVariables.Add(var._Name, var);
        }

        public void AddProcedure(Procedure Procedure)
        {
            Procedures.Add(Procedure);
        }

        public void Print()
        {
            foreach (Procedure proc in Procedures)
            {
                Console.WriteLine("Procedure: " + proc._Name + ": " + proc._ReturnType.ToString());
                
                foreach (Instruction inst in proc.GetInstructions())
                {
                    Console.WriteLine("\t" + inst._Instruction.ToString().PadRight(10) + " " + inst._Value);
                }

                Console.WriteLine();
            }
        }
    }
}
