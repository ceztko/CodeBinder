// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Java;

static class JavaClasses
{
    public const string BinderUtils =
@"import java.lang.reflect.*;

public class BinderUtils
{
    static Object _cleaner;
    static Method _register;

    static
    {
        String versionStr = System.getProperty(""java.specification.version"");
        if (versionStr.startsWith(""1."") || versionStr.startsWith(""0.""))
            versionStr = versionStr.substring(2);

        int version = Integer.parseInt(versionStr);
        if (version >= 9)
        {
            try
            {
                Class cleanerClass = Class.forName(""java.lang.ref.Cleaner"");
                Method create = cleanerClass.getMethod(""create"");
                _register = cleanerClass.getDeclaredMethod(""register"", Object.class, Runnable.class);
                _cleaner = create.invoke(null);
            }
            catch (ClassNotFoundException ex)
            {
                // Do nothing. ""java.lang.ref.Cleaner"" is not avaiable
            }
            catch (InvocationTargetException | IllegalAccessException |
                   NoSuchMethodException ex)
            {
                System.err.println(ex);
                throw new RuntimeException(ex);
            }
        }
    }

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

    public static boolean isCleanerAvaiable()
    {
        return _cleaner != null;
    }

    public static NativeHandle createNativeHandle(Object obj)
    {
        return new NativeHandle(newGlobalRef(obj), false);
    }

    public static NativeHandle createWeakNativeHandle(Object obj)
    {
        return new NativeHandle(newGlobalWeakRef(obj), true);
    }

    public static void freeNativeHandle(NativeHandle nativeHandle)
    {
        if (nativeHandle.weak)
            deleteGlobalWeakRef(nativeHandle.handle);
        else
            deleteGlobalRef(nativeHandle.handle);
    }

    static void registerForFinalization(Object obj, IObjectFinalizer finalizer)
    {
        try
        {
            _register.invoke(_cleaner, obj, finalizer);
        }
        catch (InvocationTargetException | IllegalAccessException ex)
        {
            System.err.println(ex);
            throw new RuntimeException(ex);
        }
    }

    static native long newGlobalRef(Object obj);
    static native void deleteGlobalRef(long globalref);
    static native long newGlobalWeakRef(Object obj);
    static native void deleteGlobalWeakRef(long globalref);
    static native Object getGlobalRefTarget(long handle);
    static native Object getGlobalWeakRefTarget(long handle);
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

    public Object getWrapper()
    {
        return this.wrapper;
    }

    public long getHandle()
    {
        return this.handle;
    }
}";

    public const string NativeHandle = """
public class NativeHandle
{
    long handle;
    boolean weak;

    NativeHandle(long handle, boolean weak)
    {
        this.handle = handle;
        this.weak = weak;
    }

    public long getAddress()
    {
        return handle;
    }

    public Object getTarget()
    {
        if (weak)
            return BinderUtils.getGlobalWeakRefTarget(handle);
        else
            return BinderUtils.getGlobalRefTarget(handle);
    }
}
""";

    public const string FinalizableObject = @"import java.util.*;

public abstract class FinalizableObject
{
    ArrayList<IObjectFinalizer> _finalizers;

    protected FinalizableObject()
    {
    }

    protected void registerFinalizer(IObjectFinalizer finalizer)
    {
        if (BinderUtils.isCleanerAvaiable())
        {
            BinderUtils.registerForFinalization(this, finalizer);
        }
        else
        {
            if (_finalizers == null)
                _finalizers = new ArrayList<IObjectFinalizer>();

            _finalizers.add(finalizer);
        }
    }
}
";

    public const string HandledObjectBase =
@"import java.util.*;

public class HandledObjectBase extends FinalizableObject
{
    long _handle;

    protected HandledObjectBase(long handle, boolean handled)
    {
        _handle = handle;
        if (handled)
        {
            HandledObjectFinalizer finalizer = createFinalizer();
            finalizer.handle = handle;
            registerFinalizer(finalizer);
        }
    }

    protected HandledObjectFinalizer createFinalizer()
    {
        throw new UnsupportedOperationException(""The finalizer must be supplied"");
    }

    public long getUnsafeHandle()
    {
        return _handle;
    }

    public HandleRef getHandle()
    {
        return new HandleRef(this, _handle);
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
    protected HandledObject(long handle, boolean handled)
    {
        super(handle, handled);
    }

    public boolean equals(BaseT other)
    {
        return super.equals(other);
    }
}";


    //// TODO: alternatively generate finalize() or run()
    public const string HandledObjectFinalizer = """
import java.util.*;

public abstract class HandledObjectFinalizer implements IObjectFinalizer
{
    long handle;

    // For retrocompatibility
    protected void finalize() throws Throwable
    {
        if (!BinderUtils.isCleanerAvaiable())
            freeHandle(handle);
    }

    public void run()
    {
        freeHandle(handle);
    }

    public abstract void freeHandle(long handle);
}
""";

    //// TODO: alternatively inherits Runnable or not
    public const string IObjectFinalizer = """
import java.util.*;

public interface IObjectFinalizer extends Runnable
{
}
""";
}
