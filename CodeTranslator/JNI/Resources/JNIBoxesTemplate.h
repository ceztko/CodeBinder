#pragma once

#include <stdexcept>
#include "JNITypesPrivate.h"

// Wraps custom java box type
template <typename TJBox, typename T>
class JB2NImpl
{
public:
    JB2NImpl(JNIEnv *env, TJBox box, bool commit);
    ~JB2NImpl();
public:
    inline operator T *() const { return &Value; }
private:
    jfieldID getFieldId();
public:
    T Value;
private:
    JNIEnv *m_env;
    TJBox m_box;
    bool m_commit;
};

// Adapter class to link correct JNI methods
template <typename TJBox, typename T>
struct JB2NShim
{
    static const char * GetFieldIdSignature() { throw std::exception(); }
    static T GetValue(JNIEnv *env, TJBox box, jfieldID field) { throw std::exception(); }
    static void SetValue(JNIEnv *env, TJBox box, jfieldID field, T value) { throw std::exception(); }
};

// Shim specializations
template<typename TJBox, typename T>
JB2NImpl<TJBox, T>::JB2NImpl(JNIEnv *env, TJBox box, bool commit)
{
    m_env = env;
    m_box = box;
    m_commit = commit;
    Value = JB2NShim<TJBox, T>::GetValue(env, box, getFieldId());
}

template<typename TJBox, typename T>
JB2NImpl<TJBox, T>::~JB2NImpl()
{
    if (m_commit)
        JB2NShim<TJBox, T>::SetValue(m_env, m_box, getFieldId(), Value);
}

template<typename TJBox, typename T>
jfieldID JB2NImpl<TJBox, T>::getFieldId()
{
    auto cls = m_env->GetObjectClass(m_box);
    return m_env->GetFieldID(cls, "value", JB2NShim<TJBox, T>::GetFieldIdSignature());
}

// Shimp specializations
template <>
struct JB2NShim<jBooleanBox, jboolean>
{
    static const char * GetFieldIdSignature()
    {
        return "Z";
    }
    static jboolean GetValue(JNIEnv *env, jBooleanBox box, jfieldID field)
    {
        return env->GetBooleanField(box, field);
    }
    static void SetValue(JNIEnv *env, jBooleanBox box, jfieldID field, jboolean value)
    {
        env->SetBooleanField(box, field, value);
    }
};

template <>
struct JB2NShim<jCharacterBox, jchar>
{
    static const char * GetFieldIdSignature()
    {
        return "C";
    }
    static jchar GetValue(JNIEnv *env, jCharacterBox box, jfieldID field)
    {
        return env->GetCharField(box, field);
    }
    static void SetValue(JNIEnv *env, jCharacterBox box, jfieldID field, jchar value)
    {
        env->SetCharField(box, field, value);
    }
};

template <>
struct JB2NShim<jByteBox, jbyte>
{
    static const char * GetFieldIdSignature()
    {
        return "B";
    }
    static jbyte GetValue(JNIEnv *env, jByteBox box, jfieldID field)
    {
        return env->GetByteField(box, field);
    }
    static void SetValue(JNIEnv *env, jByteBox box, jfieldID field, jbyte value)
    {
        env->SetByteField(box, field, value);
    }
};

template <>
struct JB2NShim<jShortBox, jshort>
{
    static const char * GetFieldIdSignature()
    {
        return "S";
    }
    static jshort GetValue(JNIEnv *env, jShortBox box, jfieldID field)
    {
        return env->GetShortField(box, field);
    }
    static void SetValue(JNIEnv *env, jShortBox box, jfieldID field, jshort value)
    {
        env->SetShortField(box, field, value);
    }
};

template <>
struct JB2NShim<jIntegerBox, jint>
{
    static const char * GetFieldIdSignature()
    {
        return "I";
    }
    static jint GetValue(JNIEnv *env, jIntegerBox box, jfieldID field)
    {
        return env->GetIntField(box, field);
    }
    static void SetValue(JNIEnv *env, jIntegerBox box, jfieldID field, jint value)
    {
        env->SetIntField(box, field, value);
    }
};

template <>
struct JB2NShim<jLongBox, jlong>
{
    static const char * GetFieldIdSignature()
    {
        return "J";
    }
    static jlong GetValue(JNIEnv *env, jLongBox box, jfieldID field)
    {
        return env->GetLongField(box, field);
    }
    static void SetValue(JNIEnv *env, jLongBox box, jfieldID field, jlong value)
    {
        env->SetLongField(box, field, value);
    }
};

template <>
struct JB2NShim<jFloatBox, jfloat>
{
    static const char * GetFieldIdSignature()
    {
        return "F";
    }
    static jfloat GetValue(JNIEnv *env, jFloatBox box, jfieldID field)
    {
        return env->GetFloatField(box, field);
    }
    static void SetValue(JNIEnv *env, jFloatBox box, jfieldID field, jfloat value)
    {
        env->SetFloatField(box, field, value);
    }
};

template <>
struct JB2NShim<jDoubleBox, jdouble>
{
    static const char * GetFieldIdSignature()
    {
        return "D";
    }
    static jdouble GetValue(JNIEnv *env, jDoubleBox box, jfieldID field)
    {
        return env->GetDoubleField(box, field);
    }
    static void SetValue(JNIEnv *env, jDoubleBox box, jfieldID field, jdouble value)
    {
        env->SetDoubleField(box, field, value);
    }
};
