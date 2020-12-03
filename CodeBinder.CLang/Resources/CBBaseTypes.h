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

#if UINTPTR_MAX == UINT32_MAX
#define CB_STRING_OWNSDATA_FLAG (1u << 31)
#elif UINTPTR_MAX == UINT64_MAX
#define CB_STRING_OWNSDATA_FLAG (1ull << 63)
#else
#error "Environment not 32 or 64-bit."
#endif

#if defined(__cplusplus) && defined(_MSC_VER)
// In MSVC bool is guaranteed to be 1 byte, with true == 1 and false == 0
typedef bool CBBool;
#elif defined(__APPLE__)
typedef BOOL CBBool;
#else // Others
typedef signed char CBBool;
#endif

typedef struct
{
    const char* data;
    uintptr_t opaque;
} cbstring;

#ifdef __cplusplus
#define cbstringnull cbstring{ }
#else // __cplusplus
#define cbstringnull (const cbstring){ NULL, 0 }
#endif // __cplusplus

#define cbstringp cbstring
#define cbstringr cbstring

#endif // CODE_BINDER_BASE_TYPES
