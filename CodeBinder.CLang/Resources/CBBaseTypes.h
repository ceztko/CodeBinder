#ifndef CODE_BINDER_BASE_TYPES
#define CODE_BINDER_BASE_TYPES

#ifdef __cplusplus
#include <cstdint>
#else // __cplusplus
#include <stdint.h>
#ifndef __APPLE__
#include <uchar.h>
#endif // __APPLE__
#endif // __cplusplus

#if defined(__cplusplus) && defined(_MSC_VER)
// In MSVC bool is guaranteed to be 1 byte, with true == 1 and false == 0
typedef bool BBool;
#else
#define BBool signed char
#endif

#ifdef __APPLE__
typedef char cbchar_t;
#define CB_NULL_TERMINATION '\0'
#else // __APPLE__
typedef char16_t cbchar_t;
#define CB_NULL_TERMINATION u'\0'
#endif // __APPLE__

#define cbstring_t cbchar_t *

#endif // CODE_BINDER_BASE_TYPES
