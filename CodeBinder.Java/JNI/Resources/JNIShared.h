#pragma once

#include "JNITypesPrivate.h"

jlong GetHandle(JNIEnv *env, jHandleRef handleref);
JNIEnv * GetEnv(JavaVM *jvm);
JavaVM * GetJvm(JNIEnv *env);
