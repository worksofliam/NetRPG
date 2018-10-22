using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Typing
{
    public class DataValue
    {
        protected string Name;
        protected Types Type;
        protected Object[] Value;
        protected int Dimentions = 1;
        protected dynamic InitValue = null;
        protected Dictionary<string, int> Subfields;

        public void SetArray(int Count)
        {
            this.Dimentions = Count;
            this.Value = new object[this.Dimentions];

            this.DoInitialValue();
        }

        public string GetName() => this.Name;

        public virtual void Set(object value, int index = 0)
        {
            this.Value[index] = value;
        }

        public virtual void Set(object value, string subfield)
        {
            this.Value[this.Subfields[subfield]] = value;
        }

        public dynamic Get()
        {
            if (Dimentions > 1) //If it's an array
                return this.Value;
            else
                return this.Value[0];
        }

        public virtual void SetSubfields(DataSet[] subfieldsData) { }

        public int GetSubfield(string subfield)
        {
            return this.Subfields[subfield];
        }

        public DataValue GetData(string subfield, int index = 0)
        {
            DataValue[] temp = (DataValue[])this.Value[index];
            return temp[this.Subfields[subfield]];
        }

        public dynamic Get(string subfield, int index = 0)
        {
            DataValue[] temp = (DataValue[])this.Value[index];
            return temp[this.Subfields[subfield]].Get();
        }

        public dynamic Get(int index)
        {
            return this.Value[index];
        }

        public void DoInitialValue()
        {
            dynamic initialValue = null;
            if (this.InitValue != null)
            {
                initialValue = this.InitValue;
            }
            else
            {
                switch (this.Type)
                {
                    case Types.Pointer:
                        initialValue = null;
                        break;
                    case Types.Character:
                    case Types.Varying:
                        initialValue = "";
                        break;
                    case Types.Double:
                    case Types.Float:
                    case Types.FixedDecimal:
                        initialValue = 0.0;
                        break;
                    case Types.Int8:
                    case Types.Int16:
                    case Types.Int32:
                    case Types.Int64:
                        initialValue = 0;
                        break;
                }
            }

            for (int x = 0; x < this.Dimentions; x++)
                this.Value[x] = initialValue;
        }

        public DataValue Clone()
        {
            return (DataValue)this.MemberwiseClone();
        }
    }
}
