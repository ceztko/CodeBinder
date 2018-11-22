#pragma once

#include <stdexcept>
#include "JNITypesPrivate.h"

// Wraps custom java box type
template <typename TJBox, typename TN = typename TJBox::ValueType>
class BJ2NImpl
{
public:
    BJ2NImpl(JNIEnv *env, typename TJBox::BoxPtr box, bool commit);
    ~BJ2NImpl();
public:
    inline TN * ptr() { return &Value; }
    inline TN & ref() { return Value; }
    inline operator TN *() { return &Value; }
    inline operator TN &() { return Value; }
public:
    TN Value;
private:
    JNIEnv *m_env;
    typename TJBox::BoxPtr m_box;
    bool m_commit;
};

template<typename TJBox, typename TN>
BJ2NImpl<TJBox, TN>::BJ2NImpl(JNIEnv *env, typename TJBox::BoxPtr box, bool commit)
{
    m_env = env;
    m_box = box;
    m_commit = commit;
    Value = (TN)box->GetValue(env);
}

template<typename TJBox, typename TN>
BJ2NImpl<TJBox, TN>::~BJ2NImpl()
{
    if (m_commit)
        m_box->SetValue(m_env, (typename TJBox::ValueType)Value);
}
