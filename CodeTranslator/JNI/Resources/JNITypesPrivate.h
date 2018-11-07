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

class _jBooleanBox : public _jobject {};
class _jCharacterBox : public _jobject {};
class _jByteBox : public _jobject {};
class _jShortBox : public _jobject {};
class _jIntegerBox : public _jobject {};
class _jLongBox : public _jobject {};
class _jFloatBox : public _jobject {};
class _jDoubleBox : public _jobject {};

typedef _jBooleanBox * jBooleanBox;
typedef _jCharacterBox * jCharacterBox;
typedef _jByteBox * jByteBox;
typedef _jShortBox * jShortBox;
typedef _jIntegerBox * jIntegerBox;
typedef _jLongBox * jLongBox;
typedef _jFloatBox * jFloatBox;
typedef _jDoubleBox * jDoubleBox;
