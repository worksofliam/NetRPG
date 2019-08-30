using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Runtime.Functions
{
    class Function
    {
        //TODO: only add items to this list if it's used
        private static Dictionary<string, Function> List = new Dictionary<string, Function>
        {
        };

        public static void AddFunctionReference(string Name, string Func) {
            Function result = null;

            switch (Func.ToUpper()) {
                case "DSPLY": result = new Operation.Dsply(); break;
                case "IN": result = new Operation.In(); break;
                case "RESET": result = new Operation.Reset(); break;
                case "CLEAR": result = new Operation.Clear(); break;

                case "OPEN": result = new Operation.Open(); break;
                case "READ": result = new Operation.Read(); break;
                case "READP": result = new Operation.ReadPrevious(); break;
                case "CHAIN": result = new Operation.Chain(); break;
                case "EXFMT": result = new Operation.ExecuteFormat(); break;
                case "WRITE": result = new Operation.Write(); break;
                case "%FOUND": result = new Operation.Found(); break;
                case "%EOF": result = new Operation.EndOfFile(); break;

                case "%ABS": result = new BIF.Abs(); break;
                case "%CHAR": result = new BIF.Char(); break;
                case "%DEC": result = new BIF.Dec(); break;
                case "%DECH": result = new BIF.Dec(); break;
                case "%DECPOS": result = new BIF.DecPos(); break;
                case "%EDITC": result = new BIF.EditC(); break;
                case "%ELEM": result = new BIF.Elem(); break;
                case "%FLOAT": result = new BIF.Float(); break;
                case "%INT": result = new BIF.Int(); break;
                case "%LEN": result = new BIF.Len(); break;
                case "%LOOKUP": result = new BIF.Lookup(); break;
                case "%TRIM": result = new BIF.Trim(); break;
                case "%TRIMR": result = new BIF.TrimR(); break;
                case "%TRIML": result = new BIF.TrimL(); break;
                case "%SCAN": result = new BIF.Scan(); break;
                case "%SCANRPL": result = new BIF.ScanReplace(); break;
                case "%XLATE": result = new BIF.Xlate(); break;

                case "%TIMESTAMP": result = new BIF.Timestamp(); break;
                case "%DATE": result = new BIF.Timestamp(); break;
                case "%TIME": result = new BIF.Timestamp(); break;
                
                case "%SECONDS": result = new BIF.Seconds(); break;
                case "%MINUTES": result = new BIF.Minutes(); break;
                case "%HOURS": result = new BIF.Hours(); break;
                case "%DAYS": result = new BIF.Days(); break;
                case "%MONTHS": result = new BIF.Months(); break;
                case "%YEARS": result = new BIF.Years(); break;
                case "%DIFF": result = new BIF.Diff(); break;
                case "%SUBDT": result = new BIF.SubDateTime(); break;

                case "PRINTF": result = new System.printf(); break;
            }
            
            if (result == null) {
                Error.ThrowCompileError("Function '" + Func + "' does not exist in system.");
            } else {
                if (!List.ContainsKey(Name.ToUpper()))
                    List.Add(Name.ToUpper(), result);
            }
        }

        public static bool IsFunction(string Name)
        {
            return List.ContainsKey(Name.ToUpper());
        }

        public static Function GetFunction(string Name)
        {
            Name = Name.ToUpper();

            if (List.ContainsKey(Name))
                return List[Name];
            else
                return null; //TODO: throw error
        }
        
        public virtual object Execute(object[] Parameters)
        {
            return null;
        }
    }
}
