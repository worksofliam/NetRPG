using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NetRPG.Language;

namespace NetRPG.Runtime.Typing
{
    class FileT : DataValue
    {
      private string _Path;
      private int _RowPointer = -1;
      private List<Dictionary<string, dynamic>> _Data;
      private Boolean _EOF = false;
      public FileT(string name, string file, bool userOpen) {
      }
      
      public static DataSet CreateStruct(string name, Boolean qualified = false) {
          return null;
      }

      public Boolean isEOF() => this._EOF;

      public virtual void Open() {

      }

      public virtual void Read(DataValue Structure) {

      }

      public virtual void ReadPrevious(DataValue Structure) {

      }

      public virtual void Chain(DataValue Structure, dynamic[] keys) {

      }
    }
}