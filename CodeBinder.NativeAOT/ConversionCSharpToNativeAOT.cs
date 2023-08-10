// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.NativeAOT;

[ConversionLanguageName(LanguageName)]
public class ConversionCSharpToNativeAOT : CSharpLanguageConversionBase<NAOTCompilationContext, NAOTModuleContext>
{
    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";
    public const string LanguageName = "NAOT";

    public ConversionCSharpToNativeAOT() { }

    public override bool IsNative => true;

    public override bool NeedNamespaceMapping => false;

    // True if the conversion is a template conversion,
    // eg. the method are not partial declarations but full definitions
    public bool CreateTemplateProject { get; set; }

    public override IReadOnlyCollection<string> SupportedPolicies => new[] { Features.Delegates };

    protected override NAOTCompilationContext CreateCompilationContext()
    {
        return new NAOTCompilationContext(this);
    }

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "NativeAOT" }; }
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            if (!CreateTemplateProject)
            {
                yield return new StringConversionWriter($"globals.cs",() => Globals)
                    { GeneratedPreamble = SourcePreamble };
            }
        }
    }

    const string Globals =
"""
global using System.Runtime.InteropServices;
global using System.Runtime.CompilerServices;
global using CodeBinder;
""";
}
