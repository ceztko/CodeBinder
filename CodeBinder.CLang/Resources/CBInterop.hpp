#ifndef CODE_BINDER_INTEROP_CPP_HEADER
#define CODE_BINDER_INTEROP_CPP_HEADER
#pragma once

#include "CBInterop.h"
#include <cstring>
#include <string>
#include <string_view>
#include <utility>
#include <new>
#include <stdexcept>

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
        if ((m_str.opaque & CB_STRING_OWNSDATA_FLAG) != 0)
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
            ret.opaque = len | CB_STRING_OWNSDATA_FLAG;
        }
        return ret;
    }

    void operator=(const cbstring& str)
    {
        this->~cbstringbase();
        m_str = str;
    }

    cbstringbase()
        : m_str{ "", 0 } { }

    cbstringbase(nullptr_t)
        : m_str{ } { }

    cbstringbase(const cbstring& str)
        : m_str(str) { }

    // Non owning
    cbstringbase(const char* str, size_t len)
    {
        m_str.data = str;
        m_str.opaque = len;
    }

    const cbstring& str() const { return m_str; }

    cbstring release()
    {
        auto ret = m_str;
        m_str = { };
        return ret;
    }

public:
    // Dereferencing cast to std::string_view without null check
    std::string_view operator*() const
    {
        return std::string_view(m_str.data, CBSLEN(m_str));
    }

    operator std::string_view() const
    {
        if (m_str.data == nullptr)
            throw std::invalid_argument("Inner string is null");

        return std::string_view(m_str.data, CBSLEN(m_str));
    }

    explicit operator std::string() const
    {
        if (m_str.data == nullptr)
            throw std::invalid_argument("Inner string is null");

        return std::string(m_str.data, CBSLEN(m_str));
    }

    explicit operator const char* () const
    {
        return m_str.data;
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
class cbstringp final : public cbstringbase
{
    friend class cbstringpr;
    void* operator new (size_t) = delete;
    void operator delete(void*) = delete;

public:
    cbstringp() { }

    cbstringp(cbstring&& str)
        : cbstringbase(str)
    {
        str = { };
    }

    cbstringp(std::nullptr_t)
        : cbstringbase(nullptr) { }

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

    operator const cbstring& () const
    {
        return str();
    }
};

// Used to wrap ref parameters
class cbstringpr final
{
public:
    explicit cbstringpr(cbstring* pstr)
        : m_cstr(pstr), m_str(pstr == nullptr ? cbstring() : *pstr) { }

    ~cbstringpr()
    {
        if (m_cstr != nullptr)
            *m_cstr = m_str.release();
    }

    /// <summary>
    /// Dereference cbstringpr built from null cbstring is undefined behavior
    /// </summary>
    cbstringp& operator*()
    {
        return m_str;
    }

    bool operator==(std::nullptr_t nullpointer) const
    {
        return m_cstr == nullptr;
    }

    bool operator!=(std::nullptr_t nullpointer) const
    {
        return m_cstr != nullptr;
    }

private:
    cbstring* m_cstr;
    cbstringp m_str;
};

/// <summary>
/// cbstringr constructor by default allocates
/// </summary>
class cbstringr final : public cbstringbase
{
    void* operator new (size_t) = delete;
    void operator delete(void*) = delete;

public:
    cbstringr(cbstring&& str)
        : cbstringbase(str)
    {
        str = { };
    }

    cbstringr() { }

    cbstringr(std::nullptr_t)
        : cbstringbase(nullptr) { }

    cbstringr(const char* str, size_t len)
        : cbstringbase(init(str, len)) { }

    cbstringr(const char* str)
        : cbstringbase(init(str, str == nullptr ? 0 : std::char_traits<char>::length(str))) { }

    cbstringr(const std::string_view& str)
        : cbstringbase(init(str.data(), str.length())) { }

    cbstringr(const std::string& str)
        : cbstringbase(init(str.data(), str.length())) { }

    // Cast to cbstring with a move semantics
    operator cbstring() &&
    {
        return release();
    }
};

#endif // CODE_BINDER_INTEROP_CPP_HEADER
