/**
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

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

    -(void*)handle
    {
        return _handle;
    }

    -(void)setHandle:(void*)value
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

        return self;
    }

    - (void)registerFinalizer :(id<CBIObjectFinalizer>)finalizer
    {
        if (_finalizers == nil)
            _finalizers = [[NSMutableArray alloc] init];

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
        _handled = handled;
        return self;
    }

    - (void)dealloc
    {
        if (_handled)
            [self freeHandle:_handle];
    }

    - (void)freeHandle:(void *)handle
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
