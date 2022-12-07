using System;

namespace CodeBinder.Java
{
    public static class BinderUtils
    {
        public static IntPtr NewGlobalRef(object obj)
        {
            _ = obj;
            return default;
        }

        public static void DeleteGlobalRef(IntPtr globalref)
        {
            _ = globalref;
        }

        public static IntPtr NewGlobalWeakRef(object obj)
        {
            _ = obj;
            return default;
        }

        public static void DeleteGlobalWeakRef(IntPtr globalref)
        {
            _ = globalref;
        }
    }
}
