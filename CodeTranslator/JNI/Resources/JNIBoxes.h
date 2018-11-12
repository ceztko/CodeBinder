#pragma once

#include "JNIBoxesTemplate.h"

BJ2NImpl<jBooleanBox, jboolean> BJ2N(JNIEnv *env, jBooleanBox box, bool commit = true);
BJ2NImpl<jCharacterBox, jchar> BJ2N(JNIEnv *env, jCharacterBox box, bool commit = true);
BJ2NImpl<jByteBox, jbyte> BJ2N(JNIEnv *env, jByteBox box, bool commit = true);
BJ2NImpl<jShortBox, jshort> BJ2N(JNIEnv *env, jShortBox box, bool commit = true);
BJ2NImpl<jIntegerBox, jint> BJ2N(JNIEnv *env, jIntegerBox box, bool commit = true);
BJ2NImpl<jLongBox, jlong> BJ2N(JNIEnv *env, jLongBox box, bool commit = true);
BJ2NImpl<jFloatBox, jfloat> BJ2N(JNIEnv *env, jFloatBox box, bool commit = true);
BJ2NImpl<jDoubleBox, jdouble> BJ2N(JNIEnv *env, jDoubleBox box, bool commit = true);
