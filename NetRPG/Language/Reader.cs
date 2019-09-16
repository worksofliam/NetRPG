using System;
using System.Collections.Generic;
using System.Linq;
using NetRPG.Runtime;

using System.Globalization;
using System.Threading;
using System.Text;

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

    public enum LOCATION {
        Local,
        Global
    }

    class CompileTimeSubfield {
        public LOCATION Location;
        public string Structure;

        public CompileTimeSubfield(LOCATION loc, string parent) {
            this.Location = loc;
            this.Structure = parent;
        }
    }

    class Reader
    {
        private Dictionary<string, DataSet> Struct_Templates;
        private List<DataSet> Current_Structs;

        private Module _Module;
        private Procedure CurrentProcudure;

        private Dictionary<string, CompileTimeSubfield> GlobalSubfields;

        private Dictionary<string, string> RecordFormatDisplays;

        private int SelectCount;

        private string DateFormat = "MM/dd/yy";

        public Reader()
        {
            _Module = new Module();
            CurrentProcudure = null;

            Struct_Templates = new Dictionary<string, DataSet>();
            Current_Structs = new List<DataSet>();
            GlobalSubfields = new Dictionary<string, CompileTimeSubfield>();
            RecordFormatDisplays = new Dictionary<string, string>();

            SubfieldLevel = -1;

            string indName;
            DataSet inds = new DataSet("IND"){_Type = Types.Structure, _Qualified = false};
            inds._Subfields = new List<DataSet>();
            for (int i = 1; i <= 99; i++) {
                indName = "IN" + i.ToString().PadLeft(2, '0');
                inds._Subfields.Add(new DataSet(indName){_Type = Types.Ind, _InitialValue = "0"});
                GlobalSubfields.Add(indName, new CompileTimeSubfield(LOCATION.Global, "IND"));
            }

            indName = "INLR";
            inds._Subfields.Add(new DataSet(indName){_Type = Types.Ind, _InitialValue = "0"});
            GlobalSubfields.Add(indName, new CompileTimeSubfield(LOCATION.Global, "IND"));

            _Module.AddDataSet(inds);

            SelectCount = 0;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Configurations = new Dictionary<string, string>();
            Configurations.Add("DATFMT", "en-UK");
            Configurations.Add("CCSID", "IBM285");
        }

        private Dictionary<string, string> Configurations;

        private bool isPR;
        public void ReadStatements(Statement[] Statements)
        {
            RPGToken[] tokens;

            //TODO need to figure out how to put this into the DATFMT control opt
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Configurations["DATFMT"]);

            foreach (Statement statement in Statements)
            {
                CorrectTokens(statement._Tokens);
                tokens = statement.GetTokens();
                switch (tokens[0].Type)
                {
                    case RPGLex.Type.CTL:
                        HandleControlOptions(tokens);
                        break;
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
                            if (CurrentProcudure != null)
                                CurrentProcudure.AddDataSet(Current_Structs[SubfieldLevel]);
                            else
                                _Module.AddDataSet(Current_Structs[SubfieldLevel]);

                            if (Current_Structs[SubfieldLevel]._Qualified == false)
                            {
                                foreach (DataSet var in Current_Structs[SubfieldLevel]._Subfields) {
                                    if (!GlobalSubfields.ContainsKey(var._Name))
                                        if (CurrentProcudure != null)
                                            GlobalSubfields.Add(var._Name, new CompileTimeSubfield(LOCATION.Local, Current_Structs[SubfieldLevel]._Name));
                                        else
                                            GlobalSubfields.Add(var._Name, new CompileTimeSubfield(LOCATION.Global, Current_Structs[SubfieldLevel]._Name));
                                }
                            }
                        }

                        SubfieldLevel--;
                        Current_Structs.RemoveAt(Current_Structs.Count - 1);
                        break;
                }
            }
        }

        private void HandleControlOptions(RPGToken[] tokens) {
            for (int i = 3; i < tokens.Length; i++)
            {
                if (i + 1 < tokens.Length && tokens[i + 1].Type == RPGLex.Type.BLOCK)
                {
                    switch (tokens[i].Value.ToUpper())
                    {
                        case "DATFMT":
                            //TODO: Handle RPG DATFMT special values
                            Configurations["DATFMT"] = tokens[i + 1].Block?[0].Value;
                            break;
                        case "CCSID": //TODO: handle CCSID numbers instead of .net encoding types
                            if (tokens[i + 1].Block?[0].Type == RPGLex.Type.INT_LITERAL) {
                                Configurations["CCSID"] = tokens[i + 1].Block?[0].Value;
                            } else {
                                Error.ThrowCompileError("CCSID provided must be of type integer.");
                            }
                            break;
                    }
                    i++;
                }
            }
        }

        private void HandleDeclare(RPGToken[] tokens)
        {
            //TODO: Check if DataSet already exists?
            DataSet dataSet = new DataSet(tokens[3].Value);
            DataSet[] structures;
            LOCATION currentLocation;
            string length = "";

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
                        case "USROPN":
                            dataSet._UserOpen = true;
                            break;
                        case "EXTPGM":
                        case "EXTPROC":
                            dataSet._File = tokens[i + 1].Block?[0].Value;
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
                        case "DTAARA":
                            dataSet._DataArea = dataSet._Name;
                            break;
                        case "WORKSTN":
                            _Module._HasDisplay = true;
                            dataSet._WorkStation = true;
                            break;
                        case "CONST":
                        case "VALUE":
                            dataSet._IsConstOrValue = true;
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
                            if (tokens[5].Block != null) {
                                length = tokens?[5].Block?[0].Value;

                                if (tokens[5]?.Block.Count >= 3)
                                    dataSet._Precision = int.Parse(tokens[5]?.Block?[2].Value);

                                int.TryParse(tokens[5]?.Block?[0].Value, out dataSet._Length);
                            }
                        }

                        dataSet._Type = StringToType(tokens[4].Value, length);

                        break;
                    case "F":
                        dataSet._Type = Types.File;
                        if (dataSet._File == null)
                            dataSet._File = dataSet._Name;

                        //TODO, major clean up, remove code dupe

                        dataSet._Name += "_table"; //We do this so the DS can use the name instead
                        if (dataSet._WorkStation) {
                            structures = Runtime.Typing.Files.Display.CreateStructs(dataSet._File, dataSet._Qualified);
                        } else {
                            structures = new [] {Runtime.Typing.Files.Table.CreateStruct(dataSet._File, dataSet._Qualified)};
                        }

                        foreach (DataSet structure in structures) {
                            if (dataSet._WorkStation) {
                                //this is used to map the record to a file when we are compiling the application
                                RecordFormatDisplays.Add(structure._Name, dataSet._Name);
                            }

                            if (CurrentProcudure != null) {
                                CurrentProcudure.AddDataSet(structure);
                                currentLocation = LOCATION.Local;
                            } else {
                                _Module.AddDataSet(structure);
                                currentLocation = LOCATION.Global;
                            }

                            if (dataSet._Qualified == false) {
                                foreach(DataSet subfield in structure._Subfields) {
                                    if (!GlobalSubfields.ContainsKey(subfield._Name))
                                        GlobalSubfields.Add(subfield._Name, new CompileTimeSubfield(currentLocation, structure._Name));
                                }
                            }
                        }

                        break;
                    case "C":
                        Error.ThrowCompileError("Constants not supported yet.", tokens[2].Line);
                        break;
                    case "DS":
                        dataSet._Type = Types.Structure;

                        if (dataSet._Subfields == null)
                            dataSet._Subfields = new List<DataSet>();
                        break;

                    case "PROC":
                        isPR = false;
                        if (CurrentProcudure != null)
                            _Module.AddProcedure(CurrentProcudure);

                        CurrentProcudure = new Procedure(tokens[3].Value);
                        dataSet = null;
                        break;
                    case "PI":
                        isPR = false;
                        if (tokens.Count() >= 6)
                        {
                            length = tokens?[5].Block?[0].Value;
                            CurrentProcudure._ReturnType = StringToType(tokens[4].Value, length);
                        }
                        else
                            CurrentProcudure._ReturnType = Types.Void;

                        dataSet = null;
                        break;
                    case "PR":
                        isPR = true;
                        
                        _Module.AddFunctionRef(dataSet._Name, dataSet._File);

                        dataSet = null;
                        break;

                    case "PARM":
                        if (isPR) {
                            dataSet = null;
                            break;
                        }

                        dataSet._Precision = 0;
                        if (tokens.Count() >= 6)
                        {

                            if (tokens[5]?.Block != null) {
                                length = tokens?[5].Block?[0].Value;
                                if (tokens[5]?.Block.Count >= 3)
                                    dataSet._Precision = int.Parse(tokens[5]?.Block?[2].Value);

                                int.TryParse(tokens[5]?.Block?[0].Value, out dataSet._Length);
                            } else {
                                length = "0";
                            }
                        }

                        dataSet._Type = StringToType(tokens[4].Value, length);
                        CurrentProcudure.AddParameter(tokens[3].Value, dataSet._IsConstOrValue);
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

        public static Types StringToType(string Value, string length = "0")
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
                case "TIMESTAMP":
                case "DATE":
                case "TIME":
                    return Types.Timestamp;
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

                case "POINTER":
                    return Types.Pointer;

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

                    this.SelectCount += 1;
                    if (!CurrentProcudure.ContainsDataSet("SELECT" + this.SelectCount.ToString()))
                        CurrentProcudure.AddDataSet(new DataSet("SELECT" + this.SelectCount.ToString()){_Type = Types.Ind, _InitialValue = "1"});
                    break;
                case "WHEN":
                    forElse = Labels.getLastScope();
                    CurrentProcudure.AddInstruction(Instructions.LABEL, forElse);

                    Labels.Add(Labels.getScope());
                    ParseExpression(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.BRFALSE, Labels.getScope());

                    CurrentProcudure.AddInstruction(Instructions.LDVARD, "SELECT" + this.SelectCount.ToString());
                    CurrentProcudure.AddInstruction(Instructions.LDSTR, "0"); //*off
                    CurrentProcudure.AddInstruction(Instructions.STORE);

                    Labels.Scope++;
                    break;
                case "OTHER":
                    forElse = Labels.getLastScope();
                    CurrentProcudure.AddInstruction(Instructions.LABEL, forElse);

                    Labels.Add(Labels.getScope());
                    CurrentProcudure.AddInstruction(Instructions.LDVARV, "SELECT" + this.SelectCount.ToString()); //*should be star on
                    CurrentProcudure.AddInstruction(Instructions.BRFALSE, Labels.getScope());
                    Labels.Scope++;
                    break;

                case "ENDSL":
                    CurrentProcudure.AddInstruction(Instructions.LABEL, Labels.getLastScope());
                    Labels.Scope++;

                    this.SelectCount -= 1;
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
                    _Module.AddFunctionRef("DSPLY", "DSPLY");
                    break;
                case "RETURN":
                    ParseExpression(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.RETURN);
                    break;

                case "EVAL":
                    HandleAssignment(tokens.Skip(1).ToArray());
                    break;

                case "RESET":
                    ParseAssignment(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.LDINT, "1");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "RESET");
                    _Module.AddFunctionRef("RESET", "RESET");
                    break;

                case "CLEAR":
                    ParseAssignment(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.LDINT, "1");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "CLEAR");
                    _Module.AddFunctionRef("CLEAR", "CLEAR");
                    break;

                case "IN": //No support for *LOCK
                    //We load the variable twice so we can store it later
                    ParseAssignment(tokens.Skip(1).ToList());
                    ParseAssignment(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.LDINT, "1");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "IN");
                    _Module.AddFunctionRef("IN", "IN");

                    //Then store result of in function
                    CurrentProcudure.AddInstruction(Instructions.STORE);
                    break;

                case "OPEN":
                    ParseAssignment(tokens.Skip(1).ToList());
                    CurrentProcudure.AddInstruction(Instructions.LDINT, "1");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "OPEN");
                    _Module.AddFunctionRef("OPEN", "OPEN");
                    break;

                case "READ":
                    ParseAssignment(tokens.Skip(1).ToList()); //Load the DS first

                    //Then load the table
                    tokens[1].Value += "_table";
                    ParseAssignment(tokens.Skip(1).ToList());

                    CurrentProcudure.AddInstruction(Instructions.LDINT, "2");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "READ");
                    _Module.AddFunctionRef("READ", "READ");
                    break;

                case "READP":
                    ParseAssignment(tokens.Skip(1).ToList()); //Load the DS first

                    //Then load the table
                    tokens[1].Value += "_table";
                    ParseAssignment(tokens.Skip(1).ToList());

                    CurrentProcudure.AddInstruction(Instructions.LDINT, "2");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "READP");
                    _Module.AddFunctionRef("READP", "READP");
                    break;

                case "CHAIN": 
                    //CHAIN (keys) FILLE
                    //CHAIN key FILE

                    ParseAssignment(tokens.Skip(2).ToList()); //Load the DS first

                    //Then load the table
                    tokens[2].Value += "_table";
                    ParseAssignment(tokens.Skip(2).ToList());

                    if (tokens[1].Block != null) {
                        CurrentProcudure.AddInstruction(Instructions.LDINT, ParseExpression(tokens[1].Block).ToString());
                        CurrentProcudure.AddInstruction(Instructions.CRTARR);
                    } else {
                        ParseExpression(new List<RPGToken>() {tokens[1]});
                        CurrentProcudure.AddInstruction(Instructions.LDINT, "1");
                        CurrentProcudure.AddInstruction(Instructions.CRTARR);
                    }

                    CurrentProcudure.AddInstruction(Instructions.LDINT, "3");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "CHAIN");
                    _Module.AddFunctionRef("CHAIN", "CHAIN");
                    break;

                case "EXFMT":
                    //EXFMT RCDFMT -> //EXFMT TABLE STRUCTURE IND
                    ParseAssignment(tokens.Skip(1).ToList()); //Load the DS first

                    //Then load the table
                    tokens[1].Value = RecordFormatDisplays[tokens[1].Value];
                    ParseAssignment(tokens.Skip(1).ToList());
                    
                    //We also need the indicators for execute format
                    tokens[1].Value = "IND";
                    ParseAssignment(tokens.Skip(1).ToList());
                    
                    CurrentProcudure.AddInstruction(Instructions.LDINT, "3");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "EXFMT");
                    _Module.AddFunctionRef("EXFMT", "EXFMT");
                    break;

                case "WRITE":
                    //EXFMT RCDFMT -> //EXFMT TABLE STRUCTURE
                    ParseAssignment(tokens.Skip(1).ToList()); //Load the DS first

                    //Then load the table
                    if (RecordFormatDisplays.ContainsKey(tokens[1].Value)) {
                        tokens[1].Value = RecordFormatDisplays[tokens[1].Value];
                    } else {
                        tokens[1].Value += "_table";
                    }

                    ParseAssignment(tokens.Skip(1).ToList());
                    
                    CurrentProcudure.AddInstruction(Instructions.LDINT, "2");
                    CurrentProcudure.AddInstruction(Instructions.CALL, "WRITE");
                    _Module.AddFunctionRef("WRITE", "WRITE");
                    break;

                default:
                    Error.ThrowCompileError(tokens[0].Value + " operation does not exist.", tokens[0].Line);
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

            //We do this incase we are assigning to a DS
            for (var i = 0; i < tokens.Count(); i++)
            {
                if (tokens[i].Type == RPGLex.Type.EQUALS || tokens[i].Type == RPGLex.Type.ASSIGNMENT)
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
                ParseAssignment(tokens.Take(assignIndex).ToList()); //Load the assigning to variable

                if (tokens[assignIndex].Type == RPGLex.Type.ASSIGNMENT) { //If it's a weird assignment, e.g. not EQUAL
                    ParseExpression(tokens.Take(assignIndex).ToList()); //Load the assigning to variable for expression
                }

                ParseExpression(tokens.Skip(assignIndex + 1).ToList()); //Load the expression
                if (tokens[assignIndex].Type == RPGLex.Type.ASSIGNMENT) { //If it's a weird assignment, e.g. not EQUAL
                    switch (tokens[assignIndex].Value) {
                        case "+=": 
                            CurrentProcudure.AddInstruction(Instructions.ADD); //Store it
                            break;
                        case "-=": 
                            CurrentProcudure.AddInstruction(Instructions.SUB); //Store it
                            break;
                        case "/=": 
                            CurrentProcudure.AddInstruction(Instructions.DIV); //Store it
                            break;
                        case "*=": 
                            CurrentProcudure.AddInstruction(Instructions.MUL); //Store it
                            break;
                        case "**=": 
                            Error.ThrowCompileError("Power to assignment not yet implemented.", tokens[assignIndex].Line);
                            break;
                    }
                }

                CurrentProcudure.AddInstruction(Instructions.STORE); //Store it
            }
        }

        private static string[] PoinerFunctions = new []{"ELEM"};
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
                            _Module.AddFunctionRef(token.Value, token.Value);
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
                            else {
                                if (GlobalSubfields.ContainsKey(token.Value)) {
                                    switch (GlobalSubfields[token.Value].Location) {
                                        case LOCATION.Global:
                                            CurrentProcudure.AddInstruction(Instructions.LDGBLD, GlobalSubfields[token.Value].Structure); //Load global data
                                            break;
                                        case LOCATION.Local:
                                            CurrentProcudure.AddInstruction(Instructions.LDVARD, GlobalSubfields[token.Value].Structure); //Load local data
                                            break;
                                    }
                                }
                                CurrentProcudure.AddInstruction(Instructions.LDFLDD, token.Value); //Load field?
                            }

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
                            switch (token.Value.ToUpper()) {
                                case "%EOF":
                                case "%FOUND":
                                    tokens[i + 1].Block[0].Value += "_table";
                                    if (_Module.GetDataSetList().Contains(tokens[i + 1].Block[0].Value))
                                    {
                                        CurrentProcudure.AddInstruction(Instructions.LDGBLD, tokens[i + 1].Block[0].Value); //Load global
                                    }
                                    else if (CurrentProcudure.GetDataSetList().Contains(tokens[i + 1].Block[0].Value))
                                    {
                                        CurrentProcudure.AddInstruction(Instructions.LDVARD, tokens[i + 1].Block[0].Value); //Load local
                                    }
                                    CurrentProcudure.AddInstruction(Instructions.LDINT, "1");
                                    CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                                    break;

                                default:
                                    AppendCount = ParseExpression(tokens[i + 1].Block);
                                    CurrentProcudure.AddInstruction(Instructions.LDINT, AppendCount.ToString());
                                    CurrentProcudure.AddInstruction(Instructions.CALL, token.Value);
                                    break;
                            }
                            _Module.AddFunctionRef(token.Value, token.Value);
                            
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
                                if (_Module.GetDataSet(tokens[i].Value) != null)
                                    lastType = _Module.GetDataSet(tokens[i].Value)._Type;

                                CurrentProcudure.AddInstruction(Instructions.LDGBLV, token.Value); //Load global
                                ParseExpression(tokens[i + 1].Block);
                                CurrentProcudure.AddInstruction(Instructions.LDARRV);
                            }
                            else if (CurrentProcudure.GetDataSetList().Contains(tokens[i].Value))
                            {
                                if (CurrentProcudure.GetDataSet(tokens[i].Value) != null)
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
                                        if (parameter.IsExpression)
                                            ParseExpression(parameter.GetTokens().ToList());
                                        else
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
                                if (GlobalSubfields.ContainsKey(token.Value)) {
                                    switch (GlobalSubfields[token.Value].Location) {
                                        case LOCATION.Global:
                                            CurrentProcudure.AddInstruction(Instructions.LDGBLD, GlobalSubfields[token.Value].Structure); //Load global data
                                            break;
                                        case LOCATION.Local:
                                            CurrentProcudure.AddInstruction(Instructions.LDVARD, GlobalSubfields[token.Value].Structure); //Load local data
                                            break;
                                    }
                                } else {
                                    if (lastType == Types.Void)
                                        Error.ThrowCompileError("Variable " + token.Value + " does not exist", token.Line);
                                }
                                CurrentProcudure.AddInstruction(Instructions.LDFLDV, token.Value);
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

                            case "*ISO":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "yyyy-MM-dd-HH.mm.ss");
                                break;
                            case "*MDY":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "MM/dd/yy");
                                break;
                            case "*DMY":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "dd/MM/yy");
                                break;
                            case "*EUR":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "dd.MM.yy");
                                break;
                            case "*YMD":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "yy/MM/dd");
                                break;
                            case "*USA":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, "hh:mm tt");
                                break;

                            case "*SECONDS":
                            case "*S":
                            case "*MINUTES":
                            case "*MN":
                            case "*HOURS":
                            case "*H":
                            case "*DAYS":
                            case "*D":
                            case "*MONTHS":
                            case "*M":
                            case "*YEARS":
                            case "*Y":
                                CurrentProcudure.AddInstruction(Instructions.LDSTR, token.Value.ToUpper());
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

        private void CorrectTokens(List<RPGToken> tokens) {
            Boolean ChangeMade;
            RPGToken token = null;
            string[] time;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Block != null) {
                    CorrectTokens(tokens[i].Block);
                    
                } else {
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
                                    if (tokens[i + 1].Value.Length == 4 && tokens[i + 1].Value.ToUpper().StartsWith("IN")) {
                                        //Is an indicator variable
                                        token = new RPGToken(RPGLex.Type.WORD_LITERAL, tokens[i + 1].Value.ToUpper(), tokens[i].Line);
                                    } else {
                                        token = new RPGToken(RPGLex.Type.SPECIAL, "*" + tokens[i + 1].Value, tokens[i].Line);
                                    }
                                    ChangeMade = true;
                                }
                            }

                            if (ChangeMade)
                            {
                                tokens.RemoveRange(i, 2);
                                tokens.Insert(i, token);
                            }
                            break;

                        //Need to handle date and time literals
                        case RPGLex.Type.WORD_LITERAL:
                                if (i + 1 < tokens.Count)
                                {
                                    if (tokens[i + 1].Type == RPGLex.Type.STRING_LITERAL)
                                    {
                                        switch (tokens[i].Value) {
                                            case "x":
                                                //Encoding enc = Encoding.GetEncoding(Configurations["CCSID"]);
                                                token = new RPGToken(RPGLex.Type.STRING_LITERAL, EBCDIC.ConvertHex(EBCDIC.GetEncoding(int.Parse(Configurations["CCSID"])), tokens[i + 1].Value));
                                                ChangeMade = true;
                                                break;
                                            case "d":
                                                //TODO HANDLE DATE FORMAT SOMEHOW!!
                                                token = new RPGToken(RPGLex.Type.INT_LITERAL, (DateTime.Parse(tokens[i + 1].Value) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString(), tokens[i].Line);
                                                ChangeMade = true;
                                                break;
                                            case "t":
                                                time = tokens[i + 1].Value.Split(':');
                                                if (time.Length != 3)
                                                    Error.ThrowCompileError("Incorrect time format: " + tokens[i+1].Value, tokens[i+1].Line);
                                                token = new RPGToken(RPGLex.Type.INT_LITERAL, ((int.Parse(time[0]) * 3600) + (int.Parse(time[1]) * 60) + int.Parse(time[2])).ToString(), tokens[i].Line);
                                                ChangeMade = true;
                                                break;
                                        }
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
            }
        }

        public Module GetModule() => _Module;
    }
}
