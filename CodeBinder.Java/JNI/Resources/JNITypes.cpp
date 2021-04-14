#include "JNITypesPrivate.h"

const char * _jBooleanBoxBase::getFieldIdSignature()
{
    return "Z";
}

_jBooleanBoxBase::ValueType _jBooleanBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetBooleanField((jobject)this, field);
}

void _jBooleanBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetBooleanField(this, field, value);
}

const char * _jCharacterBoxBase::getFieldIdSignature()
{
    return "C";
}

_jCharacterBoxBase::ValueType _jCharacterBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetCharField((jobject)this, field);
}

void _jCharacterBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetCharField(this, field, value);
}

const char * _jByteBoxBase::getFieldIdSignature()
{
    return "B";
}

_jByteBoxBase::ValueType _jByteBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetByteField((jobject)this, field);
}

void _jByteBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetByteField(this, field, value);
}

const char * _jShortBoxBase::getFieldIdSignature()
{
    return "S";
}

_jShortBoxBase::ValueType _jShortBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetShortField((jobject)this, field);
}

void _jShortBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetShortField(this, field, value);
}

const char * _jIntegerBoxBase::getFieldIdSignature()
{
    return "I";
}

_jIntegerBoxBase::ValueType _jIntegerBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetIntField((jobject)this, field);
}

void _jIntegerBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetIntField(this, field, value);
}

const char * _jLongBoxBase::getFieldIdSignature()
{
    return "J";
}

_jLongBoxBase::ValueType _jLongBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetLongField((jobject)this, field);
}

void _jLongBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetLongField(this, field, value);
}

const char * _jFloatBoxBase::getFieldIdSignature()
{
    return "F";
}

_jFloatBoxBase::ValueType _jFloatBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetFloatField((jobject)this, field);
}

void _jFloatBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetFloatField(this, field, value);
}

const char * _jDoubleBoxBase::getFieldIdSignature()
{
    return "D";
}

_jDoubleBoxBase::ValueType _jDoubleBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetDoubleField((jobject)this, field);
}

void _jDoubleBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetDoubleField(this, field, value);
}

const char * _jStringBoxBase::getFieldIdSignature()
{
    return "Ljava/lang/String;";
}

_jStringBoxBase::ValueType _jStringBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return (jstring)env->GetObjectField((jobject)this, field);
}

void _jStringBoxBase::setValue(JNIEnv * env, jfieldID field, ValueType value)
{
    env->SetObjectField(this, field, value);
}
