using System;
using System.Collections.Generic;
using System.Linq;
using NetRPG.Runtime;

namespace NetRPG.Language
{
    class Reader
    {
        private Module _Module;
        private Procedure CurrentProcudure;

        public Reader()
        {
            _Module = new Module();
            CurrentProcudure = null;
        }

        public void ReadStatements(Statement[] Statements)
        {
            RPGToken[] tokens;

            foreach (Statement statement in Statements)
            {
                tokens = statement.GetTokens();
                switch (tokens[0].Type)
                {
                    case RPGLex.Type.DCL:
                        HandleDeclare(tokens);
                        break;
                    case RPGLex.Type.OPERATION:
                        HandleOperation(tokens);
                        break;
                }
            }

            if (CurrentProcudure != null)
                _Module.AddProcedure(CurrentProcudure);
        }

        private void HandleDeclare(RPGToken[] tokens)
        {
            //TODO: Check if DataSet already exists?
            DataSet variable = new DataSet(tokens[1].Value);
            Types varType;
            string varLength;
            Dictionary<string, string> config = new Dictionary<string, string>();
            
            for (int i = 3; i < tokens.Length; i++)
            {
                switch (tokens[i].Value.ToUpper())
                {
                    case "DIM":
                        variable._Dimentions = int.Parse(tokens[i].Block?[0].Value);
                        break;
                }
            }

            switch (tokens[0].Value.ToUpper())
            {
                case "DCL-S":
                    varType = StringToType(tokens[2].Value, tokens[2].Block?[0].Value);
                    varLength = tokens[2].Block?[0].Value;
                    break;
                case "DCL-F":
                    break;
                case "DCL-C":
                    break;
                case "DCL-DS":
                    break;
            }

            if (variable != null)
                if (CurrentProcudure != null)
                    CurrentProcudure.AddVariable(variable);
                else
                    _Module.AddVariable(variable);
        }

        private static Types StringToType(string Value, string length = "0")
        {
            switch (Value.ToUpper())
            {
                case "CHAR": return Types.Character;
                case "VARCHAR": return Types.Varying;
                case "INT":
                    switch (length)
                    {
                        case "3": return Types.Int8;
                        case "5": return Types.Int16;
                        case "10": return Types.Int32;
                        case "20": return Types.Int64;
                        default: return Types.Int32;
                    }
                case "FLOAT":
                    switch (length)
                    {
                        case "4": return Types.Float;
                        case "8": return Types.Double;
                        default: return Types.Double;
                    }
                case "ZONED": return Types.Double;
                case "PACKED": return Types.Double;
            }

            return Types.Void;
        }

        private void HandleOperation(RPGToken[] tokens)
        {
            if (CurrentProcudure == null)
                CurrentProcudure = new Procedure("entry");

            switch (tokens[0].Value.ToUpper())
            {
                case "DSPLY":
                    ParseExpression(tokens.Skip(1).ToArray());
                    CurrentProcudure.AddInstruction(Instructions.CALL, "DSPLY");
                    break;
                case "RETURN":
                    ParseExpression(tokens.Skip(1).ToArray());
                    CurrentProcudure.AddInstruction(Instructions.RETURN);
                    break;
            }
        }

        private void ParseExpression(RPGToken[] tokens)
        {
            if (tokens == null) return;
            if (tokens.Count() == 0) return;

            Types lastType = Types.Void;
            Instructions CurrentOperation = Instructions.NOP;

            foreach (RPGToken token in tokens)
            {
                switch (token.Type)
                {
                    case RPGLex.Type.PARMS:
                        CurrentOperation = Instructions.PARMS;
                        if (token.Block != null) ParseExpression(token.Block.ToArray());
                        continue;
                    case RPGLex.Type.EQUALS:
                        CurrentOperation = Instructions.EQUAL;
                        if (token.Block != null) ParseExpression(token.Block.ToArray());
                        continue;
                    case RPGLex.Type.ADD:
                        if (lastType == Types.Character || lastType == Types.Varying || lastType == Types.String)
                            CurrentOperation = Instructions.APPEND;
                        else
                            CurrentOperation = Instructions.ADD;

                        if (token.Block != null) ParseExpression(token.Block.ToArray());
                        continue;
                    case RPGLex.Type.SUB:
                        CurrentOperation = Instructions.SUB;
                        if (token.Block != null) ParseExpression(token.Block.ToArray());
                        continue;
                    case RPGLex.Type.DIV:
                        CurrentOperation = Instructions.DIV;
                        if (token.Block != null) ParseExpression(token.Block.ToArray());
                        continue;
                    case RPGLex.Type.MUL:
                        CurrentOperation = Instructions.MUL;
                        if (token.Block != null) ParseExpression(token.Block.ToArray());
                        continue;
                    case RPGLex.Type.DOT:
                        CurrentOperation = Instructions.PARMS;
                        if (token.Block != null) ParseExpression(token.Block.ToArray());
                        continue;

                    case RPGLex.Type.BIF:
                        ParseExpression(token.Block.ToArray());
                        CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                        break;
                    case RPGLex.Type.WORD_LITERAL:
                        if (token.Block != null)
                        {
                            //TODO: check if it's an array, else it's a procedure
                        }
                        else
                        {
                            //TODO: check if the variable exists, else crash
                            CurrentProcudure.AddInstruction(Instructions.LDVAR, token.Value);
                        }
                        break;
                    case RPGLex.Type.STRING_LITERAL:
                        CurrentProcudure.AddInstruction(Instructions.LDSTR, token.Value);
                        break;
                    case RPGLex.Type.SPECIAL:
                        //TODO: handle special
                        break;
                    case RPGLex.Type.INT_LITERAL:
                    case RPGLex.Type.DOUBLE_LITERAL:
                        CurrentProcudure.AddInstruction(Instructions.LDNUM, token.Value);
                        break;
                }

                if (CurrentOperation != Instructions.NOP)
                {
                    CurrentProcudure.AddInstruction(CurrentOperation);
                    CurrentOperation = Instructions.NOP;
                }
            }

            if (CurrentOperation != Instructions.NOP)
                CurrentProcudure.AddInstruction(CurrentOperation);
        }

        public Module GetModule() => _Module;
    }
}
