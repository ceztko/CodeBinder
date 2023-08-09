// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.NativeAOT;

[ConversionLanguageName(LanguageName)]
public class ConversionCSharpToNativeAOT : CSharpLanguageConversionBase<NativeAOTCompilationContext, NativeAOTModuleContext>
{
    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";
    public const string LanguageName = "NativeAOT";

    public ConversionCSharpToNativeAOT() { }

    public override bool IsNative => true;

    public override bool NeedNamespaceMapping => false;

    public override IReadOnlyCollection<string> SupportedPolicies => new[] { Features.Delegates };

    protected override NativeAOTCompilationContext CreateCompilationContext()
    {
        return new NativeAOTCompilationContext(this);
    }

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "NativeAOT" }; }
    }
}
