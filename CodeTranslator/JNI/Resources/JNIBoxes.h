#pragma once

#include "JNIBoxesTemplate.h"

JB2NImpl<jBooleanBox, jboolean> JB2N(JNIEnv *env, jBooleanBox box, bool commit = false);
JB2NImpl<jCharacterBox, jchar> JB2N(JNIEnv *env, jCharacterBox box, bool commit = false);
JB2NImpl<jByteBox, jbyte> JB2N(JNIEnv *env, jByteBox box, bool commit = false);
JB2NImpl<jShortBox, jshort> JB2N(JNIEnv *env, jShortBox box, bool commit = false);
JB2NImpl<jIntegerBox, jint> JB2N(JNIEnv *env, jIntegerBox box, bool commit = false);
JB2NImpl<jLongBox, jlong> JB2N(JNIEnv *env, jLongBox box, bool commit = false);
JB2NImpl<jFloatBox, jfloat> JB2N(JNIEnv *env, jFloatBox box, bool commit = false);
JB2NImpl<jDoubleBox, jdouble> JB2N(JNIEnv *env, jDoubleBox box, bool commit = false);
