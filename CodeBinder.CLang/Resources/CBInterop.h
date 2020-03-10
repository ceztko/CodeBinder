#ifndef CODE_BINDER_INTEROP_HEADER
#define CODE_BINDER_INTEROP_HEADER
#pragma once

#include <cstdlib>
#include <string>
#include "CBBaseTypes.h"

#ifdef WIN32
extern "C" __declspec(dllimport) void __stdcall CoTaskMemFree(void* pv);
extern "C" __declspec(dllimport) void __stdcall LocalFree(void* pv);
#endif // WIN32

namespace cb
{
    inline void FreeString(cbstring_t str)
    {
#ifdef WIN32
        CoTaskMemFree(str);
#else
        std::free(str);
#endif
    }

    inline void FreeMemory(void* ptr)
    {
#ifdef WIN32
        LocalFree(ptr);
#else
        free(ptr);
#endif
    }
}
#endif // CODE_BINDER_INTEROP_HEADER
