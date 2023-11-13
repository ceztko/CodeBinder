/**
 * SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
 * SPDX-License-Identifier: MIT-0
 */

#pragma once

#include <cassert>
#include <cstring>
#include <vector>
#include <type_traits>

#define BUILDING_NODE_EXTENSION
#include <node_api.h>

#include "JSNAPI.h"

#include <CBInterop.hpp>

#ifdef DEFINE_NAPI_SYMBOLS
#define DECLARE_SYMBOL(symbol) decltype(::symbol)* symbol
#else
#define DECLARE_SYMBOL(symbol) extern decltype(::symbol)* symbol
#endif

// https://artificial-mind.net/blog/2020/10/03/always-false
template <class... T>
constexpr bool always_false = false;

#define CHECK_JS_EXCEPTION(env) if (js::IsJSExceptionPending(env))\
    throw cb::StackUnwinder();

namespace js
{
    extern napi_env s_Env;

    // Alias napi symbols
    DECLARE_SYMBOL(napi_typeof);
    DECLARE_SYMBOL(napi_get_cb_info);
    DECLARE_SYMBOL(napi_throw_error);
    DECLARE_SYMBOL(napi_throw_type_error);
    DECLARE_SYMBOL(napi_throw_range_error);
    DECLARE_SYMBOL(napi_get_undefined);
    DECLARE_SYMBOL(napi_get_null);
    DECLARE_SYMBOL(napi_strict_equals);
    DECLARE_SYMBOL(napi_get_global);
    DECLARE_SYMBOL(napi_get_boolean);
    DECLARE_SYMBOL(napi_get_value_string_utf8);
    DECLARE_SYMBOL(napi_create_string_utf8);
    DECLARE_SYMBOL(napi_get_array_length);
    DECLARE_SYMBOL(napi_get_value_bool);
    DECLARE_SYMBOL(napi_get_value_uint32);
    DECLARE_SYMBOL(napi_get_value_int32);
    DECLARE_SYMBOL(napi_get_value_bigint_uint64);
    DECLARE_SYMBOL(napi_get_value_bigint_int64);
    DECLARE_SYMBOL(napi_get_value_double);
    DECLARE_SYMBOL(napi_get_value_external);
    DECLARE_SYMBOL(napi_create_arraybuffer);
    DECLARE_SYMBOL(napi_create_external_arraybuffer);
    DECLARE_SYMBOL(napi_create_typedarray);
    DECLARE_SYMBOL(napi_get_typedarray_info);
    DECLARE_SYMBOL(napi_create_uint32);
    DECLARE_SYMBOL(napi_create_int32);
    DECLARE_SYMBOL(napi_create_bigint_uint64);
    DECLARE_SYMBOL(napi_create_bigint_int64);
    DECLARE_SYMBOL(napi_create_double);
    DECLARE_SYMBOL(napi_create_external);
    DECLARE_SYMBOL(napi_get_named_property);
    DECLARE_SYMBOL(napi_set_named_property);
    DECLARE_SYMBOL(napi_define_properties);
    DECLARE_SYMBOL(napi_call_function);
    DECLARE_SYMBOL(napi_new_instance);
    DECLARE_SYMBOL(napi_is_exception_pending);
    DECLARE_SYMBOL(napi_reference_ref);
    DECLARE_SYMBOL(napi_reference_unref);
    DECLARE_SYMBOL(napi_create_reference);
    DECLARE_SYMBOL(napi_delete_reference);
    DECLARE_SYMBOL(napi_has_named_property);
    DECLARE_SYMBOL(napi_create_function);
    DECLARE_SYMBOL(napi_create_object);
    DECLARE_SYMBOL(napi_add_finalizer);
    DECLARE_SYMBOL(napi_get_reference_value);
    DECLARE_SYMBOL(napi_get_last_error_info);

    // Adapter class to find the correct typed array type
    template <typename TNArray>
    struct TArrShim
    {
        static napi_typedarray_type GetType()
        {
            static_assert(always_false<TNArray>, "Not implemented");
            return (napi_typedarray_type)0;
        }
    };

    napi_value GetAddonThis();

    inline bool IsNull(napi_env env, napi_value value)
    {
        napi_value nullval;
        napi_get_null(env, &nullval);

        bool ret;
        napi_strict_equals(env, value, nullval, &ret);
        return ret;
    }

    inline bool IsUndefined(napi_env env, napi_value value)
    {
        napi_value undefvalue;
        napi_get_undefined(env, &undefvalue);

        bool ret;
        napi_strict_equals(env, value, undefvalue, &ret);
        return ret;
    }

    inline cbbool GetBoolFromNapiValue(napi_env env, napi_value value)
    {
        bool ret;
        napi_get_value_bool(env, value, &ret);
        return (cbbool)ret;
    }

    inline int8_t GetInt8FromNapiValue(napi_env env, napi_value value)
    {
        int32_t ret;
        napi_get_value_int32(env, value, &ret);
        return (int8_t)ret;
    }

    inline uint8_t GetUInt8FromNapiValue(napi_env env, napi_value value)
    {
        uint32_t ret;
        napi_get_value_uint32(env, value, &ret);
        return (uint8_t)ret;
    }

    inline int16_t GetInt16FromNapiValue(napi_env env, napi_value value)
    {
        int32_t ret;
        napi_get_value_int32(env, value, &ret);
        return (int16_t)ret;
    }

    inline uint16_t GetUInt16FromNapiValue(napi_env env, napi_value value)
    {
        uint32_t ret;
        napi_get_value_uint32(env, value, &ret);
        return (uint16_t)ret;
    }

    inline int32_t GetInt32FromNapiValue(napi_env env, napi_value value)
    {
        int32_t ret;
        napi_get_value_int32(env, value, &ret);
        return ret;
    }

    inline uint32_t GetUInt32FromNapiValue(napi_env env, napi_value value)
    {
        uint32_t ret;
        napi_get_value_uint32(env, value, &ret);
        return ret;
    }

    inline uint64_t GetInt64FromNapiValue(napi_env env, napi_value value)
    {
        int64_t ret;
        bool lossless;
        napi_get_value_bigint_int64(env, value, &ret, &lossless);
        return ret;
    }

    inline uint64_t GetUInt64FromNapiValue(napi_env env, napi_value value)
    {
        uint64_t ret;
        bool lossless;
        napi_get_value_bigint_uint64(env, value, &ret, &lossless);
        return ret;
    }

    inline float GetFloatFromNapiValue(napi_env env, napi_value value)
    {
        double ret;
        napi_get_value_double(env, value, &ret);
        return (float)ret;
    }

    inline double GetDoubleFromNapiValue(napi_env env, napi_value value)
    {
        double ret;
        napi_get_value_double(env, value, &ret);
        return (double)ret;
    }

    inline void* GetHandleRefPtrFromNapiValue(napi_env env, napi_value value)
    {
        napi_value napi_ptr;
        napi_get_named_property(env, value, "handle", &napi_ptr);

        double ret;
        napi_get_value_double(env, napi_ptr, &ret);
        return (void*)reinterpret_cast<uint64_t&>(ret);
    }

    inline void* GetPtrFromNapiValue(napi_env env, napi_value value)
    {
        double ret;
        napi_get_value_double(env, value, &ret);
        return (void*)reinterpret_cast<uint64_t&>(ret);
    }

    inline cbstring CreateCBStringFromNapiValue(napi_env env, napi_value str)
    {
        size_t len;
        napi_get_value_string_utf8(env, str, nullptr, 0, &len);
        cbstring ret = CBCreateStringFixed(len);
        napi_get_value_string_utf8(env, str, (char*)ret.data, len + 1, nullptr);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, bool value)
    {
        napi_value ret;
        napi_get_boolean(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, uint8_t value)
    {
        napi_value ret;
        napi_create_uint32(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, int8_t value)
    {
        napi_value ret;
        napi_create_int32(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, uint16_t value)
    {
        napi_value ret;
        napi_create_uint32(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, int16_t value)
    {
        napi_value ret;
        napi_create_int32(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, uint32_t value)
    {
        napi_value ret;
        napi_create_uint32(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, int32_t value)
    {
        napi_value ret;
        napi_create_int32(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, uint64_t value)
    {
        napi_value ret;
        napi_create_bigint_uint64(env, (int64_t)value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, int64_t value)
    {
        napi_value ret;
        napi_create_bigint_int64(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, float value)
    {
        napi_value ret;
        napi_create_double(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, double value)
    {
        napi_value ret;
        napi_create_double(env, value, &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, const cbstring& str)
    {
        napi_value ret;
        napi_create_string_utf8(env, str.data, CBStringGetLength(&str), &ret);
        return ret;
    }

    inline napi_value CreateNapiValue(napi_env env, const void* ptr)
    {
        napi_value ret;
        napi_create_double(env, *reinterpret_cast<const double*>((const uint64_t*)&ptr), &ret);
        return ret;
    }

    inline bool IsJSExceptionPending(napi_env env)
    {
        bool ret;
        napi_is_exception_pending(env, &ret);
        return ret;
    }

    template <typename TNArray>
    inline napi_value CreateNapiValue(napi_env env, TNArray* arr, size_t size)
    {
        napi_value arrayBuffer;
        napi_status status = napi_create_external_arraybuffer(env, (void*)arr, size, nullptr, nullptr, &arrayBuffer);
        if (status == napi_no_external_buffers_allowed)
        {
            void* data;
            napi_create_arraybuffer(env, size, &data, &arrayBuffer);
            std::memcpy(data, arr, size * sizeof(TNArray));
        }

        napi_value ret;
        napi_create_typedarray(env, TArrShim<typename std::remove_const<TNArray>::type>::GetType(), size, arrayBuffer, 0, &ret);
        return ret;
    }

    // Adapter class to link correct JNI methods
    template <typename TNArray>
    struct AJSShim
    {
        static TNArray* GetNativeArray(napi_env env, napi_value arr)
        {
            void* data;
            size_t length;
            napi_get_typedarray_info(env, arr, nullptr, &length, &data, nullptr, nullptr);
            return (TNArray*)data;
        }

        static void FreeNativeArray(napi_env env, napi_value jsarray, TNArray* narray, bool commit)
        {
            (void)env;
            (void)jsarray;
            (void)narray;
            (void)commit;
            // Do nothing
        }
    };

    template <typename TNArray, typename... Args>
    class AJS2N;

    // Wraps java array and convert to native one
    template <typename TNArray>
    class AJS2N<TNArray>
    {
    public:
        AJS2N(napi_env env, napi_value arr, bool commit)
            : m_env(env), m_jsarray(arr), m_commit(commit)
        {
            if (IsNull(env, arr))
                m_narray = nullptr;
            else
                m_narray = AJSShim<TNArray>::GetNativeArray(env, arr);
        }
        ~AJS2N()
        {
            AJSShim<TNArray>::FreeNativeArray(m_env, m_jsarray, m_narray, m_commit);
        }
    public:
        inline TNArray* n_array() const { return m_narray; }
        inline operator TNArray* () const { return m_narray; }
    private:
        napi_env m_env;
        napi_value m_jsarray;
        TNArray* m_narray;
        bool m_commit;
    };

    template <typename TNArray, typename TCArray, typename... Args>
    class AJS2N<TNArray, TCArray, Args...> : public AJS2N<TNArray, Args...>
    {
    public:
        using AJS2N<TNArray, Args...>::AJS2N;
    public:
        inline operator TCArray* () const { return (TCArray*)AJS2N<TNArray, Args...>::n_array(); }
    };

    template <>
    struct TArrShim<uint8_t>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_uint8_array;
        }
    };

    template <>
    struct TArrShim<int8_t>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_int8_array;
        }
    };

    template <>
    struct TArrShim<uint16_t>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_uint16_array;
        }
    };

    template <>
    struct TArrShim<int16_t>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_int16_array;
        }
    };

    template <>
    struct TArrShim<uint32_t>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_uint32_array;
        }
    };

    template <>
    struct TArrShim<int32_t>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_int32_array;
        }
    };

    template <>
    struct TArrShim<uint64_t>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_biguint64_array;
        }
    };

    template <>
    struct TArrShim<int64_t>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_bigint64_array;
        }
    };

    template <>
    struct TArrShim<float>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_float32_array;
        }
    };

    template <>
    struct TArrShim<double>
    {
        static napi_typedarray_type GetType()
        {
            return napi_typedarray_type::napi_float64_array;
        }
    };

    template <>
    struct AJSShim<void*>
    {
        static void** GetNativeArray(napi_env env, napi_value jsarray)
        {
            void* data;
            size_t length;
            napi_get_typedarray_info(env, jsarray, nullptr, &length, &data, nullptr, nullptr);
#if __LP64__ || _WIN64
            return (void**)data;
#else
            return new void* [length];
#endif
        }
        static void FreeNativeArray(napi_env env, napi_value jsarray, void** narray, bool commit)
        {
#if !defined(__LP64__) && !defined(_WIN64)
            if (commit)
            {
                void* data;
                size_t length;
                napi_get_typedarray_info(env, jsarray, nullptr, &length, &data, nullptr, nullptr);
                for (unsigned i = 0; i < length; i++)
                    ((uint64_t*)data)[i] = (uint64_t)narray[i];
            }

            delete[] narray;
#endif
        }
    };


    template <typename TNative>
    struct BJS2NShim final
    {
        static TNative Acquire(napi_env env, napi_value value)
        {
            (void)env;
            (void)value;
            return { };
            static_assert(always_false<TNative>, "Not implemented");
        }

        static napi_value Release(napi_env env, TNative nvalue)
        {
            (void)env;
            (void)nvalue;
            return nullptr;
            static_assert(always_false<TNative>, "Not implemented");
        }
    };

    template <typename TNative, typename TStorage = TNative>
    class BJS2N final
    {
    public:
        BJS2N(napi_env env, napi_value box)
            : m_env(env), m_jsbox(box)
        {
            napi_value box_value;
            napi_get_named_property(env, box, "value", &box_value);
            m_nvalue = (TNative)BJS2NShim<TStorage>::Acquire(env, box_value);
        }
        ~BJS2N()
        {
            auto value = BJS2NShim<TStorage>::Release(m_env, (TStorage)m_nvalue);
            napi_set_named_property(m_env, m_jsbox, "value", value);
        }
    public:
        inline operator TNative* () { return &m_nvalue; }
    private:
        napi_env m_env;
        napi_value m_jsbox;
        TNative m_nvalue;
    };

    template <>
    struct BJS2NShim<bool>
    {
        static bool Acquire(napi_env env, napi_value value)
        {
            return GetBoolFromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, bool nvalue)
        {
            napi_value ret;
            napi_get_boolean(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<uint8_t>
    {
        static uint8_t Acquire(napi_env env, napi_value value)
        {
            return GetUInt8FromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, uint8_t nvalue)
        {
            napi_value ret;
            napi_create_uint32(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<int8_t>
    {
        static int8_t Acquire(napi_env env, napi_value value)
        {
            return GetInt8FromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, int8_t nvalue)
        {
            napi_value ret;
            napi_create_int32(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<uint16_t>
    {
        static uint16_t Acquire(napi_env env, napi_value value)
        {
            return GetUInt16FromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, uint16_t nvalue)
        {
            napi_value ret;
            napi_create_uint32(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<int16_t>
    {
        static int16_t Acquire(napi_env env, napi_value value)
        {
            return GetInt16FromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, int16_t nvalue)
        {
            napi_value ret;
            napi_create_int32(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<uint32_t>
    {
        static uint32_t Acquire(napi_env env, napi_value value)
        {
            return GetUInt32FromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, uint32_t nvalue)
        {
            napi_value ret;
            napi_create_uint32(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<int32_t>
    {
        static int32_t Acquire(napi_env env, napi_value value)
        {
            return GetInt32FromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, int32_t nvalue)
        {
            napi_value ret;
            napi_create_int32(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<uint64_t>
    {
        static uint64_t Acquire(napi_env env, napi_value value)
        {
            return GetUInt64FromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, uint64_t nvalue)
        {
            napi_value ret;
            napi_create_bigint_uint64(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<int64_t>
    {
        static int64_t Acquire(napi_env env, napi_value value)
        {
            return GetInt64FromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, int64_t nvalue)
        {
            napi_value ret;
            napi_create_bigint_int64(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<float>
    {
        static float Acquire(napi_env env, napi_value value)
        {
            return GetFloatFromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, float nvalue)
        {
            napi_value ret;
            napi_create_double(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<double>
    {
        static double Acquire(napi_env env, napi_value value)
        {
            return GetDoubleFromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, double nvalue)
        {
            napi_value ret;
            napi_create_double(env, nvalue, &ret);
            return ret;
        }
    };

    template <>
    struct BJS2NShim<cbstring>
    {
        static cbstring Acquire(napi_env env, napi_value value)
        {
            return CreateCBStringFromNapiValue(env, value);
        }
        static napi_value Release(napi_env env, cbstring nvalue)
        {
            napi_value ret;
            napi_create_string_utf8(env, nvalue.data, CBStringGetLength(&nvalue), &ret);
            CBFreeString(&nvalue);
            return ret;
        }
    };
}
