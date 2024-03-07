/**
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#import "CBOCBinderUtils.h"

@implementation CBBinderUtils

    +(void)setException:(NSException*)exception
    {
        @throw exception;
    }

    +(void)checkException
    {
        // Do nothing
    }

    +(void)keepAlive:(NSObject*)obj;
    {
        // Do nothing
    }
@end
