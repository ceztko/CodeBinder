#ifndef CBHANDLEDOBJECT_HEADER
#define CBHANDLEDOBJECT_HEADER
#pragma once

#import "cboclibdefs.h"
#import <Foundation/Foundation.h>
#import "CBHandleRef.h"

OBJC_CODEBINDER_API @interface CBHandledObjectBase : NSObject
{
    @private void * _handle;
}
    - (BOOL)isEqualTo:(NSObject *)obj;

    - (NSUInteger)hash;

    @property(nonatomic,readonly) void * unsafeHandle;
    - (void *)unsafeHandle;

    @property(nonatomic,readonly) CBHandleRef * handle;
    - (CBHandleRef *)handle;

    @property(nonatomic,readonly) BOOL managed;
    - (BOOL)managed;
@end

@class CBHandledObject;

OBJC_CODEBINDER_API @interface CBHandledObject <BaseT : CBHandledObject *> : CBHandledObjectBase
@end

#endif // CBHANDLEDOBJECT_HEADER
