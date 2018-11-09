using System;
using sim = System.ValueTuple;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeTranslator.Shared;

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
        public MethodSignature GetMethodSignature()
        {
            var ret = new MethodSignature();
            ret.Name = MethodName;
            ret.ReturnParam.Set(ReturnType, null);

            if (_parameterTypes != null)
            {
                ret.HasExplictParamNames = false;
                ret.Parameters = new ParameterData[_parameterTypes.Length];
                for (int i = 0; i < _parameterTypes.Length; i++)
                    ret.Parameters[i].Set(_parameterTypes[i], "param" + i);
            }
            else if (_parameterObjs != null)
            {
                if (_parameterObjs.Length % 2 != 0)
                    throw new Exception("Parameter objects count must be divisible by two");

                ret.HasExplictParamNames = true;
                int parameterCount = _parameterObjs.Length / 2;
                ret.Parameters = new ParameterData[parameterCount];
                for (int i = 0; i < parameterCount; i++)
                    ret.Parameters[i].Set(_parameterObjs[i * 2 + 0], _parameterObjs[i * 2 + 1].ToString());
            }
            else
            {
                ret.HasExplictParamNames = true;
                ret.Parameters = new ParameterData[0];
            }

            return ret;
        }
    }
}
