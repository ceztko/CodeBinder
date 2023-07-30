#include "NAPIBinderUtils.h"
#include "JSInterop.h"

namespace js
{
    extern "C" napi_value NAPI_CreateNativeHandle(
        napi_env env, napi_callback_info info)
    {
        napi_status status;
        size_t argc = 1;
        napi_value args[1];
        status = napi_get_cb_info(env, info, &argc, args, nullptr, nullptr);
        assert(status == napi_ok);

        napi_ref ref;
        napi_create_reference(env, args[0], 1, &ref);
        assert(status == napi_ok);
        return CreateNapiValue(env, ref);
    }

    extern "C" napi_value NAPI_CreateWeakNativeHandle(
        napi_env env, napi_callback_info info)
    {
        napi_status status;
        size_t argc = 1;
        napi_value args[1];
        status = napi_get_cb_info(env, info, &argc, args, nullptr, nullptr);
        assert(status == napi_ok);

        napi_ref ref;
        status = napi_create_reference(env, args[0], 0, &ref);
        assert(status == napi_ok);
        return CreateNapiValue(env, ref);
    }

    extern "C" napi_value NAPI_FreeNativeHandle(
        napi_env env, napi_callback_info info)
    {
        napi_status status;
        size_t argc = 1;
        napi_value args[1];
        status = napi_get_cb_info(env, info, &argc, args, nullptr, nullptr);
        assert(status == napi_ok);

        napi_ref ref = (napi_ref)GetPtrFromNapiValue(env, args[0]);
        status = napi_delete_reference(env, ref);
        assert(status == napi_ok);

        return nullptr;
    }

    extern "C" napi_value NAPI_NativeHandleGetTarget(
        napi_env env, napi_callback_info info)
    {
        napi_status status;
        size_t argc = 1;
        napi_value args[1];
        status = napi_get_cb_info(env, info, &argc, args, nullptr, nullptr);
        assert(status == napi_ok);

        napi_ref ref = (napi_ref)GetPtrFromNapiValue(env, args[0]);
        napi_value ret;
        status = napi_get_reference_value(env, ref, &ret);
        assert(status == napi_ok);

        return ret;
    }
}
