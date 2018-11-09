using System;
using sim = System.ValueTuple;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeTranslator.Attributes
{
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    // TODO: Remove me
    [Obsolete]
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public sealed class NamespaceAttribute: Attribute
    {
        public NamespaceAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    // TODO: Remove me
    [Obsolete]
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ImportAttribute : Attribute
    {
        public ImportAttribute(string import)
        {
            ImportName = import;
        }

        public string ImportName { get; private set; }
    }

    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public OrderAttribute([CallerLineNumber]int order = 0)
        {
            Order = order;
        }
    }

    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public sealed class IgnoreAttribute : Attribute
    {

    }

    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class SignatureAttribute : Attribute
    {
        private Type[] _parameterTypes;
        private object[] _parameterObjs;

        public string MethodName { get; set; }
        public object ReturnType { get; set; }

        public SignatureAttribute() { }

        public SignatureAttribute(params Type[] parameterTypes)
        {
            _parameterTypes = parameterTypes;
        }

        public SignatureAttribute(params object[] parameters)
        {
            _parameterObjs = parameters;
        }

        /// <summary>Returs parameters. Also includes return type int the first entry)</summary>
        public MethodData GetMethodData()
        {
            var ret = new MethodData();
            ret.Name = MethodName;
            ret.ReturnParam.Set(ReturnType, null);

            if (_parameterTypes != null)
            {
                ret.HasExplictParamsName = false;
                ret.Parameters = new ParameterData[_parameterTypes.Length];
                for (int i = 0; i < _parameterTypes.Length; i++)
                    ret.Parameters[i].Set(_parameterTypes[i], "param" + i);
            }
            else if (_parameterObjs != null)
            {
                if (_parameterObjs.Length % 2 != 0)
                    throw new Exception("Parameter objects count must be divisible by two");

                ret.HasExplictParamsName = true;
                int parameterCount = _parameterObjs.Length / 2;
                ret.Parameters = new ParameterData[parameterCount];
                for (int i = 0; i < parameterCount; i++)
                    ret.Parameters[i].Set(_parameterObjs[i * 2 + 0], _parameterObjs[i * 2 + 1].ToString());
            }
            else
            {
                ret.HasExplictParamsName = true;
                ret.Parameters = new ParameterData[0];
            }

            return ret;
        }
    }

    public class MethodData
    {
        public bool HasExplictParamsName;
        public string Name;
        public ParameterData ReturnParam;
        public ParameterData[] Parameters;

        /// <param name="hasExplictParamsName"></param>
        public MethodData CreateFromParamStrings(string methodName, string returnType, IReadOnlyList<string> paramStrings, bool hasExplictParamsName = true)
        {
            var ret = new MethodData();
            ret.Name = methodName;
            ret.HasExplictParamsName = hasExplictParamsName;
            ret.ReturnParam.TypeName = returnType;

            var parameters = new List<ParameterData>();
            foreach (var parameter in getParameters(paramStrings, hasExplictParamsName))
                parameters.Add(new ParameterData() { Name = parameter.Name, TypeName = parameter.TypeName });

            ret.Parameters = parameters.ToArray();
            return ret;
        }

        IEnumerable<(string TypeName, string Name)> getParameters(IReadOnlyList<string> paramStrings, bool hasExplictParamsName)
        {
            if (hasExplictParamsName)
            {
                if (paramStrings.Count % 2 != 0)
                    throw new Exception("String count with parameters name must be divisible by two");

                int paramCount = paramStrings.Count / 2;
                for (int i = 0; i < paramCount; i++)
                    yield return (paramStrings[i * 2], paramStrings[i * 2 + 1]);
            }
            else
            {
                for (int i = 0; i < paramStrings.Count; i++)
                    yield return (paramStrings[i], "param" + (i + 1));
            }
        }
    }

    public struct ParameterData
    {
        Type _Type;
        string _TypeName;

        public string Name;

        public void Set(object typeObj, string name)
        {
            if (typeObj == null)
            {
                Type = null;
            }
            else
            {
                var type = typeObj as Type;
                if (type == null)
                    TypeName = typeObj.ToString();
                else
                    Type = type;
            }

            Name = name;
        }

        public void Set(string type, string name)
        {
            TypeName = type;
            Name = name;
        }

        public void Set(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public Type Type
        {
            get
            {
                if (_TypeName != null)
                    throw new Exception("Explicit type name set");
                else if (_Type != null)
                    return _Type;
                else
                    return typeof(void);
            }
            set
            {
                _Type = value;
                _TypeName = null;
            }
        }
        public string TypeName
        {
            get
            {
                if (_TypeName != null)
                    return _TypeName;
                else if (_Type != null)
                    return _Type.FullName;
                else
                    return typeof(void).FullName;
            }
            set
            {
                _TypeName = value;
                _Type = null;
            }
        }

        public bool HasExplicitTypeName
        {
            get { return _TypeName != null; }
        }
    }
}
