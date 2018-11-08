#include "JNITypesPrivate.h"

const char * _jBooleanBoxBase::getFieldIdSignature()
{
    return "Z";
}

jboolean _jBooleanBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetBooleanField((jobject)this, field);
}

void _jBooleanBoxBase::setValue(JNIEnv * env, jfieldID field, jboolean value)
{
    env->SetBooleanField(this, field, value);
}

const char * _jCharacterBoxBase::getFieldIdSignature()
{
    return "C";
}

jchar _jCharacterBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetCharField((jobject)this, field);
}

void _jCharacterBoxBase::setValue(JNIEnv * env, jfieldID field, jchar value)
{
    env->SetCharField(this, field, value);
}

const char * _jByteBoxBase::getFieldIdSignature()
{
    return "B";
}

jbyte _jByteBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetByteField((jobject)this, field);
}

void _jByteBoxBase::setValue(JNIEnv * env, jfieldID field, jbyte value)
{
    env->SetByteField(this, field, value);
}

const char * _jShortBoxBase::getFieldIdSignature()
{
    return "S";
}

jshort _jShortBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetShortField((jobject)this, field);
}

void _jShortBoxBase::setValue(JNIEnv * env, jfieldID field, jshort value)
{
    env->SetShortField(this, field, value);
}

const char * _jIntegerBoxBase::getFieldIdSignature()
{
    return "I";
}

jint _jIntegerBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetIntField((jobject)this, field);
}

void _jIntegerBoxBase::setValue(JNIEnv * env, jfieldID field, jint value)
{
    env->SetIntField(this, field, value);
}

const char * _jLongBoxBase::getFieldIdSignature()
{
    return "J";
}

jlong _jLongBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetLongField((jobject)this, field);
}

void _jLongBoxBase::setValue(JNIEnv * env, jfieldID field, jlong value)
{
    env->SetLongField(this, field, value);
}

const char * _jFloatBoxBase::getFieldIdSignature()
{
    return "F";
}

jfloat _jFloatBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetFloatField((jobject)this, field);
}

void _jFloatBoxBase::setValue(JNIEnv * env, jfieldID field, jfloat value)
{
    env->SetFloatField(this, field, value);
}

const char * _jDoubleBoxBase::getFieldIdSignature()
{
    return "D";
}

jdouble _jDoubleBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return env->GetDoubleField((jobject)this, field);
}

void _jDoubleBoxBase::setValue(JNIEnv * env, jfieldID field, jdouble value)
{
    env->SetDoubleField(this, field, value);
}

const char * _jStringBoxBase::getFieldIdSignature()
{
    return "Ljava/lang/String;";
}

jstring _jStringBoxBase::getValue(JNIEnv * env, jfieldID field) const
{
    return (jstring)env->GetObjectField((jobject)this, field);
}

void _jStringBoxBase::setValue(JNIEnv * env, jfieldID field, jstring value)
{
    env->SetObjectField(this, field, value);
}
