using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;
using System.Linq;

namespace NetRPG.Runtime.Functions.System.DataQueues
{
    class BasicDQ : IDataQueue
    {
        private Dictionary<string, List<string>> DQs;

        public BasicDQ() {
              DQs = new Dictionary<string, List<string>>();
        }

        public void Push(string name, string item) {
            if (!DQs.ContainsKey(name))
              DQs.Add(name, new List<string>());

            DQs[name].Add(item);
        }

        public string Pop(string name) {
            if (DQs == null)
              Error.ThrowRuntimeError("No data queues exist.", "DataQueues#Pop");

            if (!DQs.ContainsKey(name))
              Error.ThrowRuntimeError("Data queue '" + name + "' does not exist.", "DataQueues#Pop");

            if (DQs[name].Count == 0)
              Error.ThrowRuntimeError("Data queue '" + name + "' is empty", "DataQueues#Pop");

            string output = DQs[name].Last();
            DQs[name].RemoveAt(DQs[name].Count - 1);

            return output;
        }
    }
}
