/**
 * SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#include "JSNAPI.h"

namespace js
{
    extern "C" napi_value NAPI_CreateNativeHandle(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_CreateWeakNativeHandle(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_FreeNativeHandle(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_NativeHandleGetTarget(
        napi_env env, napi_callback_info info);
}
