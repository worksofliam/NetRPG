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
                currentStruct._Qualified = qualified;

                subfields = new List<DataSet>();
                foreach (FieldInfo field in format.Fields) {
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
        }

        public override void ExecuteFormat(DataValue Structure) {
            //Kick in gui.cs
            this.Write(Structure);
            Application.Run ();

            foreach (string varName in this.localFields.Keys) {
                Structure.GetData(varName).Set(this.localFields[varName]);
            }
        }

        public override void Write(DataValue Structure) {
            Application.Init();

            Colors.Base.Normal = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Base.Focus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Dialog.Focus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Dialog.HotFocus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);

            var win = new DisplayWindow ("") {
                X = 0,
                Y = 0,
                Width = 80,
                Height = 24
            };

            localFields = new Dictionary<string, View>();
            View currentView = null;
            RecordInfo recordFormat = RecordFormats[Structure.GetName().ToUpper()];

            foreach (FieldInfo field in recordFormat.Fields) {
                switch (field.fieldType) {
                    case FieldInfo.FieldType.Const:
                        currentView = new Label (field.Value) { X = field.Position.X, Y = field.Position.Y };
                        break;
                    case FieldInfo.FieldType.Both:
                        currentView = new TextField ("") {
                            X = field.Position.X, Y = field.Position.Y,
                            Width = field.dataType._Length
                        };
                        break;
                }

                localFields.Add(field.Name, currentView);
            }

            win.ColorScheme = Colors.Base;
        }
    }
}