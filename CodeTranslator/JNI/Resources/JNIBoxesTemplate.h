#pragma once

#include <stdexcept>
#include "JNITypesPrivate.h"

// Wraps custom java box type
template <typename TJBox, typename T>
class BJ2NImpl
{
public:
    BJ2NImpl(JNIEnv *env, TJBox box, bool commit);
    ~BJ2NImpl();
public:
    inline T * ptr() { return &Value; }
    inline T & ref() { return Value; }
    inline operator T *() { return &Value; }
    inline operator T &() { return Value; }
public:
    T Value;
private:
    JNIEnv *m_env;
    TJBox m_box;
    bool m_commit;
};

template<typename TJBox, typename T>
BJ2NImpl<TJBox, T>::BJ2NImpl(JNIEnv *env, TJBox box, bool commit)
{
    m_env = env;
    m_box = box;
    m_commit = commit;
    Value = box->GetValue(env);
}

template<typename TJBox, typename T>
BJ2NImpl<TJBox, T>::~BJ2NImpl()
{
    if (m_commit)
        m_box->SetValue(m_env, Value);
}
