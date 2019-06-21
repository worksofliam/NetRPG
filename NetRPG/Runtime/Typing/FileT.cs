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
      private Boolean _EOF = false;
      public FileT(string name, string file, bool userOpen) {
      }

      public virtual Boolean isEOF() => this._EOF;

      public virtual void Open() {

      }

      public virtual void Read(DataValue Structure) {

      }

      public virtual void ReadPrevious(DataValue Structure) {

      }

      public virtual void Chain(DataValue Structure, dynamic[] keys) {

      }

      public virtual void ExecuteFormat(DataValue Indicators, DataValue Structure) {
        
      }

      public virtual void Write(DataValue Structure) {
        
      }
    }
}