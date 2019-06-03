#include <jni.h>

extern "C"
{
    JNIEXPORT jlong JNICALL CodeBinder_BinderUtils_NewGlobalRef(
        JNIEnv *env, jclass, jobject obj)
    {
        return (jlong)env->NewGlobalRef(obj);
    }

    JNIEXPORT void JNICALL CodeBinder_BinderUtils_DeleteGlobalRef(
        JNIEnv *env, jclass, jlong globalref)
    {
        env->DeleteGlobalRef((jobject)globalref);
    }
	
	JNIEXPORT jlong JNICALL CodeBinder_BinderUtils_NewGlobalWeakRef(
		JNIEnv *env, jclass, jobject obj)
	{
        return (jlong)env->NewWeakGlobalRef(obj);
	}

	JNIEXPORT void JNICALL CodeBinder_BinderUtils_DeleteGlobalWeakRef(
		JNIEnv *env, jclass, jlong globalref)
	{
        env->DeleteWeakGlobalRef((jobject)globalref);
	}
}
