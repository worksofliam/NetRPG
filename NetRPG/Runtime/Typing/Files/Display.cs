using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NetRPG.Language;
using Terminal.Gui;
using NetRPG.Runtime.Typing;

namespace NetRPG.Runtime.Typing.Files
{
    class Display : FileT
    {
        private string _Path;
        private int _RowPointer = -1;
        private Dictionary<string, RecordInfo> RecordFormats;
        private Boolean _EOF = false;

        private Dictionary<string, View> localFields;
        public Display(string name, string file, bool userOpen) : base(name, file, userOpen) {
            this.Name = name;
            _Path = Path.Combine(Environment.CurrentDirectory, "objects", file + ".dspf");

            if (!userOpen)
                this.Open();
        }

        public static DataSet[] CreateStructs(string file, Boolean qualified = false) {
            List<DataSet> Structures = new List<DataSet>();

            DataSet currentStruct = null;
            List<DataSet> subfields = null;

            DisplayParse parser = new DisplayParse();
            parser.ParseFile(Path.Combine(Environment.CurrentDirectory, "objects", file + ".dspf"));

            foreach (RecordInfo format in parser.GetRecordFormats().Values) {
                currentStruct = new DataSet(format.Name);
                currentStruct._Type = Types.Structure;
                currentStruct._Qualified = qualified;

                subfields = new List<DataSet>();
                foreach (FieldInfo field in format.Fields) {
                    if (field.fieldType != FieldInfo.FieldType.Const)
                        subfields.Add(field.dataType);
                }

                currentStruct._Subfields = subfields;
                Structures.Add(currentStruct);
            }
            
            return Structures.ToArray();
        }

        public override void Open() {
            DisplayParse parser = new DisplayParse();
            parser.ParseFile(_Path);
            this.RecordFormats = parser.GetRecordFormats();
            localFields = new Dictionary<string, View>();
        }


        public override void ExecuteFormat(DataValue Structure, DataValue Indicators) {
            RecordInfo recordFormat = RecordFormats[Structure.GetName().ToUpper()];
            
            //Kick in gui.cs
            this.Write(Structure);
            foreach (View view in localFields.Values) {
                WindowHandler.Add(view);
            }

            WindowHandler.Run();

            foreach (string varName in Structure.GetSubfieldNames()) {
                if (this.localFields[varName] is TextField)
                    Structure.GetData(varName).Set((this.localFields[varName] as TextField).Text.ToString());
            }

            localFields = new Dictionary<string, View>();
        }

        public override void Write(DataValue Structure) {

            View currentView = null;
            RecordInfo recordFormat = RecordFormats[Structure.GetName().ToUpper()];

            foreach (FieldInfo field in recordFormat.Fields) {
                switch (field.fieldType) {
                    case FieldInfo.FieldType.Const:
                        currentView = new Label (field.Value) { X = field.Position.X, Y = field.Position.Y };
                        break;
                        
                    case FieldInfo.FieldType.Output:
                        currentView = new Label (Structure.GetData(field.Name).Get().ToString()) { X = field.Position.X, Y = field.Position.Y };
                        break;

                    case FieldInfo.FieldType.Input:
                        currentView = new TextField ("") {
                            X = field.Position.X, Y = field.Position.Y,
                            Width = field.dataType._Length,
                            Height = 1
                        };
                        break;
                        
                    case FieldInfo.FieldType.Both:
                        currentView = new TextField ("") {
                            X = field.Position.X, Y = field.Position.Y,
                            Width = field.dataType._Length,
                            Height = 1, Text = Structure.GetData(field.Name).Get().ToString()
                        };
                        break;
                }

                if (field.Keywords != null) {
                    foreach (string keyword in field.Keywords.Keys) {
                        switch (keyword) {
                            case "COLOR":
                                currentView.ColorScheme = new ColorScheme();
                                currentView.ColorScheme.Normal = Application.Driver.MakeAttribute (TextToColor(field.Keywords[keyword]), Color.Black);
                                currentView.ColorScheme.Focus = Application.Driver.MakeAttribute (TextToColor(field.Keywords[keyword]), Color.Black);
                                break;
                                
                            case "SYSNAME":
                                (currentView as Label).Text = Environment.MachineName.Substring(0, Math.Min(8, Environment.MachineName.Length));
                                break;
                        }
                    } 
                }

                localFields.Add(field.Name, currentView);
            }

        }


        public static Color TextToColor(string Colour)
        {
            switch (Colour.ToUpper())
            {
                case "GRN":
                    return Color.BrightGreen;
                case "YLW":
                    return Color.BrightYellow;
                case "BLU":
                    return Color.Cyan;
                case "RED":
                    return Color.BrightRed;
                case "WHT":
                    return Color.White;
                case "TRQ":
                    return Color.BrighCyan;
                case "PNK":
                    return Color.BrightMagenta;

                default:
                    return Color.BrightGreen;
            }
        }
    }
}