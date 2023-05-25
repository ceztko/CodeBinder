#pragma once

#include <jni.h>

#if defined(__APPLE__) && !defined(JNI_VERSION_1_8)
// Workaround for old macosx JDK7 build that doesn't
// export symbols on gcc/clang
// TODO: Create a custom jni.h wrapper header and include it
// as a more common header
#undef JNIIMPORT
#undef JNIEXPORT
#define JNIIMPORT __attribute__((visibility("default")))
#define JNIEXPORT __attribute__((visibility("default")))
#endif

#define jBooleanBox jobject
#define jByteBox jobject
#define jShortBox jobject
#define jIntegerBox jobject
#define jLongBox jobject
#define jFloatBox jobject
#define jDoubleBox jobject
#define jStringBox jobject
#define jHandleRef jobject

// Support class for array of pointers
class _jptrArray : public _jlongArray {};
typedef _jptrArray* jptrArray;
typedef jlong jptr;
