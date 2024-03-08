/**
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#ifndef CBHANDLEDOBJECT_HEADER
#define CBHANDLEDOBJECT_HEADER
#pragma once

#import "../objclibdefs.h"
#import <Foundation/Foundation.h>
#import "CBHandleRef.h"

OCLIBRARY_MACRO @protocol CBIObjectFinalizer
@end

OCLIBRARY_MACRO @interface CBHandledObjectFinalizer : NSObject<CBIObjectFinalizer>
{
    @private void* _handle;
}
@end

OCLIBRARY_MACRO @interface CBFinalizableObject : NSObject
{
    @private NSMutableArray* _finalizers;
}
@end

OCLIBRARY_MACRO @interface CBHandledObjectBase : CBFinalizableObject
{
    @private void* _handle;
    @private BOOL _handled;
}
    - (BOOL)isEqualTo:(NSObject*)obj;

    - (NSUInteger)hash;

    @property(nonatomic,readonly) void* unsafeHandle;
    - (void*)unsafeHandle;

    @property(nonatomic,readonly) CBHandleRef* handle;
    - (CBHandleRef*)handle;
@end

@class CBHandledObject;

OCLIBRARY_MACRO @interface CBHandledObject <BaseT : CBHandledObjectBase*> : CBHandledObjectBase
@end

#endif // CBHANDLEDOBJECT_HEADER
