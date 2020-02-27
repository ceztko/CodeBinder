#ifndef CODE_BINDER_BASE_TYPES
#define CODE_BINDER_BASE_TYPES

#ifdef __cplusplus
#include <cstdint>
#else // __cplusplus
#include <stdint.h>
#ifdef __APPLE__
#include <Foundation/NSString.h>
typedef unichar char16_t;
#else // __APPLE__
#include <uchar.h>
#endif // __APPLE__
#endif // __cplusplus

#define BBool signed char

#endif // CODE_BINDER_BASE_TYPES