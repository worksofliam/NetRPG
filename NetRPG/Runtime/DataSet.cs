using System;
using System.Collections.Generic;
using NetRPG.Runtime.Typing;

namespace NetRPG.Runtime
{
    /// <summary>
    /// A DataSet can be a variable, file, or struct
    /// </summary>
    public class DataSet
    {
        public string _Name;
        public Types _Type;
        public int _Length; //For non-structures
        public int _Dimentions; //If 0, not an array
        public object _InitialValue;
        public List<DataSet> _Subfields;

        //For data-structures
        public bool _Qualified; 
        public bool _Template;

        public DataSet(string name)
        {
            _Name = name;
            _Qualified = false;
        }

        public bool IsArray() => (_Dimentions > 1);

        public DataValue ToDataValue()
        {
            DataValue result = null;
            
            switch (this._Type)
            {
                case Types.Character:
                    result = new Character(this._Name, this._Length, (string) this._InitialValue);
                    if (IsArray()) result.SetArray(this._Dimentions);
                    break;

                case Types.Structure:
                    result = new Structure(this._Name);
                    if (IsArray()) result.SetArray(this._Dimentions);
                    result.SetSubfields(_Subfields.ToArray());
                    break;

                default:
                    throw new Exception(this._Type.ToString() + " is not a ready data type.");
            }


            return result;
        }
    }
}
