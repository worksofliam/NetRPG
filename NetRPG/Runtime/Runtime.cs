using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime
{

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

    public enum Instructions
    {
        NOP, //Do nothing
        PARMS, //Do nothing

        LDGBL, //Load from global variables
        LDVAR, //Load DataSet onto stack - e.g. variable, struct or array
        LDARR, //Get element from array: Stack-1 is array, Stack-0 is indec
        LDFLD, //Load field from struct: Stack-1 is struct, Stack-1 is name        

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
        OR,
        EQUAL,
        NOT_EQUAL,
        GREATER, //Stack-1 is great than Stack-0
        GREATER_EQUAL,
        LESSER, //Stack-1 is less than Stack-0
        LESSER_EQUAL,
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
