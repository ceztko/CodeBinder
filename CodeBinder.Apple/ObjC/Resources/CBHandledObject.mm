#import "CBHandledObject.h"
#import "../Internal/CBHandledObject.h"
#import "../Internal/CBOCBinderUtils.h"
#import "CBException.h"

@implementation CBHandledObjectBase
    - (id)init
    {
        self = [super init];
        if (self == nil)
            return nil;
        _handle = NULL;
        return self;
    }

    - (id)init:(void *)handle
    {
        self = [super init];
        if (self == nil)
            return nil;
        _handle = handle;
        return self;
    }

    - (void)dealloc
    {
        if (self.managed)
            [self freeHandle:_handle];
    }

    - (void)setHandle:(void *)handle
    {
        if (handle == NULL)
            @throw [[CBException alloc]init:@"Handle must be non null"];
        if (_handle != NULL)
            @throw [[CBException alloc]init:@"Handle is already set"];
        _handle = handle;
    }

    - (void)freeHandle:(void *)handle
    {
        @throw [[CBException alloc]init];
    }

    - (void *)unsafeHandle
    {
        return _handle;
    }

    - (CBHandleRef *)handle
    {
        return [[CBHandleRef alloc]init:self :_handle];
    }

    - (BOOL)managed
    {
        return YES;
    }

    - (BOOL)isEqualTo:(NSObject *)obj
    {
        if (obj == (NSObject *)nil)
            return NO;
        CBHandledObjectBase * other = CBAsOperator<CBHandledObjectBase>(obj);
        return self.referenceHandle == other.referenceHandle;
    }

    - (NSUInteger)hash
    {
        return CBGetHashCode(self.referenceHandle);
    }

    - (void *)referenceHandle
    {
        return self.unsafeHandle;
    }
@end

@implementation CBHandledObject
    - (id)init
    {
        self = [super init];
        if (self == nil)
            return nil;

        return self;
    }

    - (id)init:(void *)handle
    {
        self = [super init:handle];
        if (self == nil)
            return nil;

        return self;
    }
@end
