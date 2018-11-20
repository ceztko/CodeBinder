#pragma once

#include "JNITypes.h"

#undef jBooleanBox
#undef jCharacterBox
#undef jByteBox
#undef jShortBox
#undef jIntegerBox
#undef jLongBox
#undef jFloatBox
#undef jDoubleBox
#undef jStringBox

// Template for box types
template <typename BaseT>
class _jTypeBox : public BaseT
{
public:
    typename BaseT::Type GetValue(JNIEnv *env) const;
    void SetValue(JNIEnv *env, typename BaseT::Type value);
private:
    jfieldID getFieldId(JNIEnv *env) const;
};

////////////////////
// Base box types //
////////////////////

class _jBooleanBoxBase : public _jobject
{
public:
    typedef jboolean Type;
protected:
    static const char * getFieldIdSignature();
    jboolean getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jboolean value);
};

class _jCharacterBoxBase : public _jobject
{
public:
    typedef jboolean Type;
protected:
    static const char * getFieldIdSignature();
    jchar getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jchar value);
};

class _jByteBoxBase : public _jobject
{
public:
    typedef jbyte Type;
protected:
    static const char * getFieldIdSignature();
    jbyte getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jbyte value);
};

class _jShortBoxBase : public _jobject
{
public:
    typedef jshort Type;
protected:
    static const char * getFieldIdSignature();
    jshort getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jshort value);
};

class _jIntegerBoxBase : public _jobject
{
public:
    typedef jint Type;
protected:
    static const char * getFieldIdSignature();
    jint getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jint value);
};

class _jLongBoxBase : public _jobject
{
public:
    typedef jlong Type;
protected:
    static const char * getFieldIdSignature();
    jlong getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jlong value);
};

class _jFloatBoxBase : public _jobject
{
public:
    typedef jfloat Type;
protected:
    static const char * getFieldIdSignature();
    jfloat getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jfloat value);
};

class _jDoubleBoxBase : public _jobject
{
public:
    typedef jdouble Type;
protected:
    static const char * getFieldIdSignature();
    jdouble getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jdouble  value);
};

class _jStringBoxBase : public _jobject
{
public:
    typedef jstring Type;
protected:
    static const char * getFieldIdSignature();
    jstring getValue(JNIEnv *env, jfieldID field) const;
    void setValue(JNIEnv *env, jfieldID field, jstring  value);
};

// Typedef for box types
class _jBooleanBox : public _jTypeBox<_jBooleanBoxBase> { };
class _jCharacterBox : public _jTypeBox<_jCharacterBoxBase> { };
class _jByteBox : public _jTypeBox<_jByteBoxBase> { };
class _jShortBox : public _jTypeBox<_jShortBoxBase> { };
class _jIntegerBox : public _jTypeBox<_jIntegerBoxBase> { };
class _jLongBox : public _jTypeBox<_jLongBoxBase> { };
class _jFloatBox : public _jTypeBox<_jFloatBoxBase> { };
class _jDoubleBox : public _jTypeBox<_jDoubleBoxBase> { };
class _jStringBox : public _jTypeBox<_jStringBoxBase> { };

// Typedef for box type pointers
typedef _jBooleanBox * jBooleanBox;
typedef _jCharacterBox * jCharacterBox;
typedef _jByteBox * jByteBox;
typedef _jShortBox * jShortBox;
typedef _jIntegerBox * jIntegerBox;
typedef _jLongBox * jLongBox;
typedef _jFloatBox * jFloatBox;
typedef _jDoubleBox * jDoubleBox;
typedef _jStringBox * jStringBox;

template<typename BaseT>
typename BaseT::Type _jTypeBox<BaseT>::GetValue(JNIEnv *env) const
{
    return this->getValue(env, getFieldId(env));
}

template<typename BaseT>
void _jTypeBox<BaseT>::SetValue(JNIEnv *env, typename BaseT::Type value)
{
    this->setValue(env, getFieldId(env), value);
}

template<typename BaseT>
jfieldID _jTypeBox<BaseT>::getFieldId(JNIEnv *env) const
{
    auto cls = env->GetObjectClass((jobject)this);
    return env->GetFieldID(cls, "value", this->getFieldIdSignature());
}
