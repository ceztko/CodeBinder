// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.JavaScript.NAPI;

namespace CodeBinder.JavaScript;

[ConversionLanguageName("NAPI")]
public class ConversionCSharpToNAPI : LanguageConversion<NAPICompilationContext, NAPIModuleContext>
{
    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

    public ConversionCSharpToNAPI() { }

    protected override NAPICompilationContext createCompilationContext()
    {
        return new NAPICompilationContext(this);
    }

    public override MethodCasing MethodCasing => MethodCasing.LowerCamelCase;

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "NODEJS" }; }
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new StringConversionWriter("JSInterop.h", () => Resources.JSInterop_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("JSInterop.cpp", () => Resources.JSInterop_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("NAPIBinderUtils.h", () => Resources.NAPIBinderUtils_h) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("NAPIBinderUtils.cpp", () => Resources.NAPIBinderUtils_cpp) { BasePath = "Internal", GeneratedPreamble = SourcePreamble };
        }
    }
}
