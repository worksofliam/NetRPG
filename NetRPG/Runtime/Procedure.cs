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

        private bool _HasEntrypoint;

        public Procedure(string Name, Types ReturnType = Types.Void)
        {
            _Name = Name;
            _ReturnType = ReturnType;
            _Instructions = new List<Instruction>();
            _DataSets = new Dictionary<string, DataSet>();
            _HasEntrypoint = false;
        }

        public void AddDataSet(DataSet var)
        {
            _DataSets.Add(var._Name, var);
        }

        public void AddInstruction(Instructions Instruction, string Value = "")
        {
            _Instructions.Add(new Instruction(Instruction, Value));

            if (Instruction == Instructions.ENTRYPOINT) 
                _HasEntrypoint = true;
        }

        //TODO: Get variables
        public Instruction[] GetInstructions() => _Instructions.ToArray();
        public string[] GetDataSetList() => _DataSets.Keys.ToArray();
        public DataSet GetDataSet(string Name)
        {
            //TODO: Throw if no exist?
            if (_DataSets.ContainsKey(Name))
                return _DataSets[Name];
            else {
                Error.ThrowRuntimeError("Proedure.GetDataSet", Name + " does not exist in " + _Name);
                return null;
            }
        }

        public string GetName() => _Name;
        public bool HasEntrypoint => _HasEntrypoint;
    }
}
