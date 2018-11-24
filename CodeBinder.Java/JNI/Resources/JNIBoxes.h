#pragma once

#include "JNIBoxesTemplate.h"

BJ2NImpl<_jBooleanBox> BJ2N(JNIEnv *env, jBooleanBox box, bool commit = true);
BJ2NImpl<_jCharacterBox> BJ2N(JNIEnv *env, jCharacterBox box, bool commit = true);
BJ2NImpl<_jByteBox> BJ2N(JNIEnv *env, jByteBox box, bool commit = true);
BJ2NImpl<_jShortBox> BJ2N(JNIEnv *env, jShortBox box, bool commit = true);
BJ2NImpl<_jIntegerBox> BJ2N(JNIEnv *env, jIntegerBox box, bool commit = true);
BJ2NImpl<_jLongBox> BJ2N(JNIEnv *env, jLongBox box, bool commit = true);
BJ2NImpl<_jFloatBox> BJ2N(JNIEnv *env, jFloatBox box, bool commit = true);
BJ2NImpl<_jDoubleBox> BJ2N(JNIEnv *env, jDoubleBox box, bool commit = true);

template <typename TN>
BJ2NImpl<_jBooleanBox, TN> BJ2N(JNIEnv *env, jBooleanBox box, bool commit = true)
{
    return BJ2NImpl<_jBooleanBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jCharacterBox, TN> BJ2N(JNIEnv *env, jCharacterBox box, bool commit = true)
{
    return BJ2NImpl<_jCharacterBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jByteBox, TN> BJ2N(JNIEnv *env, jByteBox box, bool commit = true)
{
    return BJ2NImpl<_jByteBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jShortBox, TN> BJ2N(JNIEnv *env, jShortBox box, bool commit = true)
{
    return BJ2NImpl<_jShortBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jIntegerBox, TN> BJ2N(JNIEnv *env, jIntegerBox box, bool commit = true)
{
    return BJ2NImpl<_jIntegerBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jLongBox, TN> BJ2N(JNIEnv *env, jLongBox box, bool commit = true)
{
    return BJ2NImpl<_jLongBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jFloatBox, TN> BJ2N(JNIEnv *env, jFloatBox box, bool commit = true)
{
    return BJ2NImpl<_jFloatBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jDoubleBox, TN> BJ2N(JNIEnv *env, jDoubleBox box, bool commit = true)
{
    return BJ2NImpl<_jDoubleBox, TN>(env, box, commit);
}
