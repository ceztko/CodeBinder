#include "JNIBoxes.h"

BJ2NImpl<jBooleanBox, jboolean> BJ2N(JNIEnv * env, jBooleanBox box, bool commit)
{
    return BJ2NImpl<jBooleanBox, jboolean>(env, box, commit);
}

BJ2NImpl<jCharacterBox, jchar> BJ2N(JNIEnv * env, jCharacterBox box, bool commit)
{
    return BJ2NImpl<jCharacterBox, jchar>(env, box, commit);
}

BJ2NImpl<jByteBox, jbyte> BJ2N(JNIEnv * env, jByteBox box, bool commit)
{
    return BJ2NImpl<jByteBox, jbyte>(env, box, commit);
}

BJ2NImpl<jShortBox, jshort> BJ2N(JNIEnv * env, jShortBox box, bool commit)
{
    return BJ2NImpl<jShortBox, jshort>(env, box, commit);
}

BJ2NImpl<jIntegerBox, jint> BJ2N(JNIEnv * env, jIntegerBox box, bool commit)
{
    return BJ2NImpl<jIntegerBox, jint>(env, box, commit);
}

BJ2NImpl<jLongBox, jlong> BJ2N(JNIEnv * env, jLongBox box, bool commit)
{
    return BJ2NImpl<jLongBox, jlong>(env, box, commit);
}

BJ2NImpl<jFloatBox, jfloat> BJ2N(JNIEnv * env, jFloatBox box, bool commit)
{
    return BJ2NImpl<jFloatBox, jfloat>(env, box, commit);
}

BJ2NImpl<jDoubleBox, jdouble> BJ2N(JNIEnv * env, jDoubleBox box, bool commit)
{
    return BJ2NImpl<jDoubleBox, jdouble>(env, box, commit);
}
