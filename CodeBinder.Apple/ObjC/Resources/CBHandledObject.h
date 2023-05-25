#ifndef CBHANDLEDOBJECT_HEADER
#define CBHANDLEDOBJECT_HEADER
#pragma once

#import "cboclibdefs.h"
#import <Foundation/Foundation.h>
#import "CBHandleRef.h"

OBJC_CODEBINDER_API @protocol IObjectFinalizer
@end

OBJC_CODEBINDER_API @interface CBHandledObjectFinalizer : NSObject<IObjectFinalizer>
{
    @private void* _handle;
}
@end

OBJC_CODEBINDER_API @interface CBFinalizableObject : NSObject
{
    @private NSMutableArray* _finalizers;
}
@end

OBJC_CODEBINDER_API @interface CBHandledObjectBase : CBFinalizableObject
{
    @private void* _handle;
}
    - (BOOL)isEqualTo:(NSObject*)obj;

    - (NSUInteger)hash;

    @property(nonatomic,readonly) void* unsafeHandle;
    - (void*)unsafeHandle;

    @property(nonatomic,readonly) CBHandleRef* handle;
    - (CBHandleRef*)handle;
@end

@class CBHandledObject;

OBJC_CODEBINDER_API @interface CBHandledObject <BaseT : CBHandledObjectBase*> : CBHandledObjectBase
@end

#endif // CBHANDLEDOBJECT_HEADER
