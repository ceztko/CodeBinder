/*
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT
 */
package CodeBinder;

// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.handleref 
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
}
