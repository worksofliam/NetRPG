using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime
{

    public class VM {

      private string _EntryProcedure;
      private Dictionary<string, Procedure> _Procedures;

      public VM() {
        _EntryProcedure = "";
        _Procedures = new Dictionary<string, Procedure>();
      }

      public void AddModule(Module module) {
        foreach(Procedure proc in module.GetProcedures()) {
          _Procedures.Add(proc.GetName(), proc);

          if (proc.HasEntrypoint) _EntryProcedure = proc.GetName();
        }
      } 

      public void Run() {
        //TODO: initialise globals
        //TODO: Run entrypoint procedure
      }

      private void Execute(string Name, DataValue[] Parms) {
        Instruction[] instructions = _Procedures[Name].GetInstructions();

        //TODO: initialise variables
        //TOOD: execute instructions
      }
    }

}