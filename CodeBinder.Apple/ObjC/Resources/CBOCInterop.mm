/**
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#import "CBOCInterop.h"

namespace
{
    struct ThreadLocalException
    {
        NSException* Exception;
    };
}

thread_local ThreadLocalException s_exception;

@implementation CBBinderUtils

    +(void)setException:(NSException*)exception
    {
        s_exception.Exception = exception;
    }

    +(void)checkException
    {
        if (s_exception.Exception != Nil)
        {
            NSException* exception = s_exception.Exception;
            s_exception.Exception = Nil;
            @throw exception;
        }
    }

    +(void)keepAlive:(NSObject*)obj;
    {
        // Do nothing
    }

    +(void)addMemoryPressure:(int64_t)bytesAllocated;
    {
        // Do nothing, there's no equivalent in Objective-C
    }

    +(void)removeMemoryPressure:(int64_t)bytesAllocated;
    {
        // Do nothing, there's no equivalent in Objective-C
    }
@end
