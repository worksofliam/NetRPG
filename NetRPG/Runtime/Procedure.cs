using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime
{
    public class Procedure
    {
        public string _ParentModule;
        public string _Name;
        public Types _ReturnType;
        private List<Instruction> _Instructions;
        private Dictionary<string, bool> _Parameters; //Name, passByValue
        private Dictionary<string, DataSet> _DataSets;

        private Dictionary<string, int> Labels;

        private bool _HasEntrypoint;

        public Procedure(string Name, Types ReturnType = Types.Void)
        {
            _Name = Name;
            _ReturnType = ReturnType;
            _Instructions = new List<Instruction>();
            _Parameters = new Dictionary<string, bool>();
            _DataSets = new Dictionary<string, DataSet>();
            _HasEntrypoint = false;

            Labels = null;
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

        public Instruction[] GetInstructions() => _Instructions.ToArray();
        public string[] GetDataSetList() => _DataSets.Keys.ToArray();
        public DataSet GetDataSet(string Name)
        {
            if (_DataSets.ContainsKey(Name))
                return _DataSets[Name];
            else {
                Error.ThrowRuntimeError("Proedure.GetDataSet", Name + " does not exist in " + _Name);
                return null;
            }
        }
        public bool ContainsDataSet(string Name) => _DataSets.ContainsKey(Name);

        public void AddParameter(string value, bool byValue = false) => _Parameters.Add(value, byValue);
        public string[] GetParameterNames() => _Parameters.Keys.ToArray();
        public bool ParameterIsValue(string name) => _Parameters[name];
        public bool ParameterIsValue(int index) => _Parameters.ElementAt(index).Value;

        public string GetName() => _Name;
        public bool HasEntrypoint => _HasEntrypoint;

        public void CalculateLabels() {
            Labels = new Dictionary<string, int>();

            for(int i = 0; i < _Instructions.Count(); i++)
                if (_Instructions[i]._Instruction == Instructions.LABEL)
                    Labels.Add(_Instructions[i]._Value, i);
        }

        public int GetLabel(string Label) {
            if (!Labels.ContainsKey(Label))
                Error.ThrowRuntimeError("GetLabel", "Label '" + Label + " does not exist.");

            return Labels[Label];
        }
    }
}
