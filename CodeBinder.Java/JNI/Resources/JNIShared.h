/**
 * SPDX-FileCopyrightText: (C) 2021 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#pragma once

#include "JNITypesPrivate.h"

jlong GetHandle(JNIEnv* env, jHandleRef handleref);
JNIEnv* GetEnv();
JavaVM* GetJvm();
