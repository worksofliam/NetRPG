using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetRPG.Runtime.Functions
{
    class Function
    {
        private String[] FunctionAliases = { };

        //TODO: only add items to this list if it's used
        private static Dictionary<string, Function> List = new Dictionary<string, Function>
        {
        };

        public static void AddFunctionReference(string functionIdentifier, string rpgFunctionName)
        {
            Function result = null;

            functionIdentifier = functionIdentifier.ToUpper();

            result = CreateFunction(rpgFunctionName);

            if (result == null)
            {
                Error.ThrowCompileError("Function '" + rpgFunctionName + "' does not exist in system.");
            }
            else
            {
                if (!List.ContainsKey(functionIdentifier))
                    List.Add(functionIdentifier, result);
            }
        }

        private static Function CreateFunction(string rpgFunctionName)
        {
            List<Type> types = new List<Type>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                // Check if it's a class from the function namespace
                if (type == null || type.Namespace == null || !type.Namespace.StartsWith("NetRPG.Runtime.Functions") || type.BaseType != typeof(Function))
                {
                    continue;
                }

                // If it's a build-in function, then add % as prefix to the classname
                string prefix = type.Namespace.StartsWith("NetRPG.Runtime.Functions.BIF") ? "%" : "";

                // Normalize RPG function and typename to find the correct function type
                string normalizedFuncName = rpgFunctionName.ToUpper();
                string normalizedTypeName = prefix + type.Name.ToUpper();

                // Compare RPG function name with name of type and return instance of rpg function implementation if found
                if (normalizedFuncName == normalizedTypeName)
                {
                    Function function = (Function)Activator.CreateInstance(type);
                    return function;
                }

                // Check for alias
                foreach (Attribute attribute in type.GetCustomAttributes())
                {
                    // If the current attribute is not a RPG function alias, then give next attribute
                    if (attribute.GetType() != typeof(RPGFunctionAlias)) continue;

                    RPGFunctionAlias rpgFunctionAlias = (RPGFunctionAlias)attribute;

                    // Normalize RPG function alias name for comparisation
                    string normalizedRPGFunctionAlias = rpgFunctionAlias.Alias.ToUpper();

                    // Compare RPG function name with name of alias and return instance of rpg function implementation if found
                    if (normalizedRPGFunctionAlias == normalizedFuncName)
                    {
                        Function function = (Function)Activator.CreateInstance(type);
                        return function;
                    }
                }
            }

            return null;
        }

        public static bool IsFunction(string name)
        {
            return List.ContainsKey(name.ToUpper());
        }

        public static Function GetFunction(string name)
        {
            name = name.ToUpper();

            if (List.ContainsKey(name))
                return List[name];
            else
                return null; //TODO: throw error
        }

        public virtual object Execute(object[] Parameters)
        {
            return null;
        }
    }
}
