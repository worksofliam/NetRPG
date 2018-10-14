using System;
using System.Collections.Generic;
using System.Text;

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

        public DataSet(string name)
        {
            _Name = name;
        }

        public DataSet(string name, Types type, int length = 0, int dimentions = 0)
        {
            _Name = name;
            _Type = type;
            _Length = length;
            _Dimentions = 0;
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
        private Dictionary<string, DataSet> _Variables;

        public Procedure(string Name, Types ReturnType = Types.Void)
        {
            _Name = Name;
            _ReturnType = ReturnType;
            _Instructions = new List<Instruction>();
            _Variables = new Dictionary<string, DataSet>();
        }

        public void AddVariable(DataSet var)
        {
            _Variables.Add(var._Name, var);
        }

        public void AddInstruction(Instructions Instruction, string Value = "")
        {
            _Instructions.Add(new Instruction(Instruction, Value));
        }

        //TODO: Get variables
        public Instruction[] GetInstructions() => _Instructions.ToArray();
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

        GOTO,
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
        ENTRYPOINT //Program entry point
    };

    public enum Types
    {
        Void,
        Pointer,
        Structure,
        String,
        Character,
        Varying,
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
