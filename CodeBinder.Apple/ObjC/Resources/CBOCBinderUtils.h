/**
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#ifndef CB_BINDERUTILS
#define CB_BINDERUTILS
#pragma once

#import "../CBOCBaseTypes.h"
#include <cstdint>
#include <cinttypes>
#import <Foundation/Foundation.h>

OCENLIBPDF_API @interface CBBinderUtils : NSObject
+(void)setException:(NSException*)exception;

+(void)checkException;

+(void)keepAlive:(NSObject*)obj;

@end

inline void* CBGetNativeHandle(CBHandleRef* handle)
{
    if (handle == nil)
        return nullptr;

    return handle.handle;
}

inline void* CBGetNativeHandle(void* handle)
{
    return handle;
}

inline NSUInteger* CBGetNativeArray(CBNSUIntegerArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline NSInteger* CBGetNativeArray(CBNSIntegerArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline void** CBGetNativeArray(CBPtrArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline BOOL* CBGetNativeArray(CBBoolArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline char* CBGetNativeArray(CBCharArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline uint8_t* CBGetNativeArray(CBUInt8Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline int8_t* CBGetNativeArray(CBInt8Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline uint16_t* CBGetNativeArray(CBUInt16Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline int16_t* CBGetNativeArray(CBInt16Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline uint32_t* CBGetNativeArray(CBUInt32Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline int32_t* CBGetNativeArray(CBInt32Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline uint64_t* CBGetNativeArray(CBUInt64Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline int64_t* CBGetNativeArray(CBInt64Array* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline float* CBGetNativeArray(CBFloatArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline double* CBGetNativeArray(CBDoubleArray* arr)
{
    if (arr == nil)
        return nullptr;

    return arr.data;
}

inline NSUInteger CBGetHashCode(NSObject* obj)
{
    return obj.hash;
}

inline NSUInteger CBGetHashCode(int32_t value)
{
    return value;
}

inline NSUInteger CBGetHashCode(uint32_t value)
{
    return value;
}

inline NSUInteger CBGetHashCode(void* ptr)
{
    return (NSUInteger)ptr;
}

inline NSString* CBToString(NSUInteger value)
{
#if INTPTR_MAX == INT64_MAX // 64 bit
    return[NSString stringWithFormat : @"%lu", value];
#else // 32 bit
    return[NSString stringWithFormat : @"%u", value];
#endif
}

inline NSString* CBToString(NSInteger value)
{
#if INTPTR_MAX == INT64_MAX // 64 bit
    return[NSString stringWithFormat : @"%ld", value];
#else // 32 bit
    return[NSString stringWithFormat : @"%d", value];
#endif
}

inline NSString* CBToString(void * value)
{
    return [NSString stringWithFormat :@"%p", value];
}

inline NSString* CBToString(char value)
{
    return [NSString stringWithFormat :@"%c", value];
}

inline NSString* CBToString(uint8_t value)
{
    return [NSString stringWithFormat :@"%" PRIu8, value];
}

inline NSString* CBToString(int8_t value)
{
    return [NSString stringWithFormat :@"%" PRId8, value];
}

inline NSString* CBToString(uint16_t value)
{
    return [NSString stringWithFormat :@"%" PRIu16, value];
}

inline NSString* CBToString(int16_t value)
{
    return [NSString stringWithFormat :@"%" PRId16, value];
}

#if INTPTR_MAX == INT64_MAX // 64bit

inline NSString* CBToString(uint32_t value)
{
    return [NSString stringWithFormat :@"%" PRIu32, value];
}

inline NSString* CBToString(int32_t value)
{
    return [NSString stringWithFormat :@"%" PRId32, value];
}

#else // 32bit

inline NSString* CBToString(uint64_t value)
{
    return [NSString stringWithFormat :@"%" PRIu64, value];
}

inline NSString* CBToString(int64_t value)
{
    return [NSString stringWithFormat :@"%" PRId64, value];
}

#endif // 32bit

inline NSString* CBStringAdd(NSString* lhs, NSString* rhs)
{
    return[lhs initWithString :rhs];
}

inline NSString* CBStringAdd(NSString* lhs, NSObject* rhs)
{
    return[lhs initWithString :[rhs description]];
}

inline BOOL CBStringEqual(NSString* lhs, NSString* rhs)
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

inline BOOL CBStringNotEqual(NSString* lhs, NSString* rhs)
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
BOOL CBIsInstanceOf(NSObject* obj)
{
    if (obj == nil)
        return NO;

    if ([obj isKindOfClass : [T class] ])
        return YES;
    else
        return NO;
}

template<typename T>
T* CBAsOperator(NSObject* obj)
{
    if (obj == nil)
        return nil;

    if ([obj isKindOfClass : [T class] ])
        return (T*)obj;
    else
        return nil;
}

template<typename T>
T* CBCastOperator(NSObject* obj)
{
    if (obj == nil)
        return nil;

    if ([obj isKindOfClass : [T class] ])
        return (T*)obj;
    else
        @throw[NSException exceptionWithName : @"InvalidCastException" reason:nil userInfo : nil];
}

#endif // CB_BINDERUTILS
