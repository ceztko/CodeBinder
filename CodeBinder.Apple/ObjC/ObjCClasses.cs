using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    static class ObjCClasses
    {
        public const string CBException_h = @"#ifndef CB_EXCPETION
#define CB_EXCPETION
#pragma once

#import <Foundation/Foundation.h>

// Substitute for .NET Excpetion
@interface CBException : NSException
- (id)init;
- (id)init:(NSString *)message;
@end

#endif // CB_EXCPETION
";

        public const string CBException_mm = @"#import ""CBException.h""

@implementation CBException

- (id)init
{
    self = [super init];
    if (self == nil)
        return nil;

    return self;
}
- (id)init:(NSString *)message
{
    self = [super initWithName:@""Exception"" reason:message userInfo:nil];
    if (self == nil)
        return nil;

    return self;
}
@end";

        public const string CBIReadOnlyList_h = @"#ifndef CB_IREADONLYLIST
#define CB_IREADONLYLIST
#pragma once

// Substitute for .NET IReadOnlyList
@protocol CBIReadOnlyList
@end

#endif // CB_IREADONLYLIST
";

        public const string CBIEqualityCompararer_h = @"#ifndef CB_IEQUALITYCOMPARARER
#define CB_IEQUALITYCOMPARARER
#pragma once

// Substitute for .NET IReadOnlyList
@protocol CBIEqualityCompararer
@end

#endif // CB_IEQUALITYCOMPARARER
";

        public const string CBIDisposable_h = @"#ifndef CB_IDISPOSABLE
#define CB_IDISPOSABLE

// Substitute for .NET IDisposable
@protocol CBIDisposable
- (void)dispose;
@end

#endif // CB_IDISPOSABLE
";

        public const string CBKeyValuePair_h =
@"#ifndef CB_KEYVALUEPAIR
#define CB_KEYVALUEPAIR
#pragma once

#import <Foundation/Foundation.h>

// Substitute for .NET KeyValuePair
@interface CBKeyValuePair<__covariant KeyType, __covariant ValueType> : NSObject
{
@private
    KeyType _key;
    ValueType _value;
}

@property (nonatomic,readonly) KeyType key;
@property (nonatomic,readonly) ValueType value;

- (id)init:(KeyType)key :(ValueType)value;
- (KeyType)key;
- (ValueType)value;
@end

#endif // CB_KEYVALUEPAIR
";

        public const string CBKeyValuePair_mm =
@"#import ""CBKeyValuePair.h""

@implementation CBKeyValuePair

- (id)init:(id)key :(id)value
{
    self = [super init];
    if (self == nil)
        return nil;

    _key = key;
    _value = value;
    return self;
}

- (id)key
{
    return _key;
}

- (id)value
{
    return _value;
}
@end
";

        public const string CBHandleRef_h =
@"#ifndef CB_HANDLEREF
#define CB_HANDLEREF
#pragma once

#import <Foundation/Foundation.h>

// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.handleref
@interface CBHandleRef : NSObject
{
    @private
    NSObject * _wrapper;
    void * _handle;
}

@property (nonatomic,readonly) NSObject * wrapper;
@property (nonatomic,readonly) void * handle;

- (id)init;
- (id)init:(NSObject *)wrapper :(void *)handle;
- (NSObject *)wrapper;
- (void *)handle;
@end

#endif // CB_HANDLEREF
";

        public const string CBHandleRef_mm =
@"#import ""CBHandleRef.h""

@implementation CBHandleRef

- (id)init
{
    self = [super init];
    if (self == nil)
        return nil;

    _wrapper = nil;
    _handle = 0;
    return self;
}

- (id)init:(NSObject *)wrapper :(void *)handle;
{
    self = [super init];
    if (self == nil)
        return nil;

    _wrapper = wrapper;
    _handle = handle;
    return self;
}

- (NSObject *)wrapper
{
    return _wrapper;
}

- (void *)handle
{
    return _handle;
}

@end
";

        public const string CBBinderUtils_h =
@"#ifndef CB_BINDERUTILS
#define CB_BINDERUTILS
#pragma once

#import <Foundation/Foundation.h>
#import ""../CBOCBaseTypes.h""

inline void * CBUtilsGetNativeHandle(CBHandleRef *handle)
{
    if (handle == nil)
        return nullptr;

    return handle.handle;
}

inline void * CBUtilsGetNativeHandle(void *handle)
{
    return handle;
}

inline NSUInteger * CBUtilsGetNativeArray(CBNSUIntegerArray *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline NSInteger * CBUtilsGetNativeArray(CBNSIntegerArray *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline void ** CBUtilsGetNativeArray(CBPtrArray *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline BOOL * CBUtilsGetNativeArray(CBBoolArray *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline char * CBUtilsGetNativeArray(CBCharArray *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline uint8_t * CBUtilsGetNativeArray(CBUInt8Array *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline int8_t * CBUtilsGetNativeArray(CBInt8Array *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline uint16_t * CBUtilsGetNativeArray(CBUInt16Array *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline int16_t * CBUtilsGetNativeArray(CBInt16Array *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline uint32_t * CBUtilsGetNativeArray(CBUInt32Array *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline int32_t * CBUtilsGetNativeArray(CBInt32Array *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline uint64_t * CBUtilsGetNativeArray(CBUInt64Array *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline int64_t * CBUtilsGetNativeArray(CBInt64Array *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline float * CBUtilsGetNativeArray(CBFloatArray *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline double * CBUtilsGetNativeArray(CBDoubleArray *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline NSString * __strong * CBUtilsGetNativeArray(CBStringArray *arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline NSUInteger CBUtilsGetHashCode(NSObject *obj)
{
    return obj.hash;
}

inline NSUInteger CBUtilsGetHashCode(int32_t value)
{
    return value;
}

inline NSUInteger CBUtilsGetHashCode(uint32_t value)
{
    return value;
}

inline NSUInteger CBUtilsGetHashCode(void *ptr)
{
    return (NSUInteger)ptr;
}

inline NSString * CBBinderStringAdd(NSString *lhs, NSObject *rhs)
{
    return [lhs initWithString:[rhs description]];
}

inline BOOL CBBinderStringEqual(NSString *lhs, NSString *rhs)
{
    if (lhs == nil)
    {
        if (rhs == nil)
            return YES;
        else
            return NO;
    }
    else
    {
        return [lhs isEqualToString:rhs];
    }
}

inline BOOL CBBinderStringNotEqual(NSString *lhs, NSString *rhs)
{
    if (lhs == nil)
    {
        if (rhs == nil)
            return NO;
        else
            return YES;
    }
    else
    {
        return ![lhs isEqualToString:rhs];
    }
}

template<typename T>
T * CBBinderAsOperator(NSObject *obj)
{
    if (obj == nil)
        return nil;
	
	if ([obj isKindOfClass:[T class]])
		return (T *)obj;
	else
		return nil;
}

template<typename T>
T * CBBinderCastOperator(NSObject *obj)
{
    if (obj == nil)
        return nil;
	
	if ([obj isKindOfClass:[T class]])
		return (T *)obj;
	else
		@throw [NSException exceptionWithName:@""InvalidCastException"" reason:nil userInfo:nil];
}

#endif // CB_BINDERUTILS
";
    }
}
