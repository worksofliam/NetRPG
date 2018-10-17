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
        protected Dictionary<string, dynamic> Properties;
        protected Dictionary<string, DataValue> Subfields;

        public void SetArray(int Count)
        {
            this.Dimentions = Count;
            this.Value = new object[this.Dimentions];

            this.InitialValue();
        }

        public string GetName() => this.Name;

        public virtual void Set(object value, int index = 0)
        {
            //this.Value[index] = value;
            this.Set(value, index);
        }

        public dynamic Get()
        {
            if (Dimentions > 1) //If it's an array
                return this.Value;
            else
                return this.Value[0];
        }

        public DataValue Get(string subfield)
        {
            return this.Subfields[subfield];
        }

        protected void InitialValue()
        {
            dynamic initialValue = null;
            if (this.Properties.ContainsKey("initialValue"))
            {
                initialValue = this.Properties["initialValue"];
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
                    case Types.Int8:
                    case Types.Int16:
                    case Types.Int32:
                    case Types.Int64:
                    case Types.Packed:
                    case Types.Zoned:
                        initialValue = 0;
                        break;
                }
            }

            for (int x = 0; x < this.Dimentions; x++)
                this.Value[x] = initialValue;
        }
    }
}
