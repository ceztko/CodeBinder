#import "CBHandledObject.h"
#import "../Internal/CBHandledObject.h"
#import "../Internal/CBOCBinderUtils.h"
#import "CBException.h"

@implementation CBHandledObjectFinalizer
    - (id)init
    {
        self = [super init];
        if (self == nil)
            return nil;
        return self;
    }

    -(void)dealloc
    {
        [self freeHandle:_handle];
    }

    - (void)freeHandle:(void*)handle
    {
        @throw [[CBException alloc]init];
    }

    -(long)handle
    {
        return _handle;
    }

    -(void)setHandle:(long)value
    {
        _handle = value;
    }
@end

@implementation CBFinalizableObject
    - (id)init
    {
        self = [super init];
        if (self == nil)
            return nil;

        _finalizers = [[NSMutableArray alloc] init];
        return self;
    }


    - (void)registerFinalizer :(IObjectFinalizer*)finalizer
    {
        [_finalizers addObject:finalizer];
    }
@end

@implementation CBHandledObjectBase
    - (id)init:(void*)handle :(BOOL)handled
    {
        self = [super init];
        if (self == nil)
            return nil;

        _handle = handle;
        if (handled)
        {
            CBHandledObjectFinalizer* finalizer = [self createFinalizer];
            finalizer.handle = handle;
            [self registerFinalizer:finalizer];
        }

        return self;
    }

    -(CBHandledObjectFinalizer*)createFinalizer
    {
        @throw[NSException exceptionWithName:@"Not implemented" reason:nil userInfo:nil];
    }

    - (void *)unsafeHandle
    {
        return _handle;
    }

    - (CBHandleRef*)handle
    {
        return [[CBHandleRef alloc]init:self :_handle];
    }

    - (BOOL)isEqualTo:(NSObject*)obj
    {
        if (obj == (NSObject*)nil)
            return NO;
        CBHandledObjectBase* other = CBAsOperator<CBHandledObjectBase>(obj);
        return self.referenceHandle == other.referenceHandle;
    }

    - (NSUInteger)hash
    {
        return CBGetHashCode(self.referenceHandle);
    }

    - (void*)referenceHandle
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

    - (id)init:(void*)handle :(BOOL)handled
    {
        self = [super init:handle :handled];
        if (self == nil)
            return nil;

        return self;
    }
@end
