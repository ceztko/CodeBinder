/**
 * SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#include "JSNAPI.h"

namespace js
{
    extern "C" napi_value NAPI_CBCreateNativeHandle(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_CBCreateWeakNativeHandle(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_CBFreeNativeHandle(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_CBNativeHandleGetTarget(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_CBAdjustEternalMemory(
        napi_env env, napi_callback_info info);
}
