/* This file was generated. DO NOT EDIT! */
#ifndef CODE_BINDER_INTEROP_INTERNAL_HEADER
#define CODE_BINDER_INTEROP_INTERNAL_HEADER
#pragma once

#include "../CBInterop.h"
#include <cstddef>
#include <cstring>
#include <string>
#include <string_view>
#include <codecvt>
#include <new>
#include <codecvt>

#ifdef WIN32
extern "C" __declspec(dllimport) void* __stdcall CoTaskMemAlloc(size_t cb);
extern "C" __declspec(dllimport) void* __stdcall LocalAlloc(unsigned int uFlags, size_t uBytes);
#endif // WIN32

// _CBU: Narrow to platform specific codebinder unicode string (std::string in Apple,
//       std::u16string everywhere else) and take data view
#ifdef CBSTRING_UTF8
#define _CBU(str) _U8C(str)
#else
#define _CBU(str) _U16C(str)
#endif

namespace cb
{
    namespace internal
    {
        inline std::string U16ToU8(const std::u16string_view& view);
        inline std::u16string U8ToU16(const std::string_view& view);
    }

    inline cbstring_t CopyString(const cbchar_t* str, size_t len)
    {
        // Allocate also the space of the termination character
#ifdef WIN32
        cbstring_t newstr = (cbstring_t)CoTaskMemAlloc((len + 1) * sizeof(cbchar_t));
#else
        cbstring_t newstr = (cbstring_t)std::malloc((len + 1) * sizeof(cbchar_t));
#endif
        if (newstr == nullptr)
            throw std::bad_alloc();

        std::memcpy(newstr, str, len * sizeof(cbchar_t));
        newstr[len] = CB_NULL_TERMINATION;
        return newstr;
    }

    inline cbstring_t MarshalString(const std::string_view& view)
    {
#ifdef CBSTRING_UTF8
        auto str = view.data();
        size_t len = view.length();
#else
        auto u16str = internal::U8ToU16(view);
        auto str = u16str.c_str();
        size_t len = u16str.length();
#endif
        return CopyString(str, len);
    }

    inline cbstring_t MarshalString(const char* str)
    {
        return MarshalString(std::string_view(str));
    }

    inline cbstring_t MarshalString(const std::string& str)
    {
        return MarshalString(std::string_view(str.c_str(), str.length()));
    }

    inline cbstring_t MarshalString(const std::u16string_view& view)
    {
#ifdef CBSTRING_UTF8
        auto u8str = internal::U16ToU8(view);
        auto str = u8str.c_str();
        size_t len = u8str.length();
#else
        auto str = view.data();
        size_t len = view.length();
#endif
        return CopyString(str, len);
    }

    inline cbstring_t MarshalString(const char16_t* str)
    {
        return MarshalString(std::u16string_view(str));
    }

    inline cbstring_t MarshalString(const std::u16string& str)
    {
        return MarshalString(std::u16string_view(str.c_str(), str.length()));
    }

#ifdef WIN32
    inline cbstring_t MarshalString(const std::wstring_view& view)
    {
        return MarshalString(reinterpret_cast<const std::wstring_view&>(view));
    }

    inline cbstring_t MarshalString(const wchar_t* str)
    {
        return MarshalString(reinterpret_cast<const char16_t*>(str));
    }

    inline cbstring_t MarshalString(const std::wstring& str)
    {
        return MarshalString(reinterpret_cast<const std::u16string&>(str));
    }
#endif // WIN32

    inline void* AllocMemory(size_t size)
    {
#ifdef WIN32
        unsigned int LMEM_FIXED = 0x0000;
        auto ret = LocalAlloc(LMEM_FIXED, size);
#else
        auto ret = malloc(size);

#endif
        if (ret == nullptr)
            throw std::bad_alloc();

        return ret;
    }

    std::string internal::U16ToU8(const std::u16string_view& view)
    {
        if (view.length() == 0)
            return std::string();

#if defined(_MSC_VER) && _MSC_VER < 1922 // Before MSVC 16.2
        // https://developercommunity.visualstudio.com/content/problem/539181/stdwstring-convert-char16-t-cant-work-in-vs2017.html
        static std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>, wchar_t> convert;
        return convert.to_bytes((wchar_t*)view.data(), (wchar_t*)view.data() + view.length());
#else
        static std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> convert;
        return convert.to_bytes(view.data(), view.data() + view.length());
#endif
    }

    std::u16string internal::U8ToU16(const std::string_view& view)
    {
        if (view.length() == 0)
            return std::u16string();

#if defined(_MSC_VER) && _MSC_VER < 1922 // Before MSVC 16.2
        // https://developercommunity.visualstudio.com/content/problem/539181/stdwstring-convert-char16-t-cant-work-in-vs2017.html
        static std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>, wchar_t> convert;
        return reinterpret_cast<std::u16string&>(convert.from_bytes(view.data(), view.data() + view.length()));
#else
        static std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> convert;
        return convert.from_bytes(view.data(), view.data() + view.length());
#endif
    }
}
#endif // CODE_BINDER_INTEROP_INTERNAL_HEADER
