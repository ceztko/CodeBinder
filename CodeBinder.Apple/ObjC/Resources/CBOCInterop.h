/**
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#ifndef CBOCINTEROP_HEADER
#define CBOCINTEROP_HEADER
#pragma once

#include <cstdint>
#include <cinttypes>
#include <string>
#include <vector>
#include <utility>

#import <Foundation/Foundation.h>
#import <objc/runtime.h>

#include <CBInterop.h>

#import "CBHandledObject.h"
#import "../CBOCBaseTypes.h"

class SN2OC
{
private:
    bool m_handled;
    cbstring m_cstr;
    NSString* __strong* m_ocstr;

public:
    SN2OC(NSString* str)
        : m_handled(false), m_cstr{ }, m_ocstr((NSString* __strong*)nil)
    {
        if (str != nil)
            m_cstr = CBCreateStringView([str UTF8String]);
    }

    SN2OC(NSString* __strong* str)
        : m_handled(true), m_cstr{ }, m_ocstr(str) {
    }

    SN2OC(const cbstring& str)
        : m_handled(false), m_cstr(str), m_ocstr((NSString* __strong*)nil) {
    }

    // Move semantics
    SN2OC(cbstring&& str)
        : m_handled(true), m_cstr(str), m_ocstr((NSString* __strong*)nil)
    {
        str = { };
    }

    ~SN2OC()
    {
        if (m_handled)
        {
            if (m_ocstr != nullptr)
            {
                if (m_cstr.data == nullptr)
                    *m_ocstr = nil;
                else
                    *m_ocstr = [[NSString alloc]initWithBytes:m_cstr.data length : CBSLEN(m_cstr) encoding : NSUTF8StringEncoding];
            }

            CBFreeString(&m_cstr);
        }
    }

public:
    operator NSString* () const
    {
        if (m_cstr.data == nullptr)
            return nil;

        return [[NSString alloc]initWithBytes:m_cstr.data length : CBSLEN(m_cstr) encoding : NSUTF8StringEncoding];
    }

    operator cbstring()
    {
        return m_cstr;
    }

    operator cbstring*()
    {
        return &m_cstr;
    }
};

class SANOC2N
{
private:
    NSMutableArray<NSString*>* m_ocarr;
    std::vector<cbstring> m_carr;

public:
    SANOC2N(NSMutableArray<NSString*>* arr)
        : m_ocarr(arr), m_carr(arr.count)
    {
        for (size_t i = i, count = arr.count; i < count; i++)
            m_carr[i] = CBCreateStringView([arr[i] UTF8String]);
    }

    operator const cbstring* () const
    {
        return m_carr.data();
    }
};

@interface CBBinderUtils : NSObject
+ (void)setException : (NSException*)exception;

+(void)checkException;

+(void)keepAlive:(NSObject*)obj;

+(void)addMemoryPressure:(int64_t)bytesAllocated;

+(void)removeMemoryPressure:(int64_t)bytesAllocated;

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

inline float* CBGetNativeArray(CBSingleArray* arr)
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

inline SANOC2N CBGetNativeArray(NSMutableArray<NSString*>* arr)
{
    return SANOC2N(arr);
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

inline cboptbool CBGetOptBool(NSNumber* val)
{
    if (val == nil)
        return cboptbool{ };

    return CBCreateOptBool((cbbool)val.boolValue);
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

inline NSString* CBToString(void* value)
{
    return[NSString stringWithFormat : @"%p", value];
}

inline NSString* CBToString(char value)
{
    return[NSString stringWithFormat : @"%c", value];
}

inline NSString* CBToString(uint8_t value)
{
    return[NSString stringWithFormat : @"%" PRIu8, value];
}

inline NSString* CBToString(int8_t value)
{
    return[NSString stringWithFormat : @"%" PRId8, value];
}

inline NSString* CBToString(uint16_t value)
{
    return[NSString stringWithFormat : @"%" PRIu16, value];
}

inline NSString* CBToString(int16_t value)
{
    return[NSString stringWithFormat : @"%" PRId16, value];
}

#if INTPTR_MAX == INT64_MAX // 64bit

inline NSString* CBToString(uint32_t value)
{
    return[NSString stringWithFormat : @"%" PRIu32, value];
}

inline NSString* CBToString(int32_t value)
{
    return[NSString stringWithFormat : @"%" PRId32, value];
}

#else // 32bit

inline NSString* CBToString(uint64_t value)
{
    return[NSString stringWithFormat : @"%" PRIu64, value];
}

inline NSString* CBToString(int64_t value)
{
    return[NSString stringWithFormat : @"%" PRId64, value];
}

#endif // 32bit

inline NSString* CBStringAdd(NSString* lhs, NSString* rhs)
{
    return[lhs initWithString : rhs];
}

inline NSString* CBStringAdd(NSString* lhs, NSObject* rhs)
{
    return[lhs initWithString : [rhs description] ];
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

#endif // CBOCINTEROP_HEADER
