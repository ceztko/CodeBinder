/* This file was generated. DO NOT EDIT! */
#ifndef CODE_BINDER_INTEROP_HEADER
#define CODE_BINDER_INTEROP_HEADER
#pragma once

#include <cstdlib>
#include <exception>
#include <string>
#include "CBBaseTypes.h"

#ifdef WIN32
extern "C" __declspec(dllimport) void __stdcall CoTaskMemFree(void* pv);
extern "C" __declspec(dllimport) void __stdcall LocalFree(void* pv);
#endif // WIN32

namespace cb
{
    /// <summary>
    /// This exception can be used just to unwind the stack,
    /// for example in Java interop scenario when returning
    /// from callbacks. It should be catched in outer C functions
    /// and just return from them
    /// </summary>
    class StackUnwinder : public ::std::exception
    {
    };

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
