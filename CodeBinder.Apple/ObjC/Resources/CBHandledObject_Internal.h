#ifndef CBHANDLEDOBJECT_INTERNAL_HEADER
#define CBHANDLEDOBJECT_INTERNAL_HEADER
#pragma once

@interface CBHandledObjectBase ()
    - (id)init;

    - (id)init:(void *)handle;

    - (void)dealloc;

    - (void)setHandle:(void *)handle;

    - (void)freeHandle:(void *)handle;

    @property(nonatomic,readonly) void * referenceHandle;
    - (void *)referenceHandle;
@end

@interface CBHandledObject ()
    - (id)init;

    - (id)init:(void *)handle;
@end

#endif // CBHANDLEDOBJECT_INTERNAL_HEADER
