// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.JavaScript.NAPI;

namespace CodeBinder.JavaScript;

[ConversionLanguageName("NAPI")]
public class ConversionCSharpToNAPI : CSharpLanguageConversionBase<NAPICompilationContext, NAPIModuleContext>
{
    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

    public ConversionCSharpToNAPI() { }

    protected override NAPICompilationContext CreateCompilationContext()
    {
        return new NAPICompilationContext(this);
    }

    public override MethodCasing MethodCasing => MethodCasing.LowerCamelCase;

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "NODEJS", "NAPI" }; }
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new StringConversionWriter("JSInterop.h", () => Resources.JSInterop_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("JSInterop.cpp", () => Resources.JSInterop_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("JSNAPI.h", () => Resources.JSNAPI_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("node_api.h", () => Resources.node_api_h) { BasePath = "Internal" };
            yield return new StringConversionWriter("node_api_types.h", () => Resources.node_api_types_h) { BasePath = "Internal" };
            yield return new StringConversionWriter("js_native_api.h", () => Resources.js_native_api_h) { BasePath = "Internal" };
            yield return new StringConversionWriter("js_native_api_types.h", () => Resources.js_native_api_types_h) { BasePath = "Internal" };
            yield return new StringConversionWriter("NAPIBinderUtils.h", () => Resources.NAPIBinderUtils_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("NAPIBinderUtils.cpp", () => Resources.NAPIBinderUtils_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("symbols.ld.exports", () => Exports_ld) { GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("symbols.ld64.exports", () => Exports_ld64) { GeneratedPreamble = SourcePreamble };
        }
    }

    const string Exports_ld = """
NAPI {
    global: _init; _fini;
        napi_register_module_v1;
    local: *;
};
""";

    const string Exports_ld64 = """
_napi_register_module_v1
""";
}
