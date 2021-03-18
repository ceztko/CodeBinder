using CodeBinder.Attributes;
using System;
using System.Diagnostics;

namespace CodeBinder.Apple.Attributes
{
    /// <summary>ObjC Specific selector for the parameter</summary>
    /// <remarks>This attribute is not marked with Conditional since it
    /// may be useful for mapping outside of the assembly</remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class SelectorAttribute : CodeBinderAttribute
    {
        public string Selector { get; private set; }

        public SelectorAttribute(string selector)
        {
            Selector = selector;
        }
    }

    // Specify that the attribute is a C type (for example add "struct"
    // qualifier in the method signatures
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false)]
    public sealed class CLangTypeAttribute : CodeBinderAttribute
    {
        public CLangTypeAttribute() { }
    }
}
