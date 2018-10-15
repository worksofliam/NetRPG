using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime
{
    /// <summary>
    /// A DataSet can be a variable, file, or struct
    /// </summary>
    public class DataSet
    {
        public string _Name;
        public Types _Type;
        public int _Length;
        public int _Dimentions; //If 0, not an array

        public object _InitialValue;

        public DataSet(string name)
        {
            _Name = name;
        }

        public bool IsArray() => (_Dimentions > 0);
    }

    public class Instruction
    {
        public Instructions _Instruction;
        public string _Value;
        public Instruction(Instructions instruction, string value = null)
        {
            _Instruction = instruction;
            _Value = value;
        }
    }

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
        public DataSet GetDataSet(string Name) {
            return _DataSets[Name];
        }
    }

    public enum Instructions
    {
        NOP, //Do nothing
        PARMS, //Do nothing

        LDVAR, //Load var onto stack
        LDARR, //Load from array

        LDSTR, //Load constant string onto stack
        LDNUM, //Load constant numeric onto stack

        STVAR, //Store variable
        STARR, //Store into array

        LABEL,
        CALL, //Functions
        ADD,
        SUB,
        MUL,
        DIV,
        APPEND,
        NOT, //Negate
        EQUAL,
        ASSIGN,
        RETURN,
        ENTRYPOINT, //Program entry point
        BRFALSE, //Break if false
        BRTRUE, //Break if true
        BR //Break
    };

    public enum Types
    {
        Void,
        Pointer,
        Structure,
        String,
        Character,
        Varying,
        Ind, //Char(1)
        Int64, //Int(20)
        Int32, //Int(10)
        Int16, //Int(5)
        Int8, //Int(3);
        Double, //Float(8)
        Float, //Float(4)

        Zoned, //Float(8)
        Packed //Float(8)
    }
}
