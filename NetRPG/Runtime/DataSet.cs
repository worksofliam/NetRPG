using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime
{
    /// <summary>
    /// A DataSet can be a variable, file, or struct
    /// </summary>
    public class DataSet
    {
        public string _Name;
        public Types _Type;
        public int _Length;
        public int _Dimentions; //If 0, not an array

        public object _InitialValue;

        public DataSet(string name)
        {
            _Name = name;
        }

        public bool IsArray() => (_Dimentions > 0);
    }
}
