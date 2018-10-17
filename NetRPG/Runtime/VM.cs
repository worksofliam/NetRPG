using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Runtime
{

    public class VM
    {
        private Dictionary<string, DataValue> GlobalVariables;
        private string _EntryProcedure;
        private Dictionary<string, Procedure> _Procedures;

        public VM()
        {
            _EntryProcedure = "";
            _Procedures = new Dictionary<string, Procedure>();
            GlobalVariables = new Dictionary<string, DataValue>();
        }

        public void AddModule(Module module)
        {
            foreach (Procedure proc in module.GetProcedures())
            {
                _Procedures.Add(proc.GetName(), proc);
                if (proc.HasEntrypoint) _EntryProcedure = proc.GetName();
            }
            
            foreach (String global in module.GetDataSetList())
            {
                DataValue set = module.GetDataSet(global).ToDataValue();
                GlobalVariables.Add(set.GetName(), set);
            }
        }

        public void Run()
        {
            //TODO: initialise globals
            //TODO: Run entrypoint procedure
            Execute(_EntryProcedure);
        }

        private object Execute(string Name, DataValue[] Parms = null)
        {
            object[] tempArray;
            object[] Values = new object[3];
            List<object> Stack = new List<object>();

            Dictionary<string, int> Labels = new Dictionary<string, int>();
            Dictionary<string, DataValue> LocalVariables = new Dictionary<string, DataValue>();
            Instruction[] instructions = _Procedures[Name].GetInstructions();

            //TODO: initialise variables
            foreach (String global in _Procedures[Name].GetDataSetList())
            {
                DataValue set = _Procedures[Name].GetDataSet(global).ToDataValue();
                LocalVariables.Add(set.GetName(), set);
                LocalVariables[set.GetName()].Set(_Procedures[Name].GetDataSet(global)._InitialValue);
            }

            //TODO: Do this only once and not everytime a procedure is called.
            for(int i = 0; i < instructions.Count(); i++)
                if (instructions[i]._Instruction == Instructions.LABEL)
                    Labels.Add(instructions[i]._Value, i);

            for (int ip = 0; ip < instructions.Count(); ip++)
            {
                switch (instructions[ip]._Instruction)
                {
                    case Instructions.APPEND:
                    case Instructions.ADD:
                    case Instructions.SUB:
                    case Instructions.DIV:
                    case Instructions.EQUAL:
                    case Instructions.GREATER:
                    case Instructions.GREATER_EQUAL:
                    case Instructions.LESSER:
                    case Instructions.LESSER_EQUAL:
                    case Instructions.NOT_EQUAL:
                    case Instructions.OR:
                        Values[0] = Stack[Stack.Count - 2];
                        Values[1] = Stack[Stack.Count - 1];
                        Stack.RemoveRange(Stack.Count-2, 2);
                        Stack.Add(Operate(instructions[ip]._Instruction, Values[0], Values[1]));
                        break;
                        

                    case Instructions.BR:
                        ip = Labels[instructions[ip]._Value];
                        break;

                    case Instructions.BRFALSE:
                        Values[0] = Stack[Stack.Count - 1];
                        Stack.RemoveRange(Stack.Count - 1, 1);
                        if ((bool) Operate(Instructions.EQUAL, Values[0], false))
                            ip = Labels[instructions[ip]._Value];
                        break;

                    case Instructions.BRTRUE:
                        Values[0] = Stack[Stack.Count - 1];
                        Stack.RemoveRange(Stack.Count - 1, 1);
                        if ((bool)Operate(Instructions.EQUAL, Values[0], true))
                            ip = Labels[instructions[ip]._Value];
                        break;

                    case Instructions.CALL:
                        //TODO: implement
                        switch (instructions[ip]._Value.ToUpper())
                        {
                            case "DSPLY":
                                Values[0] = Stack[Stack.Count - 1];
                                Console.WriteLine(Values[0]);
                                Stack.RemoveRange(Stack.Count - 1, 1);
                                break;
                        }
                        break;

                    case Instructions.LDARR:
                        tempArray = (object[])Stack[Stack.Count - 2];
                        Values[1] = Stack[Stack.Count - 1];
                        Stack.RemoveRange(Stack.Count - 2, 2);
                        Stack.Add(tempArray[(int) Values[1]]);
                        break;

                    case Instructions.LDFLD:
                        //TODO implement
                        break;

                    case Instructions.LDGBL:
                        Stack.Add(GlobalVariables[instructions[ip]._Value].Get());
                        break;

                    case Instructions.LDNUM:
                        Stack.Add(double.Parse(instructions[ip]._Value));
                        break;

                    case Instructions.LDSTR:
                        Stack.Add(instructions[ip]._Value);
                        break;

                    case Instructions.LDVAR:
                        Stack.Add(LocalVariables[instructions[ip]._Value].Get());
                        break;

                    case Instructions.NOT:
                        Values[0] = Stack[Stack.Count - 1];
                        Stack.RemoveRange(Stack.Count - 1, 1);
                        Stack.Add(!(bool)Values[0]);
                        break;

                    case Instructions.RETURN:
                        if (_Procedures[Name]._ReturnType == Types.Void)
                            return null;
                        else
                        {
                            Values[0] = Stack[Stack.Count - 1];
                            return Values[0];
                        }
                            
                    case Instructions.STARR:
                        Values[0] = Stack[Stack.Count - 2];
                        Values[1] = Stack[Stack.Count - 1];

                        if (GlobalVariables.ContainsKey(instructions[ip]._Value))
                            GlobalVariables[instructions[ip]._Value].Set(Values[0], (int)Values[1]);
                        else if (LocalVariables.ContainsKey(instructions[ip]._Value))
                            LocalVariables[instructions[ip]._Value].Set(Values[0], (int)Values[1]);

                        Stack.RemoveRange(Stack.Count - 2, 2);
                        break;

                    case Instructions.STVAR:
                        Values[0] = Stack[Stack.Count - 1];

                        if (GlobalVariables.ContainsKey(instructions[ip]._Value))
                            GlobalVariables[instructions[ip]._Value].Set(Values[0]);
                        else if (LocalVariables.ContainsKey(instructions[ip]._Value))
                            LocalVariables[instructions[ip]._Value].Set(Values[0]);

                        Stack.RemoveRange(Stack.Count - 1, 1);
                        break;
                }

            }
            
            return null;
        }

        public static object Operate(Instructions op, dynamic a, dynamic b)
        {
            switch (op)
            {
                case Instructions.GREATER:
                    return a > b;
                case Instructions.GREATER_EQUAL:
                    return a >= b;
                case Instructions.LESSER:
                    return a < b;
                case Instructions.LESSER_EQUAL:
                    return a <= b;
                case Instructions.EQUAL:
                    return a == b;
                case Instructions.ADD:
                case Instructions.APPEND:
                    return a + b;
                case Instructions.SUB:
                    return a - b;
                case Instructions.DIV:
                    return a / b;
                case Instructions.MUL:
                    return a / b;
                case Instructions.NOT_EQUAL:
                    return a != b;
                case Instructions.OR:
                    return a || b;
                default:
                    throw new Exception("unknown operator " + op);
            }
        }
    }

   public class DataValue
    {
        protected string Name;
        protected Types Type;
        protected Object[] Value;
        protected int Dimentions = 0;
        protected Dictionary<string, string> Properties;
        protected Dictionary<string, DataValue> Subfields;

        public void SetArray(int Count)
        {
            this.Dimentions = Count;
        }

        public string GetName() => this.Name;

        public void Set(object value, int index = 0)
        {
            this.Value[index] = value;
        }

        public object Get(int index = -0)
        {
            if (this.Type == Types.Structure)
                return this.Subfields;
            else if (index >= 0)
                return this.Value;
            else
                return this.Value[0];
        }
    }

    class Character : DataValue
    {
        public Character(string name, int length, string initialValue = "")
        {
            this.Name = name;
            this.Type = Types.Character;
            this.Properties = new Dictionary<string, string>();
            this.Properties.Add("length", length.ToString());

            this.Dimentions = 1;
            this.Value = new object[this.Dimentions];
        }
        
        public new void Set(object value, int index = 0)
        {
            string NewValue = (string)value;
            int Length = int.Parse(this.Properties["length"]);

            if (NewValue.Length > Length)
                value = NewValue.Substring(0, Length);
            else
                value = NewValue.PadRight(Length);
            
            this.Value[index] = NewValue;
        }
    }
}