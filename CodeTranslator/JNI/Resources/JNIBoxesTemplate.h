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
public:
    T Value;
private:
    JNIEnv *m_env;
    TJBox m_box;
    bool m_commit;
};

template<typename TJBox, typename T>
JB2NImpl<TJBox, T>::JB2NImpl(JNIEnv *env, TJBox box, bool commit)
{
    m_env = env;
    m_box = box;
    m_commit = commit;
    Value = box->GetValue(env);
}

template<typename TJBox, typename T>
JB2NImpl<TJBox, T>::~JB2NImpl()
{
    if (m_commit)
        m_box->SetValue(m_env, Value);
}
