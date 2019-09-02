#include "JNITypesPrivate.h"

#include "JNIShared.h"

jlong _jHandleRef::getHandle(JNIEnv *env)
{
    return ::GetHandle(env, this);
}
