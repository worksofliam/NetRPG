using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime
{
    class Module
    {
        private Dictionary<string, DataSet> GlobalDataSets;
        private List<Procedure> Procedures;

        //TODO: Does it need a name?
        public Module()
        {
            GlobalDataSets = new Dictionary<string, DataSet>();
            Procedures = new List<Procedure>();
        }

        public string[] GetDataSetList() => GlobalDataSets.Keys.ToArray();
        public DataSet GetDataSet(string Name) {
            return GlobalDataSets[Name];
        }

        public void AddDataSet(DataSet var)
        {
            GlobalDataSets.Add(var._Name, var);
        }

        public void AddProcedure(Procedure Procedure)
        {
            Procedures.Add(Procedure);
        }

        public void Print()
        {
            DataSet dataSet;
            foreach (string Name in this.GetDataSetList()) {
                dataSet = this.GetDataSet(Name);
                Console.WriteLine("\t" + Name + " " + dataSet._Type + "(" + dataSet?._Length + ") " + dataSet?._Dimentions);
            }

            Console.WriteLine();

            foreach (Procedure proc in Procedures)
            {
                Console.WriteLine("Procedure: " + proc._Name + ": " + proc._ReturnType.ToString());

                foreach (string Name in proc.GetDataSetList()) {
                    dataSet = proc.GetDataSet(Name);
                    Console.WriteLine("\t" + Name + " " + dataSet._Type + "(" + dataSet?._Length + ") " + dataSet?._Dimentions);
                }

                Console.WriteLine();
                
                foreach (Instruction inst in proc.GetInstructions())
                {
                    Console.WriteLine("\t" + inst._Instruction.ToString().PadRight(10) + " " + inst._Value);
                }

                Console.WriteLine();
            }
        }
    }
}
