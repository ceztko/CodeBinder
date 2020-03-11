#ifndef CB_BINDERUTILS
#define CB_BINDERUTILS
#pragma once

#import <Foundation/Foundation.h>
#import ""../CBOCBaseTypes.h""

inline void* CBUtilsGetNativeHandle(CBHandleRef* handle)
{
    if (handle == nil)
        return nullptr;

    return handle.handle;
}

inline void* CBUtilsGetNativeHandle(void* handle)
{
    return handle;
}

inline NSUInteger* CBUtilsGetNativeArray(CBNSUIntegerArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline NSInteger* CBUtilsGetNativeArray(CBNSIntegerArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline void** CBUtilsGetNativeArray(CBPtrArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline BOOL* CBUtilsGetNativeArray(CBBoolArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline char* CBUtilsGetNativeArray(CBCharArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline uint8_t* CBUtilsGetNativeArray(CBUInt8Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline int8_t* CBUtilsGetNativeArray(CBInt8Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline uint16_t* CBUtilsGetNativeArray(CBUInt16Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline int16_t* CBUtilsGetNativeArray(CBInt16Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline uint32_t* CBUtilsGetNativeArray(CBUInt32Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline int32_t* CBUtilsGetNativeArray(CBInt32Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline uint64_t* CBUtilsGetNativeArray(CBUInt64Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline int64_t* CBUtilsGetNativeArray(CBInt64Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline float* CBUtilsGetNativeArray(CBFloatArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline double* CBUtilsGetNativeArray(CBDoubleArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline NSString* __strong* CBUtilsGetNativeArray(CBStringArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.values;
}

inline NSUInteger CBUtilsGetHashCode(NSObject* obj)
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

inline NSUInteger CBUtilsGetHashCode(void* ptr)
{
    return (NSUInteger)ptr;
}

inline NSString* CBBinderStringAdd(NSString* lhs, NSObject* rhs)
{
    return[lhs initWithString : [rhs description] ];
}

inline BOOL CBBinderStringEqual(NSString* lhs, NSString* rhs)
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
        return[lhs isEqualToString : rhs];
    }
}

inline BOOL CBBinderStringNotEqual(NSString* lhs, NSString* rhs)
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
        return ![lhs isEqualToString : rhs];
    }
}

template<typename T>
T* CBBinderAsOperator(NSObject* obj)
{
    if (obj == nil)
        return nil;

    if ([obj isKindOfClass : [T class] ])
        return (T*)obj;
    else
        return nil;
}

template<typename T>
T* CBBinderCastOperator(NSObject* obj)
{
    if (obj == nil)
        return nil;

    if ([obj isKindOfClass : [T class] ])
        return (T*)obj;
    else
        @throw[NSException exceptionWithName : @""InvalidCastException"" reason:nil userInfo : nil];
}

#endif // CB_BINDERUTILS