/*
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT
 */
package CodeBinder;

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
