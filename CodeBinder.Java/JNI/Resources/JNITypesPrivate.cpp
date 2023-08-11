/**
 * SPDX-FileCopyrightText: (C) 2021 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#include "JNITypesPrivate.h"

#include "JNIShared.h"

jlong _jHandleRef::getHandle(JNIEnv *env)
{
    return ::GetHandle(env, this);
}
