#ifndef CODE_BINDER_INTEROP_CPP_HEADER
#define CODE_BINDER_INTEROP_CPP_HEADER
#pragma once

#include "CBInterop.h"
#include <cstring>
#include <string>
#include <string_view>
#include <new>
#include <stdexcept>

#undef cbstringp
#undef cbstringr

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

    inline void* AllocMemory(size_t size)
    {
        auto ret = CBAllocMemory(size);
        if (ret == nullptr)
            throw std::bad_alloc();

        return ret;
    }

    inline void FreeMemory(void* ptr)
    {
        CBFreeMemory(ptr);
    }
}

class cbstringbase
{
    cbstringbase(const cbstringbase&) = delete;

public:
    ~cbstringbase()
    {
        if (m_str.ownsdata)
            CBFreeMemory((char*)m_str.data);
    }

protected:
    static cbstring init(const char* str, size_t len)
    {
        cbstring ret{ };
        if (len != 0)
        {
            auto newstr = (char*)CBAllocMemory(len + 1);
            std::memcpy(newstr, str, len);
            newstr[len] = '\0';
            ret.data = newstr;
            ret.length = len;
            ret.ownsdata = (unsigned)true;
        }
        return ret;
    }

    void operator=(const cbstring& str)
    {
        this->~cbstringbase();
        m_str = str;
    }

    cbstringbase()
        : m_str{ } { }

    cbstringbase(const cbstring& str)
        : m_str(str) { }

    // Non owning
    cbstringbase(const char* str, size_t len)
        : m_str{ }
    {
        m_str.data = str;
        m_str.length = len;
    }

public:
    // Dereferencing cast to std::string_view without null check
    std::string_view operator*() const
    {
        return std::string_view(m_str.data, m_str.length);
    }

    operator std::string_view() const
    {
        if (m_str.data == nullptr)
            throw std::invalid_argument("Inner string is null");

        return std::string_view(m_str.data, m_str.length);
    }

    explicit operator std::string() const
    {
        if (m_str.data == nullptr)
            throw std::invalid_argument("Inner string is null");

        return std::string(m_str.data, m_str.length);
    }

    explicit operator const char* () const
    {
        return m_str.data;
    }

    // Cast to cbstring& with a move semantics
    operator cbstring()
    {
        auto ret = m_str;
        m_str.ownsdata = (unsigned)false;
        return ret;
    }

    operator const cbstring& () const
    {
        return m_str;
    }

    bool operator==(std::nullptr_t nullpointer) const
    {
        return m_str.data == nullptr;
    }

    bool operator!=(std::nullptr_t nullpointer) const
    {
        return m_str.data != nullptr;
    }

private:
    cbstring m_str;
};

// cbstringv constructor by default doesn't allocate, assignment does
class cbstringp : public cbstringbase
{
    void* operator new (size_t) = delete;
    void operator delete(void*) = delete;

public:
    cbstringp() { }

    cbstringp(std::nullptr_t) { }

    cbstringp(const char* str, size_t len)
        : cbstringbase(str, len) { }

    cbstringp(const char* str)
        : cbstringbase(str, str == nullptr ? 0 : std::char_traits<char>::length(str)) { }

    cbstringp(const std::string_view& str)
        : cbstringbase(str.data(), str.length()) { }

    cbstringp(const std::string& str)
        : cbstringbase(str.data(), str.length()) { }

    void operator=(const std::string_view& str)
    {
        cbstringbase::operator=(init(str.data(), str.length()));
    }

    void operator=(const std::string& str)
    {
        cbstringbase::operator=(init(str.data(), str.length()));
    }

    void operator=(const char* str)
    {
        cbstringbase::operator=(init(str, str == nullptr ? 0 : std::char_traits<char>::length(str)));
    }
};

/// <summary>
/// cbstringr constructor by default allocates
/// </summary>
class cbstringr : public cbstringbase
{
    void* operator new (size_t) = delete;
    void operator delete(void*) = delete;

public:
    cbstringr() { }

    cbstringr(std::nullptr_t) { }

    cbstringr(const char* str, size_t len)
        : cbstringbase(init(str, len)) { }

    cbstringr(const char* str)
        : cbstringbase(init(str, str == nullptr ? 0 : std::char_traits<char>::length(str))) { }

    cbstringr(const std::string_view& str)
        : cbstringbase(init(str.data(), str.length())) { }

    cbstringr(const std::string& str)
        : cbstringbase(init(str.data(), str.length())) { }
};

#endif // CODE_BINDER_INTEROP_CPP_HEADER
