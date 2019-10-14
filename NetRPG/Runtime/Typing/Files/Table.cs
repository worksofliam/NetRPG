using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NetRPG.Language;

namespace NetRPG.Runtime.Typing.Files
{
    class Table : FileT
    {
        private string _Path;
        private int _RowPointer = -1;
        private List<Dictionary<string, dynamic>> _Data;
        private Boolean _EOF = false;

        public Table(string name, string file, bool userOpen) : base(name, file, userOpen) {
            this.Name = name;
            _Path = Path.Combine(Environment.CurrentDirectory, "objects", file + ".json");

            if (!userOpen)
                this.Open();
        }

        public static DataSet CreateStruct(string name, Boolean qualified = false) {
                string content = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "objects", name + ".json"));

                DataSet Structure = new DataSet(name);
                Structure._Type = Types.Structure;
                Structure._Qualified = qualified;
                List<DataSet> subfields = new List<DataSet>();;

                JObject json = JObject.Parse(content);

                DataSet subfield;
                JProperty DataProperty;
                foreach (JToken obj in json["columns"].ToList<JToken>()) {
                    DataProperty = obj.ToObject<JProperty>();
                    subfield = new DataSet(DataProperty.Name);
                    subfield._Type = Reader.StringToType(json["columns"][DataProperty.Name]["type"].ToString());
                    subfield._Length = (int)json["columns"][DataProperty.Name]["length"];
                    subfields.Add(subfield);
                }

                Structure._Subfields = subfields;

                return Structure;
        }

        public override void Open() {
            this._RowPointer = -1;

            string content = File.ReadAllText(this._Path);

            _Data = new List<Dictionary<string, dynamic>>();
            Dictionary<string, dynamic> row;

            JObject json = JObject.Parse(content);

            foreach (JObject obj in json["rows"].Children<JObject>())
            {
                row = new Dictionary<string, dynamic>();
                foreach (JProperty property in obj.Properties())
                {
                    switch (property.First.Type) {
                        case JTokenType.Boolean:
                            row[property.Name] = (property.Value.ToString() == "1" ? "1" : "0" );
                            break;
                        case JTokenType.Integer:
                            row[property.Name] = Convert.ToInt32((int)property.Value);
                            break;
                        case JTokenType.Float:
                            row[property.Name] = Convert.ToDouble((float)property.Value);
                            break;
                        case JTokenType.String:
                            row[property.Name] = property.Value.ToString();
                            break;
                    }
                }
                this._Data.Add(row);
            }
        }

        public override Boolean isEOF() => this._EOF;

        public override void Read(DataValue Structure) {
            this._RowPointer += 1;

            if (this._RowPointer >= 0 && this._RowPointer < this._Data.Count()) {
                this._EOF = false;

                foreach (string varName in this._Data[this._RowPointer].Keys.ToArray()) {
                    Structure.GetData(varName).Set(this._Data[this._RowPointer][varName]);
                }

            } else {
                this._EOF = true;
            }
        }

        public override void ReadPrevious(DataValue Structure) {
            this._RowPointer -= 1;

            if (this._RowPointer >= 0 && this._RowPointer < this._Data.Count()) {
                this._EOF = false;

                foreach (string varName in this._Data[this._RowPointer].Keys.ToArray()) {
                    Structure.GetData(varName).Set(this._Data[this._RowPointer][varName]);
                }

            } else {
                this._EOF = true;
            }
        }

        public override void Chain(DataValue Structure, dynamic[] keys) {
            this._EOF = true;

            this._RowPointer = 0;

            while (this._RowPointer >= 0 && this._RowPointer < this._Data.Count()) {

                for (var i = 0; i < keys.Length; i++) {
                    if (keys[i] != this._Data[this._RowPointer].ElementAt(i).Value) {
                        continue;
                    }

                    foreach (string varName in this._Data[this._RowPointer].Keys.ToArray()) {
                        Structure.GetData(varName).Set(this._Data[this._RowPointer][varName]);
                    }

                    this._EOF = false;
                    return;
                }

                this._RowPointer += 1;

            }
        }

        public override void Write(DataValue Structure) {
            Dictionary<string, dynamic> NewRow = new Dictionary<string, dynamic>();

            foreach (string Subfield in Structure.GetSubfieldNames()) {
                NewRow[Subfield] = Structure.Get(Subfield);
            }

            this._Data.Add(NewRow);
        }
    }
}