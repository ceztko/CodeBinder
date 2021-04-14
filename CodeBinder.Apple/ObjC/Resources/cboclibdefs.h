#pragma once

#if defined(OBJC_CODEBINDER_SHARED) || !defined(OBJC_CODEBINDER_STATIC)

#ifdef OBJC_CODEBINDER_EXPORT
    #define OBJC_CODEBINDER_API __attribute__ ((visibility ("default")))
#else
    #define OBJC_CODEBINDER_IMPORT
    #define OBJC_CODEBINDER_API
#endif

#else
    #define OBJC_CODEBINDER_API
    #ifndef OBJC_CODEBINDER_EXPORT
        #define OBJC_CODEBINDER_IMPORT
    #endif
#endif
