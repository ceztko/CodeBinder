/**
 * SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#pragma once

#include "JNITypesPrivate.h"
#include <CBBaseTypes.h>

template <typename TJBoxed, typename TNative>
class OPTJ2NImplBase
{
protected:
    static TNative getValue(JNIEnv* env, TJBoxed boxed, jfieldID fieldId)
    {
        (void)env;
        (void)boxed;
        (void)fieldId;
        static_assert(always_false<TJBoxed, TNative>, "Not implemented");
        return TNative{ };
    }

    static const char* getFieldIdSignature()
    {
        static_assert(always_false<TJBoxed, TNative>, "Not implemented");
        return nullptr;
    }
};

template <>
class OPTJ2NImplBase<jBoolean, cbbool>
{
protected:
    cbbool getValue(JNIEnv* env, jBoolean boxed, jfieldID fieldId)
    {
        return (cbbool)env->GetBooleanField((jobject)boxed, fieldId);
    }

    const char* getFieldIdSignature()
    {
        return "Z";
    }
};

template <typename TOptional, typename TJBoxed, typename TNative>
class OPTJ2NImpl : public OPTJ2NImplBase<TJBoxed, TNative>
{
public:
    OPTJ2NImpl(JNIEnv* env, TJBoxed boxed)
    {
        if (boxed == nullptr)
        {
            m_Optional.has_value = (cbbool)false;
            m_Optional.value = TNative{ };
        }
        else
        {
            m_Optional.has_value = (cbbool)true;
            auto cls = env->GetObjectClass((jobject)boxed);
            auto fieldId = env->GetFieldID(cls, "value", this->getFieldIdSignature());
            m_Optional.value = this->getValue(env, boxed, fieldId);
        }
    }

    operator const TOptional&()
    {
        return m_Optional;
    }
private:
    TOptional m_Optional;
};

inline OPTJ2NImpl<cboptbool, jBoolean, cbbool> OPTJ2N(JNIEnv* env, jBoolean boxed)
{
    return OPTJ2NImpl<cboptbool, jBoolean, cbbool>(env, boxed);
}
