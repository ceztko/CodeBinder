/*
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT
 */
package CodeBinder;

// JDK8 baseline implementation: "java.lang.ref.Cleaner" doesn't exist here, so
// finalization falls back to Object.finalize() (see HandledObjectFinalizer).
// The JDK9+ implementation is overlaid at "META-INF/versions/9" in the
// multi-release jar and must keep the very same member signatures.
public class BinderUtils
{
    static final ThreadLocal<RuntimeException> _exception = new ThreadLocal<RuntimeException>();

    public static void checkException()
    {
        RuntimeException exception = _exception.get();
        if (exception != null)
        {
            _exception.remove();
            throw exception;
        }
    }

    public static void setException(RuntimeException exception)
    {
        _exception.set(exception);
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
        return false;
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

    public static void keepAlive(Object obj)
    {
        if (obj == null)
            throw new IllegalArgumentException();
    }

    public static void addMemoryPressure(long bytesAllocated)
    {
        // Do nothing, there's no equivalent in Java
    }

    public static void removeMemoryPressure(long bytesAllocated)
    {
        // Do nothing, there's no equivalent in Java
    }

    static void registerForFinalization(Object obj, IObjectFinalizer finalizer)
    {
        // Unreachable: callers guard on isCleanerAvaiable()
        throw new UnsupportedOperationException("java.lang.ref.Cleaner is not available on this runtime");
    }

    static native long newGlobalRef(Object obj);
    static native void deleteGlobalRef(long globalref);
    static native long newGlobalWeakRef(Object obj);
    static native void deleteGlobalWeakRef(long globalref);
    static native Object getGlobalRefTarget(long handle);
    static native Object getGlobalWeakRefTarget(long handle);
}
