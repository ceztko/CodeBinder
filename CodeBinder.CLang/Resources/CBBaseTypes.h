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
typedef bool cbbool;
#elif defined(__APPLE__)
typedef BOOL cbbool;
#else // Others
typedef signed char cbbool;
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

#endif // CODE_BINDER_BASE_TYPES
