using System;
using System.Diagnostics;

namespace CodeBinder.Attributes
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
}
