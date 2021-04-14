#include "JNITypes.h"

extern "C"
{
    JNIEXPORT jlong JNICALL Java_CodeBinder_BinderUtils_newGlobalRef(
        JNIEnv *env, jclass, jobject obj)
    {
        return (jlong)env->NewGlobalRef(obj);
    }

    JNIEXPORT void JNICALL Java_CodeBinder_BinderUtils_deleteGlobalRef(
        JNIEnv *env, jclass, jlong globalref)
    {
        env->DeleteGlobalRef((jobject)globalref);
    }

	JNIEXPORT jlong JNICALL Java_CodeBinder_BinderUtils_newGlobalWeakRef(
		JNIEnv *env, jclass, jobject obj)
	{
        return (jlong)env->NewWeakGlobalRef(obj);
	}

	JNIEXPORT void JNICALL Java_CodeBinder_BinderUtils_deleteGlobalWeakRef(
		JNIEnv *env, jclass, jlong globalref)
	{
        env->DeleteWeakGlobalRef((jobject)globalref);
	}
}
