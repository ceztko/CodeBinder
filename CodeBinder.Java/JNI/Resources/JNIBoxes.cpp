#include "JNIBoxes.h"

#include <CBInterop.h>

SBJ2N::SBJ2N(JNIEnv* env, jStringBox box)
    : m_env(env), m_box(box)
{
    m_jstring = box->GetValue(env);
    if (m_jstring == nullptr)
    {
        m_chars = nullptr;
        m_isCopy = false;
        m_value = { };
    }
    else
    {
        m_chars = m_env->GetStringUTFChars(m_jstring, &m_isCopy);
        jsize length = env->GetStringUTFLength(m_jstring);
        m_value = CBCreateStringViewLen(m_chars, (size_t)length);
    }
}

SBJ2N::~SBJ2N()
{
    if (m_value.data != m_chars)
    {
        if (m_value.data == nullptr)
            m_box->SetValue(m_env, nullptr);
        else
            m_box->SetValue(m_env, m_env->NewStringUTF(m_value.data));
    }

    if (m_isCopy)
        m_env->ReleaseStringUTFChars(m_jstring, m_chars);

    CBFreeString(&m_value);
}

BJ2NImpl<_jBooleanBox> BJ2N(JNIEnv * env, jBooleanBox box)
{
    return BJ2NImpl<_jBooleanBox>(env, box);
}

BJ2NImpl<_jCharacterBox> BJ2N(JNIEnv * env, jCharacterBox box)
{
    return BJ2NImpl<_jCharacterBox>(env, box);
}

BJ2NImpl<_jByteBox> BJ2N(JNIEnv * env, jByteBox box)
{
    return BJ2NImpl<_jByteBox>(env, box);
}

BJ2NImpl<_jShortBox> BJ2N(JNIEnv * env, jShortBox box)
{
    return BJ2NImpl<_jShortBox>(env, box);
}

BJ2NImpl<_jIntegerBox> BJ2N(JNIEnv * env, jIntegerBox box)
{
    return BJ2NImpl<_jIntegerBox>(env, box);
}

BJ2NImpl<_jLongBox> BJ2N(JNIEnv * env, jLongBox box)
{
    return BJ2NImpl<_jLongBox>(env, box);
}

BJ2NImpl<_jFloatBox> BJ2N(JNIEnv * env, jFloatBox box)
{
    return BJ2NImpl<_jFloatBox>(env, box);
}

BJ2NImpl<_jDoubleBox> BJ2N(JNIEnv * env, jDoubleBox box)
{
    return BJ2NImpl<_jDoubleBox>(env, box);
}

SBJ2N BJ2N(JNIEnv* env, jStringBox box)
{
    return SBJ2N(env, box);
}
