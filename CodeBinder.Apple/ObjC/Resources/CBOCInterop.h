#ifndef CBOCINTEROP_HEADER
#define CBOCINTEROP_HEADER
#pragma once

#import <Foundation/Foundation.h>
#include <CBInterop.h>
#include <string>
#include <utility>

class SN2OC
{
private:
    bool m_handled;
    cbstring m_cstr;
    NSString* __strong* m_ocstr;

public:
    SN2OC(NSString* str)
        : m_handled(false), m_cstr{ }, m_ocstr(nil)
    {
        if (str != nil)
            m_cstr = CBCreateStringView([str UTF8String]);
    }

    SN2OC(NSString* __strong* str)
        : m_handled(true), m_cstr{ }, m_ocstr(str) { }

    SN2OC(const cbstring& str)
        : m_handled(false), m_cstr(str), m_ocstr(nil) { }

    // Move semantics
    SN2OC(cbstring&& str)
        : m_handled(true), m_cstr(str), m_ocstr(nil)
    {
        str = { };
    }

    ~SN2OC()
    {
        if (m_handled)
        {
            if (m_ocstr != nullptr)
            {
                if (m_cstr.data == nullptr)
                    *m_ocstr = nil;
                else
                    *m_ocstr = [[NSString alloc]initWithBytes:m_cstr.data length:CBSLEN(m_cstr) encoding:NSUTF8StringEncoding];
            }

            CBFreeString(&m_cstr);
        }
    }

public:
    operator NSString* () const
    {
        if (m_cstr.data == nullptr)
            return nil;

        return [[NSString alloc]initWithBytes:m_cstr.data length:CBSLEN(m_cstr) encoding:NSUTF8StringEncoding];
    }

    operator cbstring()
    {
        return m_cstr;
    }
};

#endif // CBOCINTEROP_HEADER
