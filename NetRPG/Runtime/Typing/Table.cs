using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using NetRPG.Language;

namespace NetRPG.Runtime.Typing
{
    class Table : DataValue
    {
      private string _Path;
      private int _RowPointer = -1;
      private Dictionary<string, DataSet> _Columns;
      private List<Dictionary<string, dynamic>> _Data;
      public Table(string name, bool userOpen) {
          this.Name = name;
          _Path = Path.Combine(Environment.CurrentDirectory, "files", name + ".json");

          if (userOpen == false) {
              this.Open();
          }
      }

      public void Open() {
        this._Columns = new Dictionary<string, DataSet>();
        this._RowPointer = -1;

        string content = File.ReadAllText(this._Path);

        _Data = new List<Dictionary<string, dynamic>>();
        Dictionary<string, dynamic> row;

        JObject json = JObject.Parse(content);

        DataSet dataSet;
        foreach (JObject obj in json["columns"].Children<JObject>()) {
            foreach (JProperty prop in obj.Properties())
            {
                dataSet = new DataSet(prop.Name);
                dataSet._Type = Reader.StringToType(prop["type"].ToString());
                dataSet._Length = (int)prop["length"];
                this._Columns.Add(prop.Name, dataSet);
            }
        }

        foreach (JObject obj in json["rows"].Children<JObject>())
        {
            row = new Dictionary<string, dynamic>();
            foreach (JProperty property in obj.Properties())
            {
                row[property.Name] = property.Value;
            }
        }
      }
    }
}