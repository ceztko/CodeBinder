// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder;

/// <summary>
/// An handle to a managed object that must be manually freed.
/// Useful in NAOT implementations
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe ref struct OpaqueHandle<T>
    where T : class
{
    GCHandle _handle;

    public OpaqueHandle(T obj)
    {
        _handle = GCHandle.Alloc(obj, GCHandleType.Normal);
    }

    /// <summary>
    /// Free the managed GCHAndle and the unmanaged memory
    /// </summary>
    public void Free()
    {
        _handle.Free();
        fixed (OpaqueHandle<T>* ptr = &this)
        {
            // Free the unmanaged block
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }

    /// <summary>
    /// Allocate an unamanged block of memory carrying the a GCHandle,
    /// returning it as a native pointer
    /// </summary>
    public OpaqueHandle<T>* Allocate()
    {
        var ret = Marshal.AllocHGlobal(sizeof(OpaqueHandle<T>));
        fixed (OpaqueHandle<T>* ptr = &this)
        {
            *(OpaqueHandle<T>*)ret = *ptr;
        }

        return (OpaqueHandle<T>*)ret;
    }

    public T Target => (T)_handle.Target;
}
