#ifndef CBOCINTEROP_HEADER
#define CBOCINTEROP_HEADER
#pragma once

#import <Foundation/Foundation.h>
#include <CBInterop.h>

class SN2OC
{
private:
    bool m_handled;
    cbstring_t m_cstr;
	NSString * __strong * m_ocstr;	
public:
    SN2OC(NSString * __strong * str)
	{
		m_handled = true;
		m_cstr = nullptr;
		m_ocstr = str;	
	}

    SN2OC(const cbstring_t str)
	{
		m_handled = false;		
		m_cstr = (cbstring_t)str;
		m_ocstr = nullptr;
	}
	
    SN2OC(cbstring_t &&str)
	{
		m_handled = true;		
		m_cstr = str;
		m_ocstr = nullptr;
	}
	
    ~SN2OC()
	{
		if (m_handled)
		{
			if (m_ocstr != nullptr)
			{
				if (m_cstr == nullptr)
					*m_ocstr = nil;
				else
					*m_ocstr = [[NSString alloc]initWithUTF8String:m_cstr];
			}
				
			cb::FreeString(m_cstr);
		}
	}
public:
    operator NSString *() const
	{
		if (m_cstr == nullptr)
			return nil;

		return [[NSString alloc]initWithUTF8String:m_cstr];
	}
	
    operator cbstring_t *()
	{
		return &m_cstr;
	}	
};

#endif // CBOCINTEROP_HEADER
