using System;
using System.Collections.Generic;
using System.Linq;
using NetRPG.Runtime;

namespace NetRPG.Language
{
    class Labels
    {
        private static List<String> _Labels = new List<String>();
        public static int Scope = 0;
        public static void Add(string Label)
        {
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
        private Dictionary<string, DataSet> Struct_Templates;
        private List<DataSet> Current_Structs;

        private Module _Module;
        private Procedure CurrentProcudure;

        public Reader()
        {
            _Module = new Module();
            CurrentProcudure = null;

            Struct_Templates = new Dictionary<string, DataSet>();
            Current_Structs = new List<DataSet>();

            SubfieldLevel = -1;
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
                    case RPGLex.Type.ENDDCL:
                        HandleEnd(tokens);
                        break;
                }
            }

            if (CurrentProcudure != null)
                _Module.AddProcedure(CurrentProcudure);
        }

        private int SubfieldLevel;
        private void HandleEnd(RPGToken[] tokens)
        {
            if (tokens[1].Type == RPGLex.Type.SUB)
            {
                switch (tokens[2].Value.ToUpper())
                {
                    case "DS":
                        Struct_Templates.Add(Current_Structs[SubfieldLevel]._Name, Current_Structs[SubfieldLevel]);
                        if (Current_Structs[SubfieldLevel]._Template == false) //if it's not a template, also define it
                        {
                            if (Current_Structs[SubfieldLevel]._Qualified == true)
                            {
                                if (CurrentProcudure != null)
                                    CurrentProcudure.AddDataSet(Current_Structs[SubfieldLevel]);
                                else
                                    _Module.AddDataSet(Current_Structs[SubfieldLevel]);
                            }
                            else
                            {
                                foreach (DataSet var in Current_Structs[SubfieldLevel]._Subfields)
                                    if (CurrentProcudure != null)
                                        CurrentProcudure.AddDataSet(var);
                                    else
                                        _Module.AddDataSet(var);
                            }
                        }

                        SubfieldLevel--;
                        Current_Structs.RemoveAt(Current_Structs.Count - 1);
                        break;
                }
            }
        }

        private void HandleDeclare(RPGToken[] tokens)
        {
            //TODO: Check if DataSet already exists?
            DataSet dataSet = new DataSet(tokens[3].Value);
            string length = "";
            Dictionary<string, string> config = new Dictionary<string, string>();

            for (int i = 3; i < tokens.Length; i++)
            {
                if (i + 1 < tokens.Length && tokens[i + 1].Type == RPGLex.Type.BLOCK)
                {
                    switch (tokens[i].Value.ToUpper())
                    {
                        case "DIM":
                            dataSet._Dimentions = int.Parse(tokens[i + 1].Block?[0].Value);
                            break;
                        case "INZ":
                            dataSet._InitialValue = tokens[i + 1].Block?[0].Value;
                            break;
                        case "LIKEDS":
                            dataSet._Subfields = Struct_Templates[tokens[i + 1].Block?[0].Value]._Subfields;
                            dataSet._Qualified = true;
                            break;
                    }
                    i++;
                }
                else
                {
                    switch (tokens[i].Value.ToUpper())
                    {
                        case "QUALIFIED":
                            dataSet._Qualified = true;
                            break;
                        case "TEMPLATE":
                            dataSet._Template = true;
                            break;
                    }
                }
            }

            if (tokens[1].Type == RPGLex.Type.SUB)
            {
                switch (tokens[2].Value.ToUpper())
                {
                    case "S":
                    case "SUBF":
                        dataSet._Precision = 0;
                        if (tokens.Count() >= 6)
                        {
                            length = tokens?[5].Block?[0].Value;

                            if (tokens[5]?.Block.Count >= 3)
                                dataSet._Precision = int.Parse(tokens[5]?.Block?[2].Value);

                            int.TryParse(tokens[5]?.Block?[0].Value, out dataSet._Length);
                        }

                        dataSet._Type = StringToType(tokens[4].Value, length);

                        break;
                    case "F":
                        break;
                    case "C":
                        break;
                    case "DS":
                        dataSet._Type = Types.Structure;

                        if (dataSet._Subfields == null)
                            dataSet._Subfields = new List<DataSet>();
                        break;

                    case "PROC":
                        if (CurrentProcudure != null)
                            _Module.AddProcedure(CurrentProcudure);

                        CurrentProcudure = new Procedure(tokens[3].Value);
                        dataSet = null;
                        break;
                    case "PI":
                        if (tokens.Count() >= 6)
                        {
                            length = tokens?[5].Block?[0].Value;
                            CurrentProcudure._ReturnType = StringToType(tokens[4].Value, length);
                        }
                        else
                            CurrentProcudure._ReturnType = Types.Void;

                        dataSet = null;
                        break;

                    case "PARM":
                        dataSet._Precision = 0;
                        if (tokens.Count() >= 6)
                        {
                            length = tokens?[5].Block?[0].Value;

                            if (tokens[5]?.Block.Count >= 3)
                                dataSet._Precision = int.Parse(tokens[5]?.Block?[2].Value);

                            int.TryParse(tokens[5]?.Block?[0].Value, out dataSet._Length);
                        }

                        dataSet._Type = StringToType(tokens[4].Value, length);
                        CurrentProcudure.AddParameter(tokens[3].Value);
                        break;
                }

                if (SubfieldLevel >= 0)
                {
                    Current_Structs[SubfieldLevel]._Subfields.Add(dataSet);
                }
                else if (dataSet != null && tokens[2].Value.ToUpper() == "DS" && dataSet._Type == Types.Structure)
                {
                    SubfieldLevel++;
                    Current_Structs.Add(dataSet);
                }
                else if (dataSet != null)
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
                case "IND": return Types.Ind;
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
                case "ZONED":
                case "PACKED":
                    return Types.FixedDecimal;

                case "LIKEDS":
                    return Types.Structure;
            }

            Error.ThrowCompileError("Type '" + Value + "' does not exist or is not supported.");
            return Types.Void;
        }

        private void HandleOperation(RPGToken[] tokens)
        {
            string forElse, start, end;
            if (CurrentProcudure == null)
            {
                CurrentProcudure = new Procedure("entry");
                CurrentProcudure.AddInstruction(Instructions.ENTRYPOINT);
            }

            switch (tokens[0].Value.ToUpper())
            {
                case "IF":
                    ParseExpression(tokens.Skip(1).ToList());

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
                    ParseExpression(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.BRFALSE, Labels.getScope());
                    Labels.Scope++;
                    break;
                case "ENDIF":
                    CurrentProcudure.AddInstruction(Instructions.LABEL, Labels.getLastScope());
                    break;

                case "SELECT":
                    Labels.Add(Labels.getScope());
                    Labels.Scope++;
                    break;
                case "WHEN":
                    forElse = Labels.getLastScope();
                    CurrentProcudure.AddInstruction(Instructions.LABEL, forElse);

                    Labels.Add(Labels.getScope());
                    ParseExpression(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.BRFALSE, Labels.getScope());
                    Labels.Scope++;
                    break;
                case "ENDSL":
                    CurrentProcudure.AddInstruction(Instructions.LABEL, Labels.getLastScope());
                    Labels.Scope++;
                    break;

                case "DOW":
                    CurrentProcudure.AddInstruction(Instructions.LABEL, Labels.getScope());
                    Labels.Add(Labels.getScope());
                    Labels.Scope++;

                    ParseExpression(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.BRFALSE, Labels.getScope());
                    Labels.Add(Labels.getScope());
                    Labels.Scope++;
                    break;
                case "ENDDO":
                    end = Labels.getLastScope();
                    start = Labels.getLastScope();
                    CurrentProcudure.AddInstruction(Instructions.BR, start);
                    CurrentProcudure.AddInstruction(Instructions.LABEL, end);
                    Labels.Scope++;
                    break;

                case "DSPLY":
                    ParseExpression(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.LDINT, "1");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "DSPLY");
                    break;
                case "RETURN":
                    ParseExpression(tokens.Skip(1).ToList());
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

            if (assignIndex == -1)
            {
                //Possibly a function call
                ParseExpression(tokens.ToList());
            }
            else
            {
                //Usually an assignment
                ParseAssignment(tokens.Take(assignIndex).ToList());
                ParseExpression(tokens.Skip(assignIndex + 1).ToList());
                CurrentProcudure.AddInstruction(Instructions.STORE);
            }
        }

        private void ParseAssignment(List<RPGToken> tokens)
        {
            RPGToken token;

            if (tokens == null) return;
            if (tokens.Count() == 0) return;
            for (int i = 0; i < tokens.Count(); i++)
            {
                token = tokens[i];
                switch (token.Type)
                {
                    case RPGLex.Type.BIF: //TODO: Subst assignment
                        if (tokens[i + 1].Block != null)
                        {
                            ParseExpression(tokens[i + 1].Block);
                            CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                            i++;
                        }
                        else
                        {
                            //TODO: What if no parameters?
                        }
                        break;

                    case RPGLex.Type.WORD_LITERAL:
                        if (i + 1 < tokens.Count() && tokens[i + 1].Block != null)
                        {
                            if (_Module.GetDataSetList().Contains(tokens[i].Value))
                            {
                                CurrentProcudure.AddInstruction(Instructions.LDGBLD, token.Value); //Load global
                                ParseExpression(tokens[i + 1].Block);

                                if (_Module.GetDataSet(token.Value)._Type == Types.Structure)
                                    CurrentProcudure.AddInstruction(Instructions.LDARRD);
                            }
                            else if (CurrentProcudure.GetDataSetList().Contains(tokens[i].Value))
                            {
                                CurrentProcudure.AddInstruction(Instructions.LDVARD, token.Value); //Load local
                                ParseExpression(tokens[i + 1].Block);

                                if (CurrentProcudure.GetDataSet(token.Value)._Type == Types.Structure)
                                    CurrentProcudure.AddInstruction(Instructions.LDARRD);
                            }
                            else
                            {
                                //Assuming is a field.
                                CurrentProcudure.AddInstruction(Instructions.LDFLDD, token.Value);
                                ParseExpression(tokens[i + 1].Block);
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
                        CurrentProcudure.AddInstruction(Instructions.LDINT, token.Value);
                        break;
                    case RPGLex.Type.DOUBLE_LITERAL:
                        CurrentProcudure.AddInstruction(Instructions.LDDOU, token.Value);
                        break;
                }
            }
        }

        private int ParseExpression(List<RPGToken> tokens)
        {
            bool ChangeMade = false;
            int ParmCount = 1;
            int AppendCount = 0;
            RPGToken token = null;
            if (tokens == null) return 0;
            if (tokens.Count() == 0) return 0;

            Types lastType = Types.Void;
            Statement[] Parameters = null;
            List<Instructions> Append = new List<Instructions>();

            for (int i = 0; i < tokens.Count; i++)
            {
                ChangeMade = false;
                switch (tokens[i].Type)
                {
                    case RPGLex.Type.INT_LITERAL:
                        if (i + 2 < tokens.Count)
                        {
                            if (tokens[i + 1].Type == RPGLex.Type.DOT)
                            {
                                if (tokens[i + 2].Type == RPGLex.Type.INT_LITERAL)
                                {
                                    token = new RPGToken(RPGLex.Type.DOUBLE_LITERAL, tokens[i].Value + "." + tokens[i + 2].Value, tokens[i].Line);
                                    ChangeMade = true;
                                }
                            }
                        }

                        if (ChangeMade)
                        {
                            tokens.RemoveRange(i, 3);
                            tokens.Insert(i, token);
                        }
                        break;

                    case RPGLex.Type.MUL:
                        if (i + 1 < tokens.Count)
                        {
                            if (tokens[i + 1].Type == RPGLex.Type.WORD_LITERAL)
                            {
                                token = new RPGToken(RPGLex.Type.SPECIAL, "*" + tokens[i + 1].Value, tokens[i].Line);
                                ChangeMade = true;
                            }
                        }

                        if (ChangeMade)
                        {
                            tokens.RemoveRange(i, 2);
                            tokens.Insert(i, token);
                        }
                        break;
                }
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                token = tokens[i];
                switch (token.Type)
                {
                    case RPGLex.Type.PARMS:
                        ParmCount++;
                        for (int x = Append.Count - 1; x >= 0; x--)
                            CurrentProcudure.AddInstruction(Append[x]);
                        Append.Clear();
                        continue;
                    case RPGLex.Type.EQUALS:
                        Append.Add(Instructions.EQUAL);
                        continue;
                    case RPGLex.Type.ADD:
                        if (lastType == Types.Character || lastType == Types.Varying || lastType == Types.String)
                            Append.Add(Instructions.APPEND);
                        else
                            Append.Add(Instructions.ADD);
                        continue;
                    case RPGLex.Type.SUB:
                        Append.Add(Instructions.SUB);
                        continue;
                    case RPGLex.Type.DIV:
                        Append.Add(Instructions.DIV);
                        continue;
                    case RPGLex.Type.MUL:
                        Append.Add(Instructions.MUL);
                        continue;
                    case RPGLex.Type.DOT:
                        continue;
                    case RPGLex.Type.LESS_THAN:
                        Append.Add(Instructions.LESSER);
                        continue;
                    case RPGLex.Type.MORE_THAN:
                        Append.Add(Instructions.GREATER);
                        continue;
                    case RPGLex.Type.LT_EQUAL:
                        Append.Add(Instructions.LESSER_EQUAL);
                        continue;
                    case RPGLex.Type.MT_EQUAL:
                        Append.Add(Instructions.GREATER_EQUAL);
                        continue;
                    case RPGLex.Type.NOT:
                        Append.Add(Instructions.NOT_EQUAL);
                        continue;

                    case RPGLex.Type.BIF:
                        if (tokens[i + 1].Block != null)
                        {
                            AppendCount = ParseExpression(tokens[i + 1].Block);
                            CurrentProcudure.AddInstruction(Instructions.LDINT, AppendCount.ToString());
                            CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                            i++;
                        }
                        else
                        {
                            //TODO: What if no parameters?
                        }
                        break;
                    case RPGLex.Type.BLOCK:
                        ParseExpression(token.Block);
                        break;

                    case RPGLex.Type.WORD_LITERAL:
                        if (i + 1 < tokens.Count && tokens[i + 1].Block != null)
                        {
                            if (_Module.GetDataSetList().Contains(tokens[i].Value))
                            {
                                lastType = _Module.GetDataSet(tokens[i].Value)._Type;

                                CurrentProcudure.AddInstruction(Instructions.LDGBLV, token.Value); //Load global
                                ParseExpression(tokens[i + 1].Block);
                                CurrentProcudure.AddInstruction(Instructions.LDARRV);
                            }
                            else if (CurrentProcudure.GetDataSetList().Contains(tokens[i].Value))
                            {
                                lastType = CurrentProcudure.GetDataSet(tokens[i].Value)._Type;

                                CurrentProcudure.AddInstruction(Instructions.LDVARV, token.Value); //Load local
                                ParseExpression(tokens[i + 1].Block);
                                CurrentProcudure.AddInstruction(Instructions.LDARRV);
                            }
                            else if (lastType == Types.Structure) 
                            {
                                CurrentProcudure.AddInstruction(Instructions.LDFLDV, token.Value); //Load local
                                ParseExpression(tokens[i + 1].Block);
                                CurrentProcudure.AddInstruction(Instructions.LDARRV);
                            }
                            else
                            {
                                if (Runtime.Functions.Function.IsFunction(token.Value))
                                    AppendCount = ParseExpression(tokens[i + 1].Block);
                                else
                                {
                                    //Always pass by ref, convert to value if needed at runtime
                                    //TODO: determine if need to pass by value (because expressions in the param)
                                    Parameters = Statement.ParseParams(tokens[i + 1].Block);
                                    foreach (Statement parameter in Parameters)
                                        ParseAssignment(parameter.GetTokens().ToList());
                                    AppendCount = Parameters.Length;
                                }
                                CurrentProcudure.AddInstruction(Instructions.LDINT, AppendCount.ToString());
                                CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                            }

                            i++;
                        }
                        else
                        {
                            if (_Module.GetDataSetList().Contains(tokens[i].Value))
                            {
                                lastType = _Module.GetDataSet(tokens[i].Value)._Type;
                                if (lastType == Types.Structure)
                                    CurrentProcudure.AddInstruction(Instructions.LDGBLD, token.Value); //Load global data
                                else
                                    CurrentProcudure.AddInstruction(Instructions.LDGBLV, token.Value); //Load global value
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
                        switch (token.Value.ToUpper())
                        {
                            case "*BLANK":
                            case "*BLANKS":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "");
                                break;
                            case "*ZERO":
                            case "*ZEROS":
                                CurrentProcudure.AddInstruction(Instructions.LDINT, "0");
                                break;
                            case "*ON":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "1");
                                break;
                            case "*OFF":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "0");
                                break;
                            case "*NULL":
                                CurrentProcudure.AddInstruction(Instructions.LDNULL);
                                break;
                        }
                        break;
                    case RPGLex.Type.INT_LITERAL:
                        CurrentProcudure.AddInstruction(Instructions.LDINT, token.Value);
                        break;
                    case RPGLex.Type.DOUBLE_LITERAL:
                        CurrentProcudure.AddInstruction(Instructions.LDDOU, token.Value);
                        break;

                    case RPGLex.Type.OPERATION:
                    case RPGLex.Type.DCL:
                        Error.ThrowCompileError(token.Type + " not expected. Check for missing semi-colon.", token.Line);
                        break;
                }

            }

            for (int x = Append.Count - 1; x >= 0; x--)
                CurrentProcudure.AddInstruction(Append[x]);
            Append.Clear();

            return ParmCount;
        }

        public Module GetModule() => _Module;
    }
}
