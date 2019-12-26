#include <cstring>
#include <string>
#include <stdexcept>
#include <cstdlib>
#endif

#ifdef WIN32
extern "C" __declspec(dllimport) void * __stdcall CoTaskMemAlloc(size_t cb);
#else

extern "C"
{
    struct cstring16
    {
        char16_t* string;
        intptr_t length;
    };

    struct cstring
    {
        char* string;
        intptr_t length;
    };

    inline cstring16 alloc_cstring16(const char16_t* str, size_t len)
    {
        if (str == nullptr)
            return { nullptr, 0 };

        // Allocate also the space of the termination character
        char16_t* newstr;
    #ifdef WIN32
        newstr = (char16_t*)CoTaskMemAlloc((len + 1) * sizeof(char16_t));
    #else
        newstr = (char16_t*)std::malloc((len + 1) * sizeof(char16_t));
    #endif
        if (newstr == nullptr)
            throw std::bad_alloc();

        // We ignore if the string is longer than maximum positive
        // integer. The string will just be invalid in managed side
        cstring16 ret{ newstr, (intptr_t)len };
        std::memcpy(ret.String, str, len * sizeof(char16_t));
        ret.String[len] = u'\0';
        return ret;
    }
}

#ifdef __cplusplus
CString16 alloc_cstring16(const std::u16string& str)
{
    return alloc_cstring16(str.c_str(), str.length());
}
#endif
