/*
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT
 */
package CodeBinder;

import java.util.*;

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
        throw new UnsupportedOperationException("The finalizer must be supplied");
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
}
