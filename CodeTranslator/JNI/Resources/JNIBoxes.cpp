#include "JNIBoxes.h"

JB2NImpl<jBooleanBox, jboolean> JB2N(JNIEnv * env, jBooleanBox box, bool commit)
{
    return JB2NImpl<jBooleanBox, jboolean>(env, box, commit);
}

JB2NImpl<jCharacterBox, jchar> JB2N(JNIEnv * env, jCharacterBox box, bool commit)
{
    return JB2NImpl<jCharacterBox, jchar>(env, box, commit);
}

JB2NImpl<jByteBox, jbyte> JB2N(JNIEnv * env, jByteBox box, bool commit)
{
    return JB2NImpl<jByteBox, jbyte>(env, box, commit);
}

JB2NImpl<jShortBox, jshort> JB2N(JNIEnv * env, jShortBox box, bool commit)
{
    return JB2NImpl<jShortBox, jshort>(env, box, commit);
}

JB2NImpl<jIntegerBox, jint> JB2N(JNIEnv * env, jIntegerBox box, bool commit)
{
    return JB2NImpl<jIntegerBox, jint>(env, box, commit);
}

JB2NImpl<jLongBox, jlong> JB2N(JNIEnv * env, jLongBox box, bool commit)
{
    return JB2NImpl<jLongBox, jlong>(env, box, commit);
}

JB2NImpl<jFloatBox, jfloat> JB2N(JNIEnv * env, jFloatBox box, bool commit)
{
    return JB2NImpl<jFloatBox, jfloat>(env, box, commit);
}

JB2NImpl<jDoubleBox, jdouble> JB2N(JNIEnv * env, jDoubleBox box, bool commit)
{
    return JB2NImpl<jDoubleBox, jdouble>(env, box, commit);
}
