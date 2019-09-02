#include <jni.h>
#include <cassert>
#include "JNIShared.h"

#define JNI_VERSION JNI_VERSION_1_6

static jfieldID handleFieldID;

jlong GetHandle(JNIEnv *env, jHandleRef handleref)
{
    return env->GetLongField(handleref, handleFieldID);
}

JNIEnv * GetEnv(JavaVM *jvm)
{
    // GetEnv can be used only if current thread was created
    // with Java, otherwise AttachCurrentProcess should be used
    // instead
    JNIEnv *env;
    jint rs = jvm->GetEnv((void **)&env, JNI_VERSION);
    assert(rs == JNI_OK);
    return env;
}

JavaVM * GetJvm(JNIEnv *env)
{
    JavaVM * jvm;
    jint rs = env->GetJavaVM(&jvm);
    assert(rs == JNI_OK);
    return jvm;
}

extern "C"
{
    JNIEXPORT jint JNICALL JNI_OnLoad(JavaVM* jvm, void* reserved)
    {
        JNIEnv* env = GetEnv(jvm);
        jclass cls = env->FindClass("CodeBinder/HandleRef");
        handleFieldID = env->GetFieldID(cls, "handle", "J");
        env->DeleteLocalRef(cls);
        return JNI_VERSION;
    }
}
