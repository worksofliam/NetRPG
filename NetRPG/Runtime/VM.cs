using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NetRPG.Runtime.Typing;
using NetRPG.Runtime.Functions;

namespace NetRPG.Runtime
{
    class RunTimeModule {

        public Dictionary<string, DataValue> GlobalVariables;
        public RunTimeModule() {
            GlobalVariables = new Dictionary<string, DataValue>();
        }
    }

    public class VM
    {
        private bool IsTestingEnv;
        private string _EntryProcedure;
        private Dictionary<string, RunTimeModule> RunTimeModules;
        private Boolean _DisplayRequired;
        private Dictionary<string, Procedure> _Procedures;

        public VM(bool testingVM = false)
        {
            IsTestingEnv = testingVM;
            _EntryProcedure = "";
            _Procedures = new Dictionary<string, Procedure>();
            RunTimeModules = new Dictionary<string, RunTimeModule>();
        }

        public void AddModule(Module module)
        {
            string ModuleName = RunTimeModules.Count.ToString();
            //Handle return types, displays and entry point
            foreach (Procedure proc in module.GetProcedures())
            {
                proc._ParentModule = ModuleName;

                if (proc._ReturnType == Types.Void)
                    proc._ReturnType = Types.Pointer; //Any

                if (module._HasDisplay)
                    _DisplayRequired = true;

                _Procedures.Add(proc.GetName(), proc);
                if (proc.HasEntrypoint) _EntryProcedure = proc.GetName();

                proc.CalculateLabels();
            }

            //Handle adding references to internal functions
            foreach (string function in module.GetReferences()) {
                Function.AddFunctionReference(function, module.GetReferenceFunc(function));
            } 

            //Handle global variables for module
            RunTimeModules.Add(ModuleName, new RunTimeModule());

            //Handle shared memory between variables
            Dictionary<string, DataValue> SharedMemory = new Dictionary<string, DataValue>();
            
            foreach (String global in module.GetDataSetList())
            {
                //TODO: don't just compare names, also compare types for shared memory

                DataValue set;
                if (SharedMemory.ContainsKey(global)) {
                    set = SharedMemory[global];
                } else {
                    set = module.GetDataSet(global).ToDataValue();
                    SharedMemory.Add(global, set);
                }

                RunTimeModules[ModuleName].GlobalVariables.Add(set.GetName(), set);

                if (set is Structure) {
                    if (!(set as Structure).isQualified()) {
                        foreach (string column in (set as Structure).GetSubfieldNames()) {
                            if (SharedMemory.ContainsKey(column))
                                (set as Structure).SetData(SharedMemory[column], column);
                            else
                                SharedMemory.Add(column, (set as Structure).GetData(column));
                        }
                    }
                }
            }
        }

        private List<string> CallStack;
        public object Run()
        {
            if (_DisplayRequired)
                WindowHandler.Init();
            CallStack = new List<string>();
            try {
                return Execute(_EntryProcedure);
            } catch (Exception e) {
                Console.WriteLine("-- Error --");
                Console.WriteLine(e.Message);
                Console.WriteLine("RPG call stack: ");
                foreach(string item in CallStack) {
                    Console.WriteLine("\t" + item);
                }
                Console.WriteLine(".NET call stack:");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("-- Error --");
                Console.WriteLine();
                PrintModules();
                if (_DisplayRequired)
                    Console.ReadLine();
                return null;
            } finally {
                if (_DisplayRequired)
                    WindowHandler.End();
            }
        }

        public void PrintModules() {
            Console.WriteLine("--- Procedure instructions ---");
            foreach (Procedure proc in _Procedures.Values) {
                Console.WriteLine();
                Console.WriteLine("\t - " + proc.GetName());
                Console.WriteLine();
                foreach (Instruction inst in proc.GetInstructions()) {
                    Console.WriteLine("\t" + inst._Instruction.ToString().PadRight(10) + inst._Value);
                }
                Console.WriteLine();
            }
        }

        private object Execute(string Name, object[] Parms = null)
        {
            Function callingFunction;
            DataValue tempDataValue;
            object[] tempArray;
            object[] Values = new object[3];
            int tempIndex = 1;
            DataValue set;

            List<object> Stack = new List<object>();

            Dictionary<string, int> Labels = new Dictionary<string, int>();
            Dictionary<string, DataValue> LocalVariables = new Dictionary<string, DataValue>();
            Procedure currentProcedure = _Procedures[Name];
            Instruction[] instructions = currentProcedure.GetInstructions();

            string ModuleName = currentProcedure._ParentModule;
            
            CallStack.Add(Name);

            //Initialise local variables
            foreach (string local in currentProcedure.GetDataSetList())
            {
                set = currentProcedure.GetDataSet(local).ToDataValue();
                LocalVariables.Add(set.GetName(), set);
                LocalVariables[set.GetName()].DoInitialValue();
            }

            if (Parms != null)
            {
                string[] Parameters = currentProcedure.GetParameterNames();
                for (int x = 0; x < Parameters.Length; x++)
                {
                    if (x < Parms.Length)
                    {
                        if (Parms[x] is DataValue)
                        {
                            if (currentProcedure.ParameterIsValue(Parameters[x]))
                            {
                                LocalVariables[Parameters[x]].SetEntire((Parms[x] as DataValue).GetEntire());
                            }
                            else
                                LocalVariables[Parameters[x]] = (DataValue)Parms[x];
                        }
                        else
                        {
                            LocalVariables[Parameters[x]].Set(Parms[x]);
                        }
                    }
                    else
                    {
                        LocalVariables[Parameters[x]].SetNull();
                    }
                }
            }

            for (int ip = 0; ip < instructions.Count(); ip++)
            {
                switch (instructions[ip]._Instruction)
                {
                    case Instructions.APPEND:
                    case Instructions.ADD:
                    case Instructions.SUB:
                    case Instructions.DIV:
                    case Instructions.MUL:
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
                        ip = currentProcedure.GetLabel(instructions[ip]._Value);
                        break;

                    case Instructions.BRFALSE:
                        Values[0] = Stack[Stack.Count - 1];
                        Stack.RemoveRange(Stack.Count - 1, 1);
                        if ((bool) Operate(Instructions.EQUAL, Values[0], false))
                            ip = currentProcedure.GetLabel(instructions[ip]._Value);
                        break;

                    case Instructions.BRTRUE:
                        Values[0] = Stack[Stack.Count - 1];
                        Stack.RemoveRange(Stack.Count - 1, 1);
                        if ((bool)Operate(Instructions.EQUAL, Values[0], true))
                            ip = currentProcedure.GetLabel(instructions[ip]._Value);
                        break;

                    case Instructions.CALL:
                        tempIndex = (int)Stack[Stack.Count - 1];
                        if (_Procedures.ContainsKey(instructions[ip]._Value))
                        {
                            Values[0] = Execute(instructions[ip]._Value, Stack.GetRange(Stack.Count - (tempIndex + 1), tempIndex).ToArray());
                        }
                        else if (Function.IsFunction(instructions[ip]._Value))
                        {
                            callingFunction = Function.GetFunction(instructions[ip]._Value);
                            Values[0] = callingFunction.Execute(Stack.GetRange(Stack.Count - (tempIndex + 1), tempIndex).ToArray());
                        }
                        else
                        {
                            Error.ThrowRuntimeError(Name, "Function " + instructions[ip]._Value + " does not exist.", ip);
                        }
                        
                        Stack.RemoveRange(Stack.Count - (tempIndex + 1), tempIndex + 1); //Remove parameters from stack

                        if (Values[0] != null)
                            Stack.Add(Values[0]);
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
                        Stack.Add(RunTimeModules[ModuleName].GlobalVariables[instructions[ip]._Value].Get());
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

                    case Instructions.LDNULL:
                        Stack.Add(null);
                        break;

                    case Instructions.LDGBLD:
                        Stack.Add(RunTimeModules[ModuleName].GlobalVariables[instructions[ip]._Value]);
                        break;

                    case Instructions.LDVARD:
                        Stack.Add(LocalVariables[instructions[ip]._Value]);
                        break;

                    case Instructions.LDARRD: //Only really used to get a array subfield
                        tempDataValue = (DataValue) Stack[Stack.Count - 2];
                        Values[1] = Stack[Stack.Count - 1];

                        tempIndex = int.Parse(Values[1].ToString()) - 1;
                        Stack.RemoveRange(Stack.Count - 2, 2);
                        Values[0] = tempDataValue.Get(tempIndex);
                        Stack.Add(Values[0]);
                        break;

                    case Instructions.LDFLDD: //Load subfield data
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

                    case Instructions.CRTARR:
                        Values[0] = Stack[Stack.Count - 1];
                        List<dynamic> resultArray = new List<dynamic>();

                        if (Values[0] is int)
                            for (int i = 1; i <= Convert.ToInt32(Values[0]); i++)
                                resultArray.Add(Stack[Stack.Count - 1 - i]);

                        tempIndex = Convert.ToInt32(Values[0])+1;
                        Stack.RemoveRange(Stack.Count - tempIndex, tempIndex);
                        Stack.Add(resultArray.ToArray());
                        break;
                        
                    case Instructions.NOT:
                        Values[0] = Stack[Stack.Count - 1];
                        Stack.RemoveRange(Stack.Count - 1, 1);
                        Stack.Add(!(bool)Values[0]);
                        break;

                    case Instructions.RETURN:
                        CallStack.RemoveAt(CallStack.Count-1);
                        if (currentProcedure._ReturnType == Types.Void)
                            return null;
                        else
                        {
                            if (Stack.Count() >= 1) {
                                Values[0] = Stack[Stack.Count - 1];
                                return Values[0];
                            } else {
                                return null;
                            }
                        }

                    case Instructions.STORE:
                        
                        //Stack.Count - 3 is DateValue (usually)
                        Values[0] = Stack[Stack.Count - 2]; //Index of fieldname
                        Values[1] = Stack[Stack.Count - 1]; //Value

                        if (Values[0] is int) //Storing value into array
                        {
                            tempDataValue = (DataValue)Stack[Stack.Count - 3]; //DataValue
                            tempIndex = int.Parse(Values[0].ToString()) - 1;
                            tempDataValue.Set(Values[1], tempIndex);
                            Stack.RemoveRange(Stack.Count - 3, 3);
                        }
                        else if (Values[0] is string) //Storing value into field?
                        {
                            tempDataValue = (Structure)Stack[Stack.Count - 3]; //DataValue
                            tempDataValue.Set(Values[1], Values[0].ToString());
                            Stack.RemoveRange(Stack.Count - 3, 3);
                        }
                        else //Story value into DataValue
                        {
                            tempDataValue = (DataValue)Stack[Stack.Count - 2]; //DataValue
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
            
            CallStack.RemoveAt(CallStack.Count-1);
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
                    if (a is string)
                        a = a.Trim();
                    if (b is string)
                        b = b.Trim();

                    if (a is bool)
                        a = ((bool)a ? "1" : "0");
                    if (b is bool)
                        b = ((bool)b ? "1" : "0");

                    return a == b;
                case Instructions.ADD:
                case Instructions.APPEND:
                    return a + b;
                case Instructions.SUB:
                    return a - b;
                case Instructions.DIV:
                    return a / b;
                case Instructions.MUL:
                    return a * b;
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