/**
 * SPDX-FileCopyrightText: (C) 2021 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#include "JNICommon.h"
#include <utility>
#include <string>
#include <cassert>
#include <CBInterop.h>

using namespace std;

SJ2N::SJ2N(JNIEnv* env, jstring str)
    : m_env(env), m_string(str), m_chars(nullptr), m_isCopy(false)
{
    if (m_string != nullptr)
        m_chars = m_env->GetStringUTFChars(m_string, &m_isCopy);
}

SN2J::SN2J(JNIEnv* env, const cbstring& str)
    : m_handled(false), m_env(env), m_string(str) { }

// Move semantics
SN2J::SN2J(JNIEnv* env, cbstring&& str)
    : m_handled(true), m_env(env), m_string(str)
{
    str = { };
}

SJ2N::~SJ2N()
{
    if (m_isCopy)
        m_env->ReleaseStringUTFChars(m_string, m_chars);
}

SJ2N::operator cbstring() const
{
    if (m_string == nullptr)
        return { };

    jsize length = m_env->GetStringUTFLength(m_string);
    return CBCreateStringViewLen(m_chars, (size_t)length);
}

SN2J::~SN2J()
{
    if (m_handled)
        CBFreeString(&m_string);
}

SN2J::operator jstring() const
{
    if (m_string.data == nullptr)
        return nullptr;

    return m_env->NewStringUTF(m_string.data);
}

AJ2NImpl<jbyteArray, jbyte, uint8_t, int8_t> AJ2N(JNIEnv* env, jbyteArray jarray, bool commit)
{
    return AJ2NImpl<jbyteArray, jbyte, uint8_t, int8_t>(env, jarray, commit);
}

AJ2NImpl<jshortArray, jshort, uint16_t, int16_t> AJ2N(JNIEnv* env, jshortArray jarray, bool commit)
{
    return AJ2NImpl<jshortArray, jshort, uint16_t, int16_t>(env, jarray, commit);
}

AJ2NImpl<jintArray, jint, uint32_t, int32_t> AJ2N(JNIEnv* env, jintArray jarray, bool commit)
{
    return AJ2NImpl<jintArray, jint, uint32_t, int32_t>(env, jarray, commit);
}

AJ2NImpl<jlongArray, jlong, uint64_t, int64_t> AJ2N(JNIEnv* env, jlongArray jarray, bool commit)
{
    return AJ2NImpl<jlongArray, jlong, uint64_t, int64_t>(env, jarray, commit);
}

AJ2NImpl<jfloatArray, jfloat, float> AJ2N(JNIEnv* env, jfloatArray jarray, bool commit)
{
    return AJ2NImpl<jfloatArray, jfloat, float>(env, jarray, commit);
}

AJ2NImpl<jdoubleArray, jdouble, double> AJ2N(JNIEnv* env, jdoubleArray jarray, bool commit)
{
    return AJ2NImpl<jdoubleArray, jdouble, double>(env, jarray, commit);
}

AJ2NImpl<jptrArray, void*> AJ2N(JNIEnv* env, jptrArray jarray, bool commit)
{
    return AJ2NImpl<jptrArray, void*>(env, jarray, commit);
}
