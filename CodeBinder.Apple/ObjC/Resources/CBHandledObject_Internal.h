#ifndef CBHANDLEDOBJECT_INTERNAL_HEADER
#define CBHANDLEDOBJECT_INTERNAL_HEADER
#pragma once

@interface CBHandledObjectFinalizer ()
    - (id)init;

    -(void)dealloc;

    -(void)freeHandle:(void*)handle;

    @property(nonatomic) long handle;
    -(long)handle;
    -(void)setHandle:(long)value;
@end

@interface CBFinalizableObject ()
- (id)init;

- (void)registerFinalizer :(IObjectFinalizer*)finalizer;
@end

@interface CBHandledObjectBase ()
    - (id)init:(void*)handle :(BOOL)handled;

    -(CBHandledObjectFinalizer*)createFinalizer;

    @property(nonatomic,readonly) void* referenceHandle;
    - (void *)referenceHandle;
@end

@interface CBHandledObject ()
    - (id)init;

    - (id)init:(void*)handle :(BOOL)handled;
@end

#endif // CBHANDLEDOBJECT_INTERNAL_HEADER
