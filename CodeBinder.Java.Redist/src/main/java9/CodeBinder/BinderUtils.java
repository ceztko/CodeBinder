/*
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT
 */
package CodeBinder;

import java.lang.ref.Cleaner;

// JDK9+/Android (API 33+) implementation: unconditionally uses
// "java.lang.ref.Cleaner". It must keep the very same member signatures as the
// JDK8 baseline implementation it overlays in the multi-release jar.
public class BinderUtils
{
    static final Cleaner _cleaner = Cleaner.create();
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
        return true;
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
        _cleaner.register(obj, finalizer);
    }

    static native long newGlobalRef(Object obj);
    static native void deleteGlobalRef(long globalref);
    static native long newGlobalWeakRef(Object obj);
    static native void deleteGlobalWeakRef(long globalref);
    static native Object getGlobalRefTarget(long handle);
    static native Object getGlobalWeakRefTarget(long handle);
}
