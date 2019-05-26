using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NetRPG.Language;

namespace NetRPG.Runtime.Typing
{
    class Table : DataValue
    {
      private string _Path;
      private int _RowPointer = -1;
      private Dictionary<string, DataValue> _Columns;
      private List<Dictionary<string, dynamic>> _Data;
      public Table(string name, bool userOpen) {
          this.Name = name;
          _Path = Path.Combine(Environment.CurrentDirectory, "files", name + ".json");

          if (userOpen == false) {
              this.Open();
          }
      }

      public void Open() {
        this._Columns = new Dictionary<string, DataValue>();
        this._RowPointer = -1;

        string content = File.ReadAllText(this._Path);

        _Data = new List<Dictionary<string, dynamic>>();
        Dictionary<string, dynamic> row;

        JObject json = JObject.Parse(content);

        DataSet dataSet;
        JProperty DataProperty;
        foreach (JToken obj in json["columns"].ToList<JToken>()) {
            DataProperty = obj.ToObject<JProperty>();
            dataSet = new DataSet(DataProperty.Name);
            dataSet._Type = Reader.StringToType(json["columns"][DataProperty.Name]["type"].ToString());
            dataSet._Length = (int)json["columns"][DataProperty.Name]["length"];
            this._Columns.Add(DataProperty.Name, dataSet.ToDataValue());
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