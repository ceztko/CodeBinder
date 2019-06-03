using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    static class JavaClasses
    {
        public const string BinderUtils =
@"public class BinderUtils
{
    // Simulates as operator https://stackoverflow.com/a/148949/213871
    public static <T> T as(Object obj, Class<T> clazz)
    {
        if (clazz.isInstance(obj))
            return clazz.cast(obj);

        return null;
    }

    public static boolean equals(String lhs, String rhs)
    {
        if (lhs == null)
        {
            if (rhs == null)
                return true;
            else
                return false;
        }
        else
        {
            return lhs.equals(rhs);
        }
    }
}
";
        // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.handleref
        public const string HandleRef =
@"public class HandleRef
{
    public final Object wrapper;
    public final long handle;

    // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.handleref
    public HandleRef(Object wrapper, long handle)
    {
        this.wrapper = wrapper;
        this.handle = handle;
    }
}";
    }
}
