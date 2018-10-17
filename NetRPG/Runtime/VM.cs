using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NetRPG.Runtime.Typing;

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
            Execute(_EntryProcedure);
        }

        private object Execute(string Name, DataValue[] Parms = null)
        {
            DataValue tempDataValue;
            object[] tempArray;
            object[] Values = new object[3];
            int tempIndex = 1;

            List<object> Stack = new List<object>();

            Dictionary<string, int> Labels = new Dictionary<string, int>();
            Dictionary<string, DataValue> LocalVariables = new Dictionary<string, DataValue>();
            Instruction[] instructions = _Procedures[Name].GetInstructions();
            
            //Initialise local variables
            foreach (string local in _Procedures[Name].GetDataSetList())
            {
                DataValue set = _Procedures[Name].GetDataSet(local).ToDataValue();
                LocalVariables.Add(set.GetName(), set);
                LocalVariables[set.GetName()].Set(_Procedures[Name].GetDataSet(local)._InitialValue);
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

                    case Instructions.LDARRV:
                        
                        Values[0] = Stack[Stack.Count - 2];
                        Values[1] = Stack[Stack.Count - 1];
                        tempIndex = int.Parse(Values[1].ToString()) - 1;

                        if (Values[0] is object[])
                        {
                            tempArray = (object[])Stack[Stack.Count - 2];

                            Stack.RemoveRange(Stack.Count - 2, 2);
                            Stack.Add(tempArray[tempIndex]);
                        }
                        else
                        {
                            tempDataValue = (DataValue)Stack[Stack.Count - 2];
                            tempArray = (object[]) tempDataValue.Get();
                            tempArray = (object[]) tempArray[tempIndex];

                            Stack.RemoveRange(Stack.Count - 2, 2);
                            Stack.Add(tempArray);
                        }
                        break;

                    case Instructions.LDFLDV:
                        Values[0] = Stack[Stack.Count - 1];

                        Stack.RemoveRange(Stack.Count - 1, 1);
                        if (Values[0] is DataValue[])
                        {
                            tempArray = (DataValue[])Values[0];
                            foreach (DataValue data in tempArray)
                            {
                                if (data.GetName() == instructions[ip]._Value)
                                {
                                    Stack.Add(data.Get());
                                    break;
                                }
                            }
                        } 
                        else if (Values[0] is DataValue)
                        {
                            tempDataValue = (DataValue) Values[0];
                            Stack.Add(tempDataValue.Get(instructions[ip]._Value));
                        }
                            
                        break;

                    case Instructions.LDGBLV:
                        Stack.Add(GlobalVariables[instructions[ip]._Value].Get());
                        break;

                    case Instructions.LDVARV:
                        Stack.Add(LocalVariables[instructions[ip]._Value].Get());
                        break;

                    case Instructions.LDINT:
                        Stack.Add(int.Parse(instructions[ip]._Value));
                        break;
                    case Instructions.LDDOU:
                        Stack.Add(double.Parse(instructions[ip]._Value));
                        break;

                    case Instructions.LDSTR:
                        Stack.Add(instructions[ip]._Value);
                        break;

                    case Instructions.LDGBLD:
                        Stack.Add(GlobalVariables[instructions[ip]._Value]);
                        break;

                    case Instructions.LDVARD:
                        Stack.Add(GlobalVariables[instructions[ip]._Value]);
                        break;

                    case Instructions.LDARRD: //Only really used to get a array subfield
                        tempDataValue = (DataValue) Stack[Stack.Count - 2];
                        Values[1] = Stack[Stack.Count - 1];

                        tempIndex = int.Parse(Values[1].ToString()) - 1;
                        Stack.RemoveRange(Stack.Count - 2, 2);
                        Values[0] = tempDataValue.Get(tempIndex);
                        Stack.Add(Values[0]);
                        break;

                    case Instructions.LDFLDD:
                        Values[0] = Stack[Stack.Count - 1]; //DataValue[]

                        if (Values[0] is object[])
                        {
                            tempArray = (object[]) Values[0];
                            Stack.RemoveRange(Stack.Count - 1, 1);

                            foreach (DataValue data in tempArray)
                            {
                                if (data.GetName() == instructions[ip]._Value)
                                {
                                    Stack.Add(data);
                                    break;
                                }
                            }
                        }
                        else if (Values[0] is DataValue)
                        {
                            tempDataValue = (DataValue)Values[0];
                            Stack.Add(tempDataValue.GetData(instructions[ip]._Value));
                        }
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

                    case Instructions.STORE:
                        
                        Values[0] = Stack[Stack.Count - 2];
                        Values[1] = Stack[Stack.Count - 1]; //Value

                        if (Values[0] is int) //TODO: Accept other numeric type?
                        {
                            tempDataValue = (DataValue)Stack[Stack.Count - 3]; //DataValue
                            tempIndex = int.Parse(Values[0].ToString()) - 1;
                            tempDataValue.Set(Values[1], tempIndex);
                            Stack.RemoveRange(Stack.Count - 3, 3);
                        }
                        else if (Values[0] is string)
                        {
                            tempDataValue = (Structure)Stack[Stack.Count - 3]; //DataValue
                            tempDataValue.Set(Values[1], Values[0].ToString());
                            Stack.RemoveRange(Stack.Count - 3, 3);
                        }
                        else
                        {
                            tempDataValue = (DataValue)Stack[Stack.Count - 2]; //DataValue OR index
                            tempDataValue.Set(Values[1]);
                            Stack.RemoveRange(Stack.Count - 2, 2);
                        }
                        
                        break;

                    case Instructions.ENTRYPOINT:
                    case Instructions.LABEL:
                        //Do nothing
                        break;
                    default:
                        Console.WriteLine("Unused instruction: " + instructions[ip]._Instruction.ToString());
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




}