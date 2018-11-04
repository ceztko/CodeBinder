using System;
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
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
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
            setParameter(ref ret.ReturnParam, ReturnType, null);

            if (_parameterTypes != null)
            {
                ret.HaveArgsName = false;
                ret.Parameters = new ParameterData[_parameterTypes.Length];
                for (int i = 0; i < _parameterTypes.Length; i++)
                    setParameter(ref ret.Parameters[i], _parameterTypes[i], "arg" + i);
            }
            else if (_parameterObjs != null)
            {
                if (_parameterObjs.Length % 2 != 0)
                    throw new Exception();

                ret.HaveArgsName = true;
                int parameterCount = _parameterObjs.Length / 2;
                ret.Parameters = new ParameterData[parameterCount];
                for (int i = 0; i < parameterCount; i++)
                    setParameter(ref ret.Parameters[i], _parameterObjs[i * 2 + 0], _parameterObjs[i * 2 + 1].ToString());
            }
            else
            {
                ret.HaveArgsName = true;
                ret.Parameters = new ParameterData[0];
            }

            return ret;
        }

        private void setParameter(ref ParameterData parameter, object parameterType, string parameterName)
        {
            if (parameterType == null)
            {
                parameter.Type = typeof(void);
            }
            else
            {
                var type = parameterType as Type;
                if (type == null)
                    parameter.CustomType = parameterType.ToString();
                else
                    parameter.Type = type;
            }
            parameter.Name = parameterName;
        }
    }

    public class MethodData
    {
        public bool HaveArgsName;
        public string Name;
        public ParameterData ReturnParam;
        public ParameterData[] Parameters;
    }

    public struct ParameterData
    {
        public string Name;
        public Type Type;
        public string CustomType;
    }
}
