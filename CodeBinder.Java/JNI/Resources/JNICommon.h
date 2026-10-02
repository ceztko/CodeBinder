/**
 * SPDX-FileCopyrightText: (C) 2021 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#pragma once

#include <jni.h>
#include <stdexcept>
#include "JNIShared.h"
#include "JNIBoxes.h"
#include "JNIOptional.h"
#include <CBBaseTypes.h>
#include <CBInterop.h>

 // Wraps jstring and convert to utf-16 chars
class SJ2N
{
public:
    SJ2N(JNIEnv* env, jstring str);
    ~SJ2N();
public:
    operator cbstring() const;
private:
    JNIEnv* m_env;
    jstring m_string;
    const char* m_chars;
    jboolean m_isCopy;
};

// Wraps utf-16 chars and convert to jstring
class SN2J
{
public:
    SN2J(JNIEnv* env, const cbstring& str);
    SN2J(JNIEnv* env, cbstring&& str);
    ~SN2J();
public:
    operator jstring() const;
private:
    bool m_handled;
    JNIEnv* m_env;
    cbstring m_string;
};

// Adapter class to link correct JNI methods
template <typename TJArray, typename TNArray>
struct AJNIShim
{
    static TNArray* GetNativeArray(JNIEnv* env, TJArray jarray)
    {
        (void)env;
        (void)jarray;
        static_assert(always_false<TJArray, TNArray>, "Not implemented");
        return nullptr;
    }
    static void FreeNativeArray(JNIEnv* env, TJArray jarray, TNArray* narray, bool commit)
    {
        (void)env;
        (void)jarray;
        (void)narray;
        (void)commit;
        static_assert(always_false<TJArray, TNArray>, "Not implemented");
    }
};

template <typename TJArray, typename TNArray, typename... Args>
class AJ2NImpl;

// Wraps java array and convert to native one
template <typename TJArray, typename TNArray>
class AJ2NImpl<TJArray, TNArray>
{
public:
    AJ2NImpl(JNIEnv* env, TJArray array, bool commit)
    {
        m_env = env;
        m_jarray = array;
        m_commit = commit;
        if (array == nullptr)
            m_narray = nullptr;
        else
            m_narray = AJNIShim<TJArray, TNArray>::GetNativeArray(env, array);
    }
    ~AJ2NImpl()
    {
        if (m_jarray != nullptr)
            AJNIShim<TJArray, TNArray>::FreeNativeArray(m_env, m_jarray, m_narray, m_commit);
    }
public:
    inline TNArray* n_array() { return m_narray; }
    inline operator TNArray* () { return m_narray; }
    inline const TNArray* n_array() const { return m_narray; }
    inline operator const TNArray* () const { return m_narray; }
private:
    JNIEnv* m_env;
    TJArray m_jarray;
    TNArray* m_narray;
    bool m_commit;
};

template <typename TJArray, typename TNArray, typename TCArray, typename... Args>
class AJ2NImpl<TJArray, TNArray, TCArray, Args...> : public AJ2NImpl<TJArray, TNArray, Args...>
{
public:
    using AJ2NImpl<TJArray, TNArray, Args...>::AJ2NImpl;
public:
    inline operator TCArray* () { return (TCArray*)AJ2NImpl<TJArray, TNArray, Args...>::n_array(); }
};

template <>
struct AJNIShim<jbyteArray, jbyte>
{
    static jbyte* GetNativeArray(JNIEnv* env, jbyteArray jarray)
    {
        return env->GetByteArrayElements(jarray, nullptr);
    }

    static void FreeNativeArray(JNIEnv* env, jbyteArray jarray, jbyte* narray, bool commit)
    {
        env->ReleaseByteArrayElements(jarray, narray, commit ? 0 : JNI_ABORT);
    }
};

template <>
struct AJNIShim<jshortArray, jshort>
{
    static jshort* GetNativeArray(JNIEnv* env, jshortArray jarray)
    {
        return env->GetShortArrayElements(jarray, nullptr);
    }

    static void FreeNativeArray(JNIEnv* env, jshortArray jarray, jshort* narray, bool commit)
    {
        env->ReleaseShortArrayElements(jarray, narray, commit ? 0 : JNI_ABORT);
    }
};

template <>
struct AJNIShim<jintArray, jint>
{
    static jint* GetNativeArray(JNIEnv* env, jintArray jarray)
    {
        return env->GetIntArrayElements(jarray, nullptr);
    }

    static void FreeNativeArray(JNIEnv* env, jintArray jarray, jint* narray, bool commit)
    {
        env->ReleaseIntArrayElements(jarray, narray, commit ? 0 : JNI_ABORT);
    }
};

template <>
struct AJNIShim<jlongArray, jlong>
{
    static jlong* GetNativeArray(JNIEnv* env, jlongArray jarray)
    {
        return env->GetLongArrayElements(jarray, nullptr);
    }

    static void FreeNativeArray(JNIEnv* env, jlongArray jarray, jlong* narray, bool commit)
    {
        env->ReleaseLongArrayElements(jarray, narray, commit ? 0 : JNI_ABORT);
    }
};

template <>
struct AJNIShim<jfloatArray, jfloat>
{
    static jfloat* GetNativeArray(JNIEnv* env, jfloatArray jarray)
    {
        return env->GetFloatArrayElements(jarray, nullptr);
    }

    static void FreeNativeArray(JNIEnv* env, jfloatArray jarray, jfloat* narray, bool commit)
    {
        env->ReleaseFloatArrayElements(jarray, narray, commit ? 0 : JNI_ABORT);
    }
};

template <>
struct AJNIShim<jdoubleArray, jdouble>
{
    static jdouble* GetNativeArray(JNIEnv* env, jdoubleArray jarray)
    {
        return env->GetDoubleArrayElements(jarray, nullptr);
    }

    static void FreeNativeArray(JNIEnv* env, jdoubleArray jarray, jdouble* narray, bool commit)
    {
        env->ReleaseDoubleArrayElements(jarray, narray, commit ? 0 : JNI_ABORT);
    }
};

// Wraps java string array and convert to a native one with non owning
// cbstring, as a cbstring with CB_STRING_OWNSDATA_FLAG set is owned by
// the callee. The converted strings are kept in m_strings
template <>
class AJ2NImpl<jstringArray, cbstring>
{
    AJ2NImpl(const AJ2NImpl&) = delete;
    AJ2NImpl& operator=(const AJ2NImpl&) = delete;
public:
    AJ2NImpl(JNIEnv* env, jstringArray array, bool commit)
        : m_env(env), m_jarray(array), m_commit(commit),
          m_size(0), m_strings(nullptr), m_narray(nullptr)
    {
        if (array == nullptr)
            return;

        m_size = env->GetArrayLength(array);
        m_strings = new cbstring[m_size];
        m_narray = new cbstring[m_size];
        for (jsize i = 0; i < m_size; i++)
        {
            auto jstr = (jstring)env->GetObjectArrayElement(array, i);
            if (jstr == nullptr)
            {
                m_strings[i] = { };
            }
            else
            {
                jsize length = env->GetStringUTFLength(jstr);
                jboolean isCopy;
                auto chars = env->GetStringUTFChars(jstr, &isCopy);
                m_strings[i] = CBCreateStringLen(chars, (size_t)length);
                // NOTE: Unconditionally release the string is required
                env->ReleaseStringUTFChars(jstr, chars);
            }

            m_narray[i] = CBCreateStringViewLen(m_strings[i].data, CBSLEN(m_strings[i]));
        }
    }
    ~AJ2NImpl()
    {
        if (m_jarray == nullptr)
            return;

        for (jsize i = 0; i < m_size; i++)
        {
            // Set the strings replaced by the callee on the java array
            if (m_commit && m_narray[i].data != m_strings[i].data)
            {
                if (m_narray[i].data == nullptr)
                    m_env->SetObjectArrayElement(m_jarray, i, nullptr);
                else
                    m_env->SetObjectArrayElement(m_jarray, i, m_env->NewStringUTF(m_narray[i].data));
            }

            // Free the strings set by the callee and the converted ones
            CBFreeString(&m_narray[i]);
            CBFreeString(&m_strings[i]);
        }

        delete[] m_narray;
        delete[] m_strings;
    }
public:
    inline cbstring* n_array() { return m_narray; }
    inline operator cbstring* () { return m_narray; }
    inline const cbstring* n_array() const { return m_narray; }
    inline operator const cbstring* () const { return m_narray; }
private:
    JNIEnv* m_env;
    jstringArray m_jarray;
    bool m_commit;
    jsize m_size;
    cbstring* m_strings;
    cbstring* m_narray;
};

template <>
struct AJNIShim<jptrArray, void*>
{
    static void** GetNativeArray(JNIEnv* env, jptrArray jarray)
    {
#if __LP64__ || _WIN64
        return (void**)env->GetLongArrayElements(jarray, nullptr);
#else
        jsize size = env->GetArrayLength(jarray);
        return new void* [size];
#endif
    }

    static void FreeNativeArray(JNIEnv* env, jptrArray jarray, void** narray, bool commit)
    {
#if __LP64__ || _WIN64
        env->ReleaseLongArrayElements(jarray, (jlong*)narray, commit ? 0 : JNI_ABORT);
#else
        if (commit)
        {
            jsize size = env->GetArrayLength(jarray);
            auto nLongArray = env->GetLongArrayElements(jarray, nullptr);
            for (int i = 0; i < size; i++)
                nLongArray[i] = (jlong)narray[i];
            env->ReleaseLongArrayElements(jarray, nLongArray, 0);
        }

        delete[] narray;
#endif
    }
};

template <>
class AJ2NImpl<jptrArray, void*>
{
public:
    AJ2NImpl(JNIEnv* env, jptrArray array, bool commit)
    {
        m_env = env;
        m_jarray = array;
        m_commit = commit;
        if (array == nullptr)
            m_narray = nullptr;
        else
            m_narray = AJNIShim<jptrArray, void*>::GetNativeArray(env, array);
    }
    ~AJ2NImpl()
    {
        if (m_jarray != nullptr)
            AJNIShim<jptrArray, void*>::FreeNativeArray(m_env, m_jarray, m_narray, m_commit);
    }
public:
    inline void** n_array() { return m_narray; }
    inline operator void** () { return m_narray; }
    inline const void** n_array() const { return (const void**)m_narray; }
    inline operator const void** () const { return (const void**)m_narray; }
private:
    JNIEnv* m_env;
    jptrArray m_jarray;
    void** m_narray;
    bool m_commit;
};

// Function overloads to create actual implentations of convert classes
AJ2NImpl<jbyteArray, jbyte, uint8_t, int8_t> AJ2N(JNIEnv* env, jbyteArray jarray, bool commit);
AJ2NImpl<jshortArray, jshort, uint16_t, int16_t> AJ2N(JNIEnv* env, jshortArray jarray, bool commit);
AJ2NImpl<jintArray, jint, uint32_t, int32_t> AJ2N(JNIEnv* env, jintArray jarray, bool commit);
AJ2NImpl<jlongArray, jlong, uint64_t, int64_t> AJ2N(JNIEnv* env, jlongArray jarray, bool commit);
AJ2NImpl<jfloatArray, jfloat, float> AJ2N(JNIEnv* env, jfloatArray jarray, bool commit);
AJ2NImpl<jdoubleArray, jdouble, double> AJ2N(JNIEnv* env, jdoubleArray jarray, bool commit);
AJ2NImpl<jptrArray, void*> AJ2N(JNIEnv* env, jptrArray jarray, bool commit);
AJ2NImpl<jstringArray, cbstring> AJ2N(JNIEnv* env, jstringArray jarray, bool commit);
