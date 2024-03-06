// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using System.Runtime.InteropServices;

namespace CodeBinder;

public static class BinderUtils
{
    static ConditionalWeakTable<object, object> _finalizationRegistry;

    [ThreadStatic]
    static Exception? _exception;

    static BinderUtils()
    {
        _finalizationRegistry = new ConditionalWeakTable<object, object>();
    }

    public static void CheckException()
    {
        if (_exception != null)
        {
            var exception = _exception;
            _exception = null;
            throw exception;
        }
    }

    public static void SetException(Exception exception)
    {
        _exception = exception;
    }

    public static NativeHandle CreateNativeHandle(object obj)
    {
        return new NativeHandle(GCHandle.Alloc(obj, GCHandleType.Pinned));
    }

    public static NativeHandle CreateWeakNativeHandle(object obj)
    {
        return new NativeHandle(GCHandle.Alloc(obj, GCHandleType.Weak));
    }

    public static void KeepAlive(object obj)
    {
        GC.KeepAlive(obj);
    }

    public static void FreeNativeHandle(ref NativeHandle nativeHandle)
    {
        nativeHandle.Handle.Free();
    }

    internal static void RegisterForFinalization(object finalizable, IObjectFinalizer finalizer)
    {
        _finalizationRegistry.Add(finalizable, finalizer);
    }
}

public struct NativeHandle
{
    internal GCHandle Handle { get; private set; }

    internal NativeHandle(GCHandle handle)
    {
        Handle = handle;
    }

    public IntPtr Address => GCHandle.ToIntPtr(Handle);

    public object Target => Handle.Target;
}
