using System;
using System.Collections.Generic;
using System.Text;
using NetRPG.Runtime.Functions.System.DataQueues;
using System.Linq;

namespace NetRPG.Runtime.Functions.System
{
    class DataQueue
    {
        //This is where would we change the dataqueue if we were to add MQ support
        private static IDataQueue CurrentDQ = new BasicDQ();
        public static void Push(string name, string item) => CurrentDQ.Push(name, item);
        public static string Pop(string name) => CurrentDQ.Pop(name);
    }
}