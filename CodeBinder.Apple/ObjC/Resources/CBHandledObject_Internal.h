#ifndef CBHANDLEDOBJECT_INTERNAL_HEADER
#define CBHANDLEDOBJECT_INTERNAL_HEADER
#pragma once

#import "../Support/CBHandledObject.h"

@interface CBHandledObjectFinalizer ()
    - (id)init;

    -(void)dealloc;

    -(void)freeHandle:(void*)handle;

    @property(nonatomic) void* handle;
    -(void*)handle;
    -(void)setHandle:(void*)value;
@end

@interface CBFinalizableObject ()
- (id)init;

- (void)registerFinalizer :(id<CBIObjectFinalizer>)finalizer;
@end

@interface CBHandledObjectBase ()
    - (id)init:(void*)handle :(BOOL)handled;

    -(void)dealloc;

    -(void)freeHandle:(void*)handle;

    @property(nonatomic,readonly) void* referenceHandle;
    - (void *)referenceHandle;
@end

@interface CBHandledObject ()
    - (id)init;

    - (id)init:(void*)handle :(BOOL)handled;
@end

#endif // CBHANDLEDOBJECT_INTERNAL_HEADER
