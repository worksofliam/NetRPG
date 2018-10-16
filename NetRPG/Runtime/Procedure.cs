using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime
{
    public class Procedure
    {
        public string _Name;
        public Types _ReturnType;
        private List<Instruction> _Instructions;
        private Dictionary<string, DataSet> _DataSets;

        public Procedure(string Name, Types ReturnType = Types.Void)
        {
            _Name = Name;
            _ReturnType = ReturnType;
            _Instructions = new List<Instruction>();
            _DataSets = new Dictionary<string, DataSet>();
        }

        public void AddDataSet(DataSet var)
        {
            _DataSets.Add(var._Name, var);
        }

        public void AddInstruction(Instructions Instruction, string Value = "")
        {
            _Instructions.Add(new Instruction(Instruction, Value));
        }

        //TODO: Get variables
        public Instruction[] GetInstructions() => _Instructions.ToArray();
        public string[] GetDataSetList() => _DataSets.Keys.ToArray();
        public DataSet GetDataSet(string Name)
        {
            return _DataSets[Name];
        }
    }
}
