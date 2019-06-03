using System;
using System.Runtime.InteropServices;

namespace CodeBinder
{
    public static class BinderUtils
    {
        [DllImport("ENLibPdf", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr NewGlobalRef(object obj);

        [DllImport("ENLibPdf", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void DeleteGlobalRef(IntPtr globalref);

        [DllImport("ENLibPdf", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr NewGlobalWeakRef(object obj);

        [DllImport("ENLibPdf", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void DeleteGlobalWeakRef(IntPtr globalref);
    }
}
