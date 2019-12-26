using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeBinder.Attributes
{
    public class CodeBinderAttribute : Attribute
    {
        public const string ConditionString = "CODE_BINDER";

        internal CodeBinderAttribute() { }
    }

    [Conditional(ConditionString)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ModuleAttribute : CodeBinderAttribute
    {
        public ModuleAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    [Conditional(ConditionString)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ImportAttribute : CodeBinderAttribute
    {
        public ImportAttribute(string import)
        {
            ImportName = import;
        }

        public string ImportName { get; private set; }
    }

    [Conditional(ConditionString)]
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : CodeBinderAttribute
    {
        public int Order { get; private set; }

        public OrderAttribute([CallerLineNumber]int order = 0)
        {
            Order = order;
        }
    }

    [Conditional(ConditionString)]
    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class IgnoreAttribute : CodeBinderAttribute
    {

    }

    [Conditional(ConditionString)]
    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RequiresAttribute : CodeBinderAttribute
    {
        private string[] _policies;

        public RequiresAttribute(params string[] policies)
        {
            _policies = policies;
        }
    }

    /// <summary>
    /// Inerit this class to create a binder of a parameter/return value type to a C type
    /// </summary>
    [Conditional(ConditionString)]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class ParameterBinderAttribute : CodeBinderAttribute
    {
        protected ParameterBinderAttribute() { }
    }

    /// <summary>
    /// Attribute to pass interop string parameter
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PString
    {
        public static implicit operator string(PString pstr)
        {
            return pstr.String;
        }

        public static implicit operator PString(string str)
        {
            return new PString(str);
        }

        public PString(string str)
        {
            String = str;
            Lenght = new IntPtr(str.Length);
        }

        [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)]
        public string String;
        public IntPtr Lenght;
    }

    /// <summary>
    /// Attribute to return interop string
    /// </summary>
    /// <remarks>We need a separate struct for return values
    /// because PString is not blittable. We do manual manual marshalling</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct RString : IDisposable
    {
        public static implicit operator string(RString rstr)
        {
            int length = rstr.Lenght.ToInt32();
            if (rstr.String == IntPtr.Zero)
                return null;

            return Marshal.PtrToStringUni(rstr.String, length);
        }

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(String);
            String = IntPtr.Zero;
        }

        public IntPtr String;

        // Length is IntPtr so size is variable depdending on address size
        public IntPtr Lenght;
    }

    public static class Policies
    {
        public const string Delegates = "{0CA8BB17-A589-41A9-A97C-0E1870837AB4}";
        public const string OperatorOverloading = "{B19FE437-C7FE-41A5-841F-D9170CCEEBA1}";
        public const string ReifiedGenerics = "{E3D821B4-562B-4C9F-8753-1E9C6F4D93A1}";
        public const string YieldReturn = "{6D4647AE-CA4C-49F0-8BE6-138512992E21}";
        /// <summary>
        /// Represent functionalities tipically present in all .NET base libraries
        /// </summary>
        public const string DotNet = "{162BC1C4-BEEA-4E30-A525-E084E528232C}";
        /// <summary>
        /// Represent functionalities tipically present only with .NET Framework
        /// </summary>
        public const string NETFramework = "{5DD7ECEA-0F59-4781-BCA2-1DC916EB3CFC}";
        public const string ValueTypes = "{5DD7ECEA-0F59-4781-BCA2-1DC916EB3CFC}";
        public const string PassByRef = "{5B8D618C-93E8-46F2-85FE-DE28FEE86196}";
        public const string ExplicitInterfaceImplementation = "E0B93847-8DF4-4DE8-B7C8-7F002410EC76";
    }
}
