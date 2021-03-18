/* This file was generated. DO NOT EDIT! */
#pragma once

#include "JNITypesPrivate.h"
#include <CBBaseTypes.h>

// Wraps custom java box type
template <typename TJBox, typename TN = typename TJBox::ValueType>
class BJ2NImpl
{
public:
    BJ2NImpl(JNIEnv* env, typename TJBox::BoxPtr box, bool commit)
    {
        m_env = env;
        m_box = box;
        m_commit = commit;
        Value = (TN)box->GetValue(env);
    }
    ~BJ2NImpl()
    {
        if (m_commit)
            m_box->SetValue(m_env, (typename TJBox::ValueType)Value);
    }
public:
    inline TN* ptr() { return &Value; }
    inline TN& ref() { return Value; }
    inline operator TN* () { return &Value; }
    inline operator TN& () { return Value; }
public:
    TN Value;
private:
    JNIEnv* m_env;
    typename TJBox::BoxPtr m_box;
    bool m_commit;
};

class SBJ2N
{
public:
    SBJ2N(JNIEnv* env, jStringBox box, bool commit)
    {
        m_env = env;
        m_box = box;
        m_commit = commit;
        // TODO
    }
    ~SBJ2N()
    {
        // TODO
    }

public:
    inline operator cbstring* () { return &Value; }
private:
    cbstring Value;
    JNIEnv* m_env;
    jStringBox m_box;
    bool m_commit;
};

BJ2NImpl<_jBooleanBox> BJ2N(JNIEnv* env, jBooleanBox box, bool commit = true);
BJ2NImpl<_jCharacterBox> BJ2N(JNIEnv* env, jCharacterBox box, bool commit = true);
BJ2NImpl<_jByteBox> BJ2N(JNIEnv* env, jByteBox box, bool commit = true);
BJ2NImpl<_jShortBox> BJ2N(JNIEnv* env, jShortBox box, bool commit = true);
BJ2NImpl<_jIntegerBox> BJ2N(JNIEnv* env, jIntegerBox box, bool commit = true);
BJ2NImpl<_jLongBox> BJ2N(JNIEnv* env, jLongBox box, bool commit = true);
BJ2NImpl<_jFloatBox> BJ2N(JNIEnv* env, jFloatBox box, bool commit = true);
BJ2NImpl<_jDoubleBox> BJ2N(JNIEnv* env, jDoubleBox box, bool commit = true);
SBJ2N BJ2N(JNIEnv* env, jStringBox box, bool commit = true);

template <typename TN>
BJ2NImpl<_jBooleanBox, TN> BJ2N(JNIEnv* env, jBooleanBox box, bool commit = true)
{
    return BJ2NImpl<_jBooleanBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jCharacterBox, TN> BJ2N(JNIEnv* env, jCharacterBox box, bool commit = true)
{
    return BJ2NImpl<_jCharacterBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jByteBox, TN> BJ2N(JNIEnv* env, jByteBox box, bool commit = true)
{
    return BJ2NImpl<_jByteBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jShortBox, TN> BJ2N(JNIEnv* env, jShortBox box, bool commit = true)
{
    return BJ2NImpl<_jShortBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jIntegerBox, TN> BJ2N(JNIEnv* env, jIntegerBox box, bool commit = true)
{
    return BJ2NImpl<_jIntegerBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jLongBox, TN> BJ2N(JNIEnv* env, jLongBox box, bool commit = true)
{
    return BJ2NImpl<_jLongBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jFloatBox, TN> BJ2N(JNIEnv* env, jFloatBox box, bool commit = true)
{
    return BJ2NImpl<_jFloatBox, TN>(env, box, commit);
}

template <typename TN>
BJ2NImpl<_jDoubleBox, TN> BJ2N(JNIEnv* env, jDoubleBox box, bool commit = true)
{
    return BJ2NImpl<_jDoubleBox, TN>(env, box, commit);
}
