using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Typing.Files;
using System.Linq;

namespace NetRPG.Runtime.Functions.System.DataQueues
{
    interface IDataQueue
    {

        void Push(string name, string item);

        string Pop(string name);
    }
}
