#ifndef CODE_BINDER_BASE_TYPES
#define CODE_BINDER_BASE_TYPES
#pragma once

#ifdef __cplusplus
#include <cstdint>
#include <cstdlib>
#include <climits>
#else // __cplusplus
#include <stdint.h>
#include <stdlib.h>
#include <limits.h>
#endif // __cplusplus

#ifdef __APPLE__
#include <objc/objc.h>
#endif

#if defined(__cplusplus) && defined(_MSC_VER)
// In MSVC bool is guaranteed to be 1 byte, with true == 1 and false == 0
typedef bool CBBool;
#elif defined(__APPLE__)
typedef BOOL CBBool;
#else // Others
typedef signed char CBBool;
#endif

// TODO: Should be fixed for big endian, with ownsdata being first
typedef struct
{
    const char* data;
    size_t length : sizeof(uintptr_t)* CHAR_BIT - 1;
    unsigned ownsdata : 1;
} cbstring;

#ifdef __cplusplus
#define cbstringnull cbstring{ }
#else // __cplusplus
#define cbstringnull (const cbstring){ 0 }
#endif // __cplusplus

#define cbstringp cbstring
#define cbstringr cbstring

#endif // CODE_BINDER_BASE_TYPES
