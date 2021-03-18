#include "JNIBoxes.h"

BJ2NImpl<_jBooleanBox> BJ2N(JNIEnv * env, jBooleanBox box, bool commit)
{
    return BJ2NImpl<_jBooleanBox>(env, box, commit);
}

BJ2NImpl<_jCharacterBox> BJ2N(JNIEnv * env, jCharacterBox box, bool commit)
{
    return BJ2NImpl<_jCharacterBox>(env, box, commit);
}

BJ2NImpl<_jByteBox> BJ2N(JNIEnv * env, jByteBox box, bool commit)
{
    return BJ2NImpl<_jByteBox>(env, box, commit);
}

BJ2NImpl<_jShortBox> BJ2N(JNIEnv * env, jShortBox box, bool commit)
{
    return BJ2NImpl<_jShortBox>(env, box, commit);
}

BJ2NImpl<_jIntegerBox> BJ2N(JNIEnv * env, jIntegerBox box, bool commit)
{
    return BJ2NImpl<_jIntegerBox>(env, box, commit);
}

BJ2NImpl<_jLongBox> BJ2N(JNIEnv * env, jLongBox box, bool commit)
{
    return BJ2NImpl<_jLongBox>(env, box, commit);
}

BJ2NImpl<_jFloatBox> BJ2N(JNIEnv * env, jFloatBox box, bool commit)
{
    return BJ2NImpl<_jFloatBox>(env, box, commit);
}

BJ2NImpl<_jDoubleBox> BJ2N(JNIEnv * env, jDoubleBox box, bool commit)
{
    return BJ2NImpl<_jDoubleBox>(env, box, commit);
}

SBJ2N BJ2N(JNIEnv* env, jStringBox box, bool commit)
{
    return SBJ2N(env, box, commit);
}
