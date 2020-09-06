using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime
{
    public class Module
    {
        private Dictionary<string, string> FunctionReference; //Used for internal functions
        public bool _HasDisplay;
        private Dictionary<string, DataSet> GlobalDataSets;
        private List<Procedure> Procedures;

        //TODO: Does it need a name?
        public Module()
        {
            GlobalDataSets = new Dictionary<string, DataSet>();
            Procedures = new List<Procedure>();
            _HasDisplay = false;
            FunctionReference = new Dictionary<string, string>();
        }

        public string[] GetDataSetList() => GlobalDataSets.Keys.ToArray();
        public DataSet GetDataSet(string Name) {
            if (GlobalDataSets.ContainsKey(Name)) {
                return GlobalDataSets[Name];
            } else {
                Error.ThrowRuntimeError("Proedure.GetDataSet", Name + " does not exist globally.");
                return null;
            }
        }

        public void AddFunctionRef(string reference, string function) {
            if (!FunctionReference.ContainsKey(reference)) {
                FunctionReference.Add(reference, function);
            }
        }

        public string[] GetReferences() => FunctionReference.Keys.ToArray();

        public string GetReferenceFunc(string reference) => FunctionReference[reference];

        public Procedure[] GetProcedures() => Procedures.ToArray();

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
            int ip = 0;

            foreach (string Name in this.GetDataSetList()) {
                dataSet = this.GetDataSet(Name);
                Console.Write("\t" + Name + " " + dataSet._Type + "(" + dataSet?._Length + ") ");
                if (dataSet?._Dimentions > 0)
                    Console.Write("Dim(" + dataSet?._Dimentions + ")");

                Console.WriteLine();
            }
            Console.WriteLine();

            foreach (Procedure proc in Procedures)
            {
                ip = 0;
                Console.WriteLine("Procedure: " + proc._Name + ": " + proc._ReturnType.ToString());

                foreach (string Name in proc.GetDataSetList()) {
                    dataSet = proc.GetDataSet(Name);
                    Console.Write("\t" + Name + " " + dataSet._Type + "(" + dataSet?._Length + ") ");
                    if (dataSet?._Dimentions > 0)
                        Console.Write("Dim(" + dataSet?._Dimentions + ")");

                    Console.WriteLine();
                }

                Console.WriteLine();
                
                foreach (Instruction inst in proc.GetInstructions())
                {
                    Console.WriteLine("\t" + ip.ToString().PadRight(5) + inst._Instruction.ToString().PadRight(15) + " " + inst._Value);
                    ip++;
                }

                Console.WriteLine();
            }
        }
    }
}
