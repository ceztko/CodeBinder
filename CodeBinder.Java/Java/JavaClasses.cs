// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
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

    // TODO: Consider moving this methods to generation of exising .NET class BinderUtils.
    // See CodeBinder.Redist
    public static native long newGlobalRef(Object obj);
    public static native void deleteGlobalRef(long globalref);
    public static native long newGlobalWeakRef(Object obj);
    public static native void deleteGlobalWeakRef(long globalref);
}";
        // https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.handleref
        public const string HandleRef =
@"// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.handleref
public class HandleRef
{
    public final Object wrapper;
    public final long handle;

    public HandleRef()
    {
        this.wrapper = null;
        this.handle = 0;
    }

    public HandleRef(Object wrapper, long handle)
    {
        this.wrapper = wrapper;
        this.handle = handle;
    }
}";
        public const string HandledObjectBase =
@"import java.util.*;

public class HandledObjectBase
{
    long _handle;

    protected HandledObjectBase()
    {
        _handle = 0;
    }

    protected HandledObjectBase(long handle)
    {
        _handle = handle;
    }

    protected void finalize() throws Throwable
    {
        if (getManaged())
            freeHandle(_handle);
        super.finalize();
    }

    protected void setHandle(long handle)
    {
        if (handle == 0)
            throw new RuntimeException(""Handle must be non null"");
        if (_handle != 0)
            throw new RuntimeException(""Handle is already set"");
        _handle = handle;
    }

    protected void freeHandle(long handle)
    {
        throw new UnsupportedOperationException();
    }

    public long getUnsafeHandle()
    {
        return _handle;
    }

    public HandleRef getHandle()
    {
        return new HandleRef(this, _handle);
    }

    public boolean getManaged()
    {
        return true;
    }
    
    public boolean equals(Object obj)
    {
        if (obj == null)
            return false;
        HandledObjectBase other = BinderUtils.as(obj, HandledObjectBase.class);
        return this.getReferenceHandle() == other.getReferenceHandle();
    }
    
    public boolean equals(HandledObjectBase obj)
    {
        if (obj == null)
            return false;
        
        return this.getReferenceHandle() == obj.getReferenceHandle();
    } 

    public int hashCode()
    {
        return ((Long)getReferenceHandle()).hashCode();
    }

    protected long getReferenceHandle()
    {
        return getUnsafeHandle();
    }
}";

        public const string HandledObject =
@"import java.util.*;

public class HandledObject <BaseT extends HandledObject<BaseT>> extends HandledObjectBase
{
    protected HandledObject()
    {
    }

    protected HandledObject(long handle)
    {
        super(handle);
    }

    public boolean equals(BaseT other)
    {
        return super.equals(other);
    }
}";
    }
}
