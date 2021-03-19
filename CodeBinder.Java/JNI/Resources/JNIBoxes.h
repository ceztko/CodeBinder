/* This file was generated. DO NOT EDIT! */
#pragma once

#include "JNITypesPrivate.h"
#include <CBBaseTypes.h>

// Wraps java numerical box type
template <typename TJBox, typename TNative = typename TJBox::ValueType>
class BJ2NImpl
{
public:
    BJ2NImpl(JNIEnv* env, typename TJBox::BoxPtr box)
    {
        m_env = env;
        m_box = box;
        m_value = (TNative)box->GetValue(env);
    }
    ~BJ2NImpl()
    {
        m_box->SetValue(m_env, (typename TJBox::ValueType)m_value);
    }
public:
    inline operator TNative* () { return &m_value; }
public:
    TNative m_value;
    JNIEnv* m_env;
    typename TJBox::BoxPtr m_box;
};

// Wraps java string box type
class SBJ2N
{
public:
    SBJ2N(JNIEnv* env, jStringBox box);
    ~SBJ2N();

public:
    inline operator cbstring* () { return &m_value; }
private:
    JNIEnv* m_env;
    jStringBox m_box;
    jstring m_jstring;
    const char* m_chars;
    jboolean m_isCopy;
    cbstring m_value;
};

BJ2NImpl<_jBooleanBox> BJ2N(JNIEnv* env, jBooleanBox box);
BJ2NImpl<_jCharacterBox> BJ2N(JNIEnv* env, jCharacterBox box);
BJ2NImpl<_jByteBox> BJ2N(JNIEnv* env, jByteBox box);
BJ2NImpl<_jShortBox> BJ2N(JNIEnv* env, jShortBox box);
BJ2NImpl<_jIntegerBox> BJ2N(JNIEnv* env, jIntegerBox box);
BJ2NImpl<_jLongBox> BJ2N(JNIEnv* env, jLongBox box);
BJ2NImpl<_jFloatBox> BJ2N(JNIEnv* env, jFloatBox box);
BJ2NImpl<_jDoubleBox> BJ2N(JNIEnv* env, jDoubleBox box);
SBJ2N BJ2N(JNIEnv* env, jStringBox box);

template <typename TNative>
BJ2NImpl<_jBooleanBox, TNative> BJ2N(JNIEnv* env, jBooleanBox box)
{
    return BJ2NImpl<_jBooleanBox, TNative>(env, box);
}

template <typename TNative>
BJ2NImpl<_jCharacterBox, TNative> BJ2N(JNIEnv* env, jCharacterBox box)
{
    return BJ2NImpl<_jCharacterBox, TNative>(env, box);
}

template <typename TNative>
BJ2NImpl<_jByteBox, TNative> BJ2N(JNIEnv* env, jByteBox box)
{
    return BJ2NImpl<_jByteBox, TNative>(env, box);
}

template <typename TNative>
BJ2NImpl<_jShortBox, TNative> BJ2N(JNIEnv* env, jShortBox box)
{
    return BJ2NImpl<_jShortBox, TNative>(env, box);
}

template <typename TNative>
BJ2NImpl<_jIntegerBox, TNative> BJ2N(JNIEnv* env, jIntegerBox box)
{
    return BJ2NImpl<_jIntegerBox, TNative>(env, box);
}

template <typename TNative>
BJ2NImpl<_jLongBox, TNative> BJ2N(JNIEnv* env, jLongBox box)
{
    return BJ2NImpl<_jLongBox, TNative>(env, box);
}

template <typename TNative>
BJ2NImpl<_jFloatBox, TNative> BJ2N(JNIEnv* env, jFloatBox box)
{
    return BJ2NImpl<_jFloatBox, TNative>(env, box);
}

template <typename TNative>
BJ2NImpl<_jDoubleBox, TNative> BJ2N(JNIEnv* env, jDoubleBox box)
{
    return BJ2NImpl<_jDoubleBox, TNative>(env, box);
}
