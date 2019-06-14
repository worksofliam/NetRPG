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

        private Window window;
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

            Application.Init();

            Colors.Base.Normal = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Base.Focus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Dialog.Focus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);
            Colors.Dialog.HotFocus = Application.Driver.MakeAttribute (Color.BrightGreen, Color.Black);

            window = new DisplayWindow ("") {
                X = 0,
                Y = 0,
                Width = 80,
                Height = 24
            };
            
            window.ColorScheme = Colors.Base;

            Application.Top.Add (window);
        }

        public override void ExecuteFormat(DataValue Structure) {
            //Kick in gui.cs
            this.Write(Structure);
            foreach (View view in localFields.Values) {
                window.Add(view);
            }

            Application.Top.Add (window);
            Application.Run ();

            foreach (string varName in Structure.GetSubfieldNames()) {
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
                        
                    case FieldInfo.FieldType.Both:
                        currentView = new TextField (field.Value) {
                            X = field.Position.X, Y = field.Position.Y,
                            Width = field.dataType._Length,
                            Height = 1,
                        };
                        break;
                }

                localFields.Add(field.Name, currentView);
            }

        }
    }
}