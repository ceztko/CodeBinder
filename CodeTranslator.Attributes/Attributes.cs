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
    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public sealed class RequiresAttribute : Attribute
    {
        private string[] _policies;

        public RequiresAttribute(params string[] policies)
        {
            _policies = policies;
        }
    }

    public static class Policies
    {
        public const string OperatorOverloading = "{B19FE437-C7FE-41A5-841F-D9170CCEEBA1}";
        public const string ReifiedGenerics = "{E3D821B4-562B-4C9F-8753-1E9C6F4D93A1}";
        public const string Yield = "{6D4647AE-CA4C-49F0-8BE6-138512992E21}";
        public const string NETFramework = "{5DD7ECEA-0F59-4781-BCA2-1DC916EB3CFC}";
        public const string ValueTypes = "{5DD7ECEA-0F59-4781-BCA2-1DC916EB3CFC}";
        public const string PassByRef = "{5B8D618C-93E8-46F2-85FE-DE28FEE86196}";
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
            if (_parameterTypes != null)
                return MethodSignature.Create(MethodName, ReturnType, _parameterTypes);
            else if (_parameterObjs != null)
                return MethodSignature.Create(MethodName, ReturnType, _parameterObjs);
            else
                return MethodSignature.Create();
        }
    }
}
