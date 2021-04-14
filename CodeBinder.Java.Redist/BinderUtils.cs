using System;
using System.Runtime.InteropServices;

namespace CodeBinder.Java
{
    public static class BinderUtils
    {
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        public static extern IntPtr NewGlobalRef(object obj);

        public static extern void DeleteGlobalRef(IntPtr globalref);

        public static extern IntPtr NewGlobalWeakRef(object obj);

        public static extern void DeleteGlobalWeakRef(IntPtr globalref);
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it
    }
}
