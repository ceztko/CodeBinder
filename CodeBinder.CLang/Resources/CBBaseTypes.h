/**
 * SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

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

#ifdef __cplusplus
 // In all modern compilers bool is guaranteed to be 1 byte, with true == 1 and false == 0
typedef bool cbbool;
#else // __cplusplus
#ifdef __OBJC__
typedef BOOL cbbool;
#else // __OBJC__
typedef signed char cbbool;
#endif // __OBJC__
#endif // __cplusplus

typedef struct
{
    const char* data;
    uintptr_t opaque;
} cbstring;

typedef struct
{
    cbbool has_value;
    cbbool value;
} cboptbool;

#ifdef __cplusplus
#define cbstringnull cbstring{ }
#else // __cplusplus
#define cbstringnull (const cbstring){ NULL, 0 }
#endif // __cplusplus

#endif // CODE_BINDER_BASE_TYPES
