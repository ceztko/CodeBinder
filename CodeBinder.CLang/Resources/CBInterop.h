#ifndef CODE_BINDER_INTEROP_HEADER
#define CODE_BINDER_INTEROP_HEADER
#pragma once

#include "CBBaseTypes.h"

#ifdef __cplusplus
#include <cstring>
#else // __cplusplus
#include <string.h>
#endif // __cplusplus

#if UINTPTR_MAX == UINT32_MAX
#define CB_STRING_OWNSDATA_FLAG (1u << 31)
#elif UINTPTR_MAX == UINT64_MAX
#define CB_STRING_OWNSDATA_FLAG (1ull << 63)
#else
#error "Environment not 32 or 64-bit."
#endif

#define CBSLEN(str) (size_t)((str).opaque & ~CB_STRING_OWNSDATA_FLAG)

extern "C"
{
#ifdef WIN32
    __declspec(dllimport) void __stdcall LocalFree(void* pv);
    __declspec(dllimport) void* __stdcall LocalAlloc(unsigned int uFlags, size_t uBytes);
#endif // WIN32

    inline void* CBAllocMemory(size_t size)
    {
#ifdef WIN32
#ifndef LMEM_FIXED
        const unsigned int LMEM_FIXED = 0x0000;
#endif // LMEM_FIXED
        return LocalAlloc(LMEM_FIXED, size);
#else
        return malloc(size);
#endif
    }

    inline void CBFreeMemory(void* ptr)
    {
#ifdef WIN32
        LocalFree(ptr);
#else
        free(ptr);
#endif
    }

    inline size_t CBStringGetLength(const cbstring* str)
    {
        return CBSLEN(*str);
    }

    // TODO: CBCreateString, CBCreateStringLen
    inline cbstring CBCreateStringView(const char* str)
    {
        cbstring ret = { str, str == nullptr ? 0 : strlen(str) };
        return ret;
    }

    inline cbstring CBCreateStringViewLen(const char* str, size_t len)
    {
        cbstring ret = { str, len };
        return ret;
    }

    inline void CBFreeString(cbstring* str)
    {
        if ((str->opaque & CB_STRING_OWNSDATA_FLAG) != 0)
        {
            CBFreeMemory((char*)str->data);
            *str = cbstringnull;
        }
    }
}

#endif // CODE_BINDER_INTEROP_HEADER
