﻿// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;

namespace CodeBinder.JavaScript.NAPI;

class NAPIMethodInitConversion : ConversionWriter
{
    NAPICompilationContext _compilation;

    public NAPIMethodInitConversion(NAPICompilationContext compilation)
    {
        _compilation = compilation;
    }

    protected override void write(CodeBuilder builder)
    {
        builder.AppendLine("#include <cassert>");
        builder.AppendLine("#include <vector>");
        builder.AppendLine("#include \"Internal/JSInterop.h\"");
        builder.AppendLine("#include \"Internal/NAPIBinderUtils.h\"");
        builder.AppendLine();

        foreach (var module in _compilation.Modules)
            builder.Append("#include \"NAPI").Append(module.Name).AppendLine(".h\"");

        builder.AppendLine();
        builder.AppendLine("#define DECLARE_NAPI_METHOD(name, func) { #name, 0, func, 0, 0, 0, napi_default, 0 }");

        builder.AppendLine();
        builder.AppendLine("""
#ifdef _MSC_VER

    #define EXPORT_ATTRIB __declspec(dllexport)

#else // Non MVSC

    #define EXPORT_ATTRIB __attribute__ ((visibility ("default")))

#endif
""");

builder.AppendLine();
        builder.AppendLine("namespace js");
        using (builder.Block())
        {
            builder.Append("napi_ref s_AddonThisRef").EndOfLine();
            builder.Append("napi_env s_Env").EndOfLine();
            builder.AppendLine();
            builder.AppendLine("""
extern "C" void Destructor(napi_env env, void* finalize_data, void* finalize_hint)
{
    (void)finalize_data;
    (void)finalize_hint;
    (void)napi_delete_reference(env, s_AddonThisRef);
    s_AddonThisRef = nullptr;
}
""");
            builder.AppendLine();
            builder.AppendLine("extern \"C\" napi_value CreateAddon(napi_env env, napi_callback_info info)");
            using (builder.Block())
            {
                builder.AppendLine("""
napi_status status;

if (s_AddonThisRef != nullptr)
{
    napi_throw_error(env, nullptr, "The addon was already initalized");
    return nullptr;
}

size_t argc = 1;
napi_value args[1];
status = napi_get_cb_info(env, info, &argc, args, NULL, NULL);
assert(status == napi_ok);

status = napi_create_reference(env, args[0], 1, &s_AddonThisRef);
assert(status == napi_ok);

napi_value obj;
status = napi_create_object(env, &obj);
assert(status == napi_ok);

status = napi_add_finalizer(env, obj, nullptr, Destructor, nullptr, nullptr);
assert(status == napi_ok);
""");
                builder.AppendLine();

                builder.AppendLine("napi_property_descriptor addDescriptor[] =");
                using (builder.Block(false))
                {
                    foreach (var module in _compilation.Modules)
                    {
                        foreach (var method in module.Methods)
                        {
                            string? condition = null;
                            if (method.TryGetAttribute<ConditionAttribute>(_compilation, out var attr))
                            {
                                condition = attr.GetConstructorArgument<string>(0);
                                builder.Append("#ifdef").Space().Append(condition).AppendLine();
                            }
                            declareMethod(builder, method.GetName(), method.GetNAPIMethodName());
                            if (condition != null)
                                builder.Append("#endif //").Space().Append(condition).AppendLine();
                        }
                    }

                    declareMethod(builder, "CreateNativeHandleRef", "NAPI_CreateNativeHandleRef");
                    declareMethod(builder, "CreateWeakNativeHandleRef", "NAPI_CreateWeakNativeHandleRef");
                    declareMethod(builder, "FreeNativeHandleRef", "NAPI_FreeNativeHandleRef");
                }
                builder.EndOfLine();
                builder.AppendLine();
                builder.Append("status = napi_define_properties(env, obj, std::size(addDescriptor), addDescriptor)").EndOfLine();
                builder.Append("assert(status == napi_ok)").EndOfLine();
                builder.Append("s_Env = env").EndOfLine();
                builder.Append("return obj").EndOfLine();
            }

            builder.AppendLine();
            builder.AppendLine(
$$"""
extern "C" napi_value Init(napi_env env, napi_value exports)
{
    napi_value new_exports;
    napi_status status = napi_create_function(
        env, "", NAPI_AUTO_LENGTH, CreateAddon, nullptr, &new_exports);
    assert(status == napi_ok);
    return new_exports;
}

extern "C" EXPORT_ATTRIB napi_value napi_register_module_v1(napi_env env, napi_value exports)
{
    return Init(env, exports);
}
""");
        }
    }

    void declareMethod(CodeBuilder builder, string methodName, string napiMethodName)
    {
        builder.Append("DECLARE_NAPI_METHOD").Parenthesized().
            Append(methodName).CommaSeparator().Append(napiMethodName).Close().AppendLine(",");
    }

    protected override string GetGeneratedPreamble() => ConversionCSharpToNAPI.SourcePreamble;

    protected override string GetFileName() => "MethodInit.cpp";
}