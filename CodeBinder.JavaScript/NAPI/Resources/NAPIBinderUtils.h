#include "JSNAPI.h"

namespace js
{
    extern "C" napi_value NAPI_CreateNativeHandleRef(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_CreateWeakNativeHandleRef(
        napi_env env, napi_callback_info info);

    extern "C" napi_value NAPI_FreeNativeHandleRef(
        napi_env env, napi_callback_info info);
}
