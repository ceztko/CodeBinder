#include "NAPIBinderUtils.h"
#include "JSInterop.h"

namespace js
{
    extern "C" napi_value NAPI_CreateNativeHandleRef(
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

    extern "C" napi_value NAPI_CreateWeakNativeHandleRef(
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

    extern "C" napi_value NAPI_FreeNativeHandleRef(
        napi_env env, napi_callback_info info)
    {
        napi_status status;
        size_t argc = 1;
        napi_value args[1];
        status = napi_get_cb_info(env, info, &argc, args, nullptr, nullptr);
        assert(status == napi_ok);

        napi_ref ref = (napi_ref)GetInt64FromNapiValue(env, args[0]);
        status = napi_delete_reference(env, ref);
        assert(status == napi_ok);

        return nullptr;
    }
}
