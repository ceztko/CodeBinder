/**
 * SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

#define LOAD_SYMBOL(module, symbol) js::symbol = (decltype(::symbol)*)GetProcAddress(module, #symbol)
#else
#include <dlfcn.h>
#define LOAD_SYMBOL(module, symbol) js::symbol = (decltype(::symbol)*)dlsym(module, #symbol)
#endif

namespace
{
    struct Init
    {
        Init();
    };
}

static Init s_init;

#define DEFINE_NAPI_SYMBOLS
#include "JSInterop.h"

namespace js
{
    extern napi_ref s_AddonThisRef;

    napi_value GetAddonThis()
    {
        napi_value addonThisObj;
        napi_status status = napi_get_reference_value(s_Env, s_AddonThisRef, &addonThisObj);
        assert(status == napi_ok);
        return addonThisObj;
    }
}

Init::Init()
{
    // Resolve napi symbols
#ifdef _WIN32
    auto module = GetModuleHandle(nullptr);
#else
    auto module = dlopen(nullptr, RTLD_LAZY);
#endif
    LOAD_SYMBOL(module, napi_typeof);
    LOAD_SYMBOL(module, napi_get_cb_info);
    LOAD_SYMBOL(module, napi_throw_error);
    LOAD_SYMBOL(module, napi_throw_type_error);
    LOAD_SYMBOL(module, napi_throw_range_error);
    LOAD_SYMBOL(module, napi_get_undefined);
    LOAD_SYMBOL(module, napi_get_null);
    LOAD_SYMBOL(module, napi_strict_equals);
    LOAD_SYMBOL(module, napi_get_global);
    LOAD_SYMBOL(module, napi_get_boolean);
    LOAD_SYMBOL(module, napi_get_value_string_utf8);
    LOAD_SYMBOL(module, napi_create_string_utf8);
    LOAD_SYMBOL(module, napi_get_array_length);
    LOAD_SYMBOL(module, napi_get_value_bool);
    LOAD_SYMBOL(module, napi_get_value_uint32);
    LOAD_SYMBOL(module, napi_get_value_int32);
    LOAD_SYMBOL(module, napi_get_value_bigint_uint64);
    LOAD_SYMBOL(module, napi_get_value_bigint_int64);
    LOAD_SYMBOL(module, napi_get_value_double);
    LOAD_SYMBOL(module, napi_get_value_external);
    LOAD_SYMBOL(module, napi_create_arraybuffer);
    LOAD_SYMBOL(module, napi_create_external_arraybuffer);
    LOAD_SYMBOL(module, napi_create_typedarray);
    LOAD_SYMBOL(module, napi_get_typedarray_info);
    LOAD_SYMBOL(module, napi_create_uint32);
    LOAD_SYMBOL(module, napi_create_int32);
    LOAD_SYMBOL(module, napi_create_bigint_uint64);
    LOAD_SYMBOL(module, napi_create_bigint_int64);
    LOAD_SYMBOL(module, napi_create_double);
    LOAD_SYMBOL(module, napi_create_external);
    LOAD_SYMBOL(module, napi_get_named_property);
    LOAD_SYMBOL(module, napi_set_named_property);
    LOAD_SYMBOL(module, napi_define_properties);
    LOAD_SYMBOL(module, napi_call_function);
    LOAD_SYMBOL(module, napi_new_instance);
    LOAD_SYMBOL(module, napi_is_exception_pending);
    LOAD_SYMBOL(module, napi_reference_ref);
    LOAD_SYMBOL(module, napi_reference_unref);
    LOAD_SYMBOL(module, napi_create_reference);
    LOAD_SYMBOL(module, napi_delete_reference);
    LOAD_SYMBOL(module, napi_has_named_property);
    LOAD_SYMBOL(module, napi_create_function);
    LOAD_SYMBOL(module, napi_create_object);
    LOAD_SYMBOL(module, napi_add_finalizer);
    LOAD_SYMBOL(module, napi_get_reference_value);
    LOAD_SYMBOL(module, napi_get_last_error_info);
}
