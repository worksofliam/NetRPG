using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Typing
{
    class Structure : DataValue
    {
        private Boolean _Qualified = false;
        public Structure(string name, Boolean qualified)
        {
            this.Name = name;
            this.Type = Types.Structure;

            this.Dimentions = 1;
            this.Value = new object[this.Dimentions];

            this._Qualified = qualified;
        }

        public Boolean isQualified() => this._Qualified;
        public override void SetSubfields(DataSet[] subfieldsData)
        {
            DataValue[] subfields;
            this.Subfields = new Dictionary<string, int>();
            
            for (int x = 0; x < this.Value.Length; x++)
            {
                subfields = new DataValue[subfieldsData.Length];
                
                for (int y = 0; y < subfieldsData.Length; y++)
                {
                    subfields[y] = subfieldsData[y].ToDataValue();
                    this.Subfields[subfieldsData[y]._Name] = y;
                }

                foreach (DataValue subfield in subfields)
                    subfield.DoInitialValue();

                this.Value[x] = subfields;
            }
        }
    }
}
