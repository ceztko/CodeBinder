/*
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT
 */
package CodeBinder;

import java.util.*;

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
}
