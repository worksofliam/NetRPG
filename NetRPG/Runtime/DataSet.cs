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
        public int _Precision;
        public int _Dimentions; //If 0, not an array
        public object _InitialValue;
        public List<DataSet> _Subfields;

        //For data-structures
        public bool _Qualified = false; 
        public bool _Template;

        public bool _UserOpen = false;

        public bool _IsConstOrValue = false;

        public string _File;

        public string _DataArea = null;

        public Boolean _WorkStation = false;

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
                case Types.Ind:
                    result = new Ind(this._Name, (string)this._InitialValue);
                    break;

                case Types.Varying:
                    result = new VaryingCharacter(this._Name, this._Length, (string) this._InitialValue);
                    break;

                case Types.Character:
                    result = new Character(this._Name, this._Length, (string) this._InitialValue);
                    break;

                case Types.Int8:
                case Types.Int16:
                case Types.Int32:
                case Types.Int64:
                    result = new Int(this._Name, this._Type, Convert.ToInt32(this._InitialValue));
                    break;

                case Types.Structure:
                    result = new Structure(this._Name, this._Qualified);
                    break;
                    
                case Types.FixedDecimal: //Packed / Zoned
                    result = new FixedDecimal(this._Name, this._Type, this._Precision, Convert.ToDouble(this._InitialValue));
                    break;
                
                case Types.Float:
                case Types.Double:
                    result = new Float(this._Name, this._Type, Convert.ToDouble(this._InitialValue));
                    break;

                case Types.Timestamp:
                    result = new Timestamp(this._Name, Convert.ToInt32(this._InitialValue));
                    break;

                case Types.File:
                    if (this._WorkStation) {
                        result = new Typing.Files.Display(this._Name, this._File, this._UserOpen);
                    } else {
                        result = new Typing.Files.JSONTable(this._Name, this._File, this._UserOpen);
                    }
                    break;

                default:
                    Error.ThrowRuntimeError("DataSet.ToDataValue", this._Type.ToString() + " is not a ready data type.");
                    break;
            }

            if (this._DataArea != null)
                result.SetDataAreaName(this._DataArea);
            if (IsArray()) result.SetArray(this._Dimentions);
            if (this._Type == Types.Structure && _Subfields != null)
                result.SetSubfields(_Subfields.ToArray()); //Must be run after array size has been set


            return result;
        }
    }
}
