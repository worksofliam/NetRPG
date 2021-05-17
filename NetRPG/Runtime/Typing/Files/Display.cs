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

        private Dictionary<string, Subfile> Subfiles;
        private Boolean _EOF = false;

        private Dictionary<string, View> localFields;
        public Display(string name, string file, bool userOpen) : base(name, userOpen) {
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

            string indicator, currentSubfile;
            foreach (RecordInfo format in RecordFormats.Values) {
                if (format.Name == "GLOBAL") continue;
                else
                    foreach (var pair in RecordFormats["GLOBAL"].Keywords)
                        format.Keywords.Add(pair.Key, pair.Value);

                currentSubfile = "";

                if (format.Keywords != null) {
                    foreach (string keyword in format.Keywords.Keys) {
                        switch (keyword) {
                            case "SFLCTL":
                                if (Subfiles == null) Subfiles = new Dictionary<string, Subfile>();
                                
                                currentSubfile = format.Keywords[keyword];
                                Subfiles.Add(
                                    currentSubfile,
                                    new Subfile() {rows = new List<Dictionary<string, string>>()}
                                );
                                break;

                            case "SFLPAG":
                                Subfiles[currentSubfile].PerPage = int.Parse(format.Keywords[keyword]);
                                break;

                            case "SFLSIZ":
                                Subfiles[currentSubfile].MaxRows = int.Parse(format.Keywords[keyword]);
                                break;

                            case "PAGEUP":
                            case "ROLLDOWN":
                                format.Function[Key.PageUp] = int.Parse(format.Keywords[keyword]);
                                break;

                            case "PAGEDOWN":
                            case "ROLLUP":
                                format.Function[Key.PageDown] = int.Parse(format.Keywords[keyword]);
                                break;

                            default:
                                if (keyword.StartsWith("CF") && keyword.Length == 4) {
                                    //Sets the function key up
                                    indicator = keyword.Substring(2, 2).TrimStart('0');
                                    format.Function[DisplayParse.IntToKey(int.Parse(indicator))] = int.Parse(format.Keywords[keyword]);
                                }
                                break;
                        }
                    }
                }
            }
        }


        public override void ExecuteFormat(DataValue Structure, DataValue Indicators) {
            RecordInfo recordFormat = RecordFormats[Structure.GetName().ToUpper()];
            
            //Kick in gui.cs
            if (recordFormat.Keywords.Keys.Contains("SFLCTL")) {
                string subfileName = recordFormat.Keywords["SFLCTL"];
                Subfile subfile = Subfiles[subfileName];
                int rows = subfile.PerPage;

                for (int row = 0; row < rows && row < subfile.rows.Count; row++)
                    this.WriteSubfileRow(RecordFormats[subfileName], row);

            } else {
                this.Write(Structure, null);
            }

            foreach (View view in localFields.Values) {
                WindowHandler.Add(view);
            }

            WindowHandler.SetKeys(recordFormat.Function.Keys.ToArray());
            Key result = WindowHandler.Run();

            int indicator = 0;
            if (recordFormat.Function.ContainsKey(result)) {
                indicator = recordFormat.Function[result];
            }

            for (int i = 1; i <= 99; i ++)
                Indicators.GetData("IN" + i.ToString().PadLeft(2, '0')).Set(i == indicator);

            foreach (string varName in Structure.GetSubfieldNames()) {
                //TODO: something about READC?
                if (this.localFields.ContainsKey(varName))
                    if (this.localFields[varName] is TextField)
                        Structure.GetData(varName).Set((this.localFields[varName] as TextField).Text.ToString());
            }

            localFields = new Dictionary<string, View>();
        }

        public override void Write(DataValue Structure, DataValue Indicators) {

            View currentView = null;
            string name = Structure.GetName().ToUpper();
            RecordInfo recordFormat = RecordFormats[name];
            bool toShow = true;

            if (recordFormat.Keywords.Keys.Contains("SFL")) {

                //If this indicator is on, we clear the subfile.
                if (Indicators.Get("IN85") == "1") {
                    Subfiles[name].rows.Clear();
                    Indicators.GetData("IN85").Set(false);

                } else {
                    Dictionary<string, string> columns = new Dictionary<string, string>();
                    foreach (FieldInfo field in recordFormat.Fields) {
                        columns.Add(field.Name, Structure.GetData(field.Name).Get());
                    }

                    Subfiles[name].rows.Add(columns);
                }

            } else {

                if (recordFormat.Fields.Count() > 0) {
                    foreach (FieldInfo field in recordFormat.Fields) {
                        toShow = true;

                        if (field.Conditionals.Count > 0) {
                            foreach (Conditional cond in field.Conditionals) {
                                toShow = (Indicators.Get("IN" + cond.indicator.ToString().PadLeft(2, '0')) == (cond.negate ? "0" : "1"));
                            }
                        }

                        if (!toShow) continue;

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
            }

        }

        public void WriteSubfileRow(RecordInfo format, int plusRow = 0) {
            View currentView = null;
            Subfile subfile = Subfiles[format.Name];

            foreach (FieldInfo field in format.Fields) {
                currentView = null;

                switch (field.fieldType) {
                    case FieldInfo.FieldType.Const:
                        currentView = new Label (subfile.rows[plusRow][field.Name]) { X = field.Position.X, Y = field.Position.Y + plusRow };
                        break;
                        
                    case FieldInfo.FieldType.Output:
                        currentView = new Label (subfile.rows[plusRow][field.Name]) { X = field.Position.X, Y = field.Position.Y + plusRow };
                        break;

                    case FieldInfo.FieldType.Input:
                        currentView = new TextField ("") {
                            X = field.Position.X, Y = field.Position.Y + plusRow,
                            Width = field.dataType._Length,
                            Height = 1
                        };
                        break;
                        
                    case FieldInfo.FieldType.Both:
                        currentView = new TextField ("") {
                            X = field.Position.X, Y = field.Position.Y + plusRow,
                            Width = field.dataType._Length,
                            Height = 1, Text = subfile.rows[plusRow][field.Name]
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

                if (currentView != null)
                    localFields.Add(field.Name + "-" + plusRow.ToString(), currentView);
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

    public class Subfile {
        public List<Dictionary<string, string>> rows;
        public int MaxRows = 0;

        public int PerPage = 0;
    }
}