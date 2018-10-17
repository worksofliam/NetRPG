using System;
using System.Collections.Generic;
using System.Linq;
using NetRPG.Runtime;

namespace NetRPG.Language
{
    class Labels {
        private static List<String> _Labels = new List<String>();
        public static int Scope = 0;
        public static void Add(string Label) {
            _Labels.Add(Label);
        }
        public static String getScope()
        {
            return "SCOPE" + Scope.ToString();
        }
        public static String getLastScope()
        {
            String Out = _Labels[_Labels.Count - 1];
            _Labels.RemoveAt(_Labels.Count - 1);
            return Out;
        }
    }

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
                    case RPGLex.Type.WORD_LITERAL:
                        HandleAssignment(tokens);
                        break;
                }
            }

            if (CurrentProcudure != null)
                _Module.AddProcedure(CurrentProcudure);
        }

        private void HandleDeclare(RPGToken[] tokens)
        {
            //TODO: Check if DataSet already exists?
            DataSet dataSet = new DataSet(tokens[3].Value);
            Dictionary<string, string> config = new Dictionary<string, string>();
            
            for (int i = 3; i < tokens.Length; i++)
            {
                if (tokens[i+1].Type == RPGLex.Type.BLOCK) {
                    switch (tokens[i].Value.ToUpper())
                    {
                        case "DIM":
                            dataSet._Dimentions = int.Parse(tokens[i+1].Block?[0].Value);
                            break;
                        case "INZ":
                            dataSet._InitialValue = tokens[i+1].Block?[0].Value;
                            break;
                    }
                    i++;
                } else {
                    
                }
            }

            if (tokens[1].Type == RPGLex.Type.SUB) {
                switch (tokens[2].Value.ToUpper())
                {
                    case "S":
                        dataSet._Type = StringToType(tokens[4].Value, tokens[5]?.Block?[0].Value);
                        int.TryParse(tokens[5]?.Block?[0].Value, out dataSet._Length);
                        break;
                    case "F":
                        break;
                    case "C":
                        break;
                    case "DS":
                        break;
                }

                if (dataSet != null)
                    if (CurrentProcudure != null)
                        CurrentProcudure.AddDataSet(dataSet);
                    else
                        _Module.AddDataSet(dataSet);
            }
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
            string forElse;
            if (CurrentProcudure == null)
            {
                CurrentProcudure = new Procedure("entry");
                CurrentProcudure.AddInstruction(Instructions.ENTRYPOINT);
            }

            switch (tokens[0].Value.ToUpper())
            {
                case "IF":
                    ParseExpression(tokens.Skip(1).ToArray());

                    Labels.Add(Labels.getScope());
                    CurrentProcudure.AddInstruction(Instructions.BRFALSE, Labels.getScope());
                    Labels.Scope++;
                    break;
                
                case "ELSE":
                    forElse = Labels.getLastScope();
                    Labels.Add(Labels.getScope());
                    CurrentProcudure.AddInstruction(Instructions.BR, Labels.getScope());
                    Labels.Scope++;

                    CurrentProcudure.AddInstruction(Instructions.LABEL, forElse);
                    break;

                case "ELSEIF":
                    forElse = Labels.getLastScope();
                    Labels.Add(Labels.getScope());

                    CurrentProcudure.AddInstruction(Instructions.LABEL, forElse);
                    ParseExpression(tokens.Skip(1).ToArray());
                    CurrentProcudure.AddInstruction(Instructions.BRFALSE, Labels.getScope());
                    Labels.Scope++;
                    break;
                case "ENDIF":
                    CurrentProcudure.AddInstruction(Instructions.LABEL, Labels.getLastScope());
                    break;

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

        private void HandleAssignment(RPGToken[] tokens)
        {
            if (CurrentProcudure == null)
            {
                CurrentProcudure = new Procedure("entry");
                CurrentProcudure.AddInstruction(Instructions.ENTRYPOINT);
            }

            if (tokens == null) return;
            if (tokens.Count() == 0) return;

            int assignIndex = -1;

            for (var i = 0; i < tokens.Count(); i++)
            {
                if (tokens[i].Type == RPGLex.Type.EQUALS)
                {
                    assignIndex = i;
                    break;
                }
            }

            ParseAssignment(tokens.SkipLast(assignIndex).ToArray());
            ParseExpression(tokens.Skip(assignIndex + 1).ToArray());
            CurrentProcudure.AddInstruction(Instructions.STORE);
            //TODO: figure out how we're storing data, lol
        }

        private void ParseAssignment(RPGToken[] tokens)
        {
            RPGToken token;

            if (tokens == null) return;
            if (tokens.Count() == 0) return;
            for (int i = 0; i < tokens.Length; i++)
            {
                token = tokens[i];
                switch (token.Type)
                {
                    case RPGLex.Type.BIF: //TODO: Subst assignment
                        if (tokens[i + 1].Block != null)
                        {
                            ParseExpression(tokens[i + 1].Block.ToArray());
                            CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                            i++;
                        }
                        else
                        {
                            //TODO: What if no parameters?
                        }
                        break;

                    case RPGLex.Type.WORD_LITERAL:
                        if (i + 1 < tokens.Length && tokens[i + 1].Block != null)
                        {
                            //DONE: check if it's an array, else it's a procedure
                            if (_Module.GetDataSetList().Contains(tokens[i].Value))
                            {
                                CurrentProcudure.AddInstruction(Instructions.LDGBLD, token.Value); //Load global
                                ParseExpression(tokens[i + 1].Block.ToArray());
                            }
                            else if (CurrentProcudure.GetDataSetList().Contains(tokens[i].Value))
                            {
                                CurrentProcudure.AddInstruction(Instructions.LDVARD, token.Value); //Load local
                                ParseExpression(tokens[i + 1].Block.ToArray());
                            }
                            else
                            {
                                //TODO: IS FIELD?
                                ParseExpression(tokens[i + 1].Block.ToArray());
                                CurrentProcudure.AddInstruction(Instructions.LDFLDD, token.Value);
                            }

                            i++;
                        }
                        else
                        {
                            if (_Module.GetDataSetList().Contains(tokens[i].Value))
                                CurrentProcudure.AddInstruction(Instructions.LDGBLD, token.Value); //Load global
                            else if (CurrentProcudure.GetDataSetList().Contains(tokens[i].Value))
                                CurrentProcudure.AddInstruction(Instructions.LDVARD, token.Value); //Load local
                            else
                                CurrentProcudure.AddInstruction(Instructions.LDFLDD, token.Value); //Load field?
                            
                        }
                        break;
                    case RPGLex.Type.STRING_LITERAL:
                        CurrentProcudure.AddInstruction(Instructions.LDSTR, token.Value);
                        break;
                    case RPGLex.Type.INT_LITERAL:
                    case RPGLex.Type.DOUBLE_LITERAL:
                        CurrentProcudure.AddInstruction(Instructions.LDNUM, token.Value);
                        break;
                }
            }
        }

        private void ParseExpression(RPGToken[] tokens)
        {
            RPGToken token;
            if (tokens == null) return;
            if (tokens.Count() == 0) return;

            Types lastType = Types.Void;
            Instructions CurrentOperation = Instructions.NOP;

            for (int i = 0; i < tokens.Length; i++)
            {
                token = tokens[i];
                switch (token.Type)
                {
                    case RPGLex.Type.PARMS:
                        CurrentOperation = Instructions.NOP;
                        continue;
                    case RPGLex.Type.EQUALS:
                        CurrentOperation = Instructions.EQUAL;
                        continue;
                    case RPGLex.Type.ADD:
                        if (lastType == Types.Character || lastType == Types.Varying || lastType == Types.String)
                            CurrentOperation = Instructions.APPEND;
                        else
                            CurrentOperation = Instructions.ADD;
                        continue;
                    case RPGLex.Type.SUB:
                        CurrentOperation = Instructions.SUB;
                        continue;
                    case RPGLex.Type.DIV:
                        CurrentOperation = Instructions.DIV;
                        continue;
                    case RPGLex.Type.MUL:
                        CurrentOperation = Instructions.MUL;
                        continue;
                    case RPGLex.Type.DOT:
                        CurrentOperation = Instructions.LDFLDV;
                        continue;
                    case RPGLex.Type.LESS_THAN:
                        CurrentOperation = Instructions.LESSER;
                        continue;
                    case RPGLex.Type.MORE_THAN:
                        CurrentOperation = Instructions.GREATER;
                        continue;
                    case RPGLex.Type.LT_EQUAL:
                        CurrentOperation = Instructions.LESSER_EQUAL;
                        continue;
                    case RPGLex.Type.MT_EQUAL:
                        CurrentOperation = Instructions.GREATER_EQUAL;
                        continue;
                    case RPGLex.Type.NOT:
                        CurrentOperation = Instructions.NOT_EQUAL;
                        continue;

                    case RPGLex.Type.BIF:
                        if (tokens[i+1].Block != null) { 
                            ParseExpression(tokens[i+1].Block.ToArray());
                            CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                            i++;
                        } else {
                            //TODO: What if no parameters?
                        }
                        break;
                    case RPGLex.Type.BLOCK:
                        ParseExpression(token.Block.ToArray());
                        break;

                    case RPGLex.Type.WORD_LITERAL:
                        if (i + 1 < tokens.Length && tokens[i+1].Block != null)
                        {
                            if (_Module.GetDataSetList().Contains(tokens[i].Value))
                            {
                                lastType = _Module.GetDataSet(tokens[i].Value)._Type;

                                CurrentProcudure.AddInstruction(Instructions.LDGBLV, token.Value); //Load global
                                ParseExpression(tokens[i + 1].Block.ToArray());
                                CurrentProcudure.AddInstruction(Instructions.LDARRV);
                            }
                            else if (CurrentProcudure.GetDataSetList().Contains(tokens[i].Value))
                            {
                                lastType = CurrentProcudure.GetDataSet(tokens[i].Value)._Type;

                                CurrentProcudure.AddInstruction(Instructions.LDVARV, token.Value); //Load local
                                ParseExpression(tokens[i + 1].Block.ToArray());
                                CurrentProcudure.AddInstruction(Instructions.LDARRV);
                            }
                            else
                            {
                                //TODO: Maybe check the procedure exists? Could be an array within a struct
                                ParseExpression(tokens[i + 1].Block.ToArray());
                                CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                            }
                            
                            i++;
                        }
                        else
                        {
                            if (_Module.GetDataSetList().Contains(tokens[i].Value))
                            {
                                lastType = _Module.GetDataSet(tokens[i].Value)._Type;
                                CurrentProcudure.AddInstruction(Instructions.LDGBLV, token.Value); //Load global
                            }
                            else if (CurrentProcudure.GetDataSetList().Contains(tokens[i].Value))
                            {
                                lastType = CurrentProcudure.GetDataSet(tokens[i].Value)._Type;
                                CurrentProcudure.AddInstruction(Instructions.LDVARV, token.Value); //Load local
                            }
                            else
                            {
                                CurrentProcudure.AddInstruction(Instructions.LDFLDV, token.Value); //Load field?
                            }
                        }
                        break;
                    case RPGLex.Type.STRING_LITERAL:
                        lastType = Types.String;
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
