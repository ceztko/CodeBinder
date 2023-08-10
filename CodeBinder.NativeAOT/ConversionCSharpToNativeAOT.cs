// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.NativeAOT;

[ConversionLanguageName(LanguageName)]
[ConfigurationSwitch("create-template", "Create template project and definitions (NativeAOT)")]
public class ConversionCSharpToNativeAOT : CSharpLanguageConversionBase<NAOTCompilationContext, NAOTModuleContext>
{
    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";
    public const string LanguageName = "NAOT";

    public ConversionCSharpToNativeAOT() { }

    public override bool IsNative => true;

    public override bool NeedNamespaceMapping => false;

    // True if the conversion is a template conversion,
    // eg. the method are not partial declarations but full definitions
    public bool CreateTemplate { get; set; }

    public override IReadOnlyCollection<string> SupportedPolicies => new[] { Features.Delegates };

    protected override NAOTCompilationContext CreateCompilationContext()
    {
        return new NAOTCompilationContext(this);
    }

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "NativeAOT" }; }
    }

    public override bool TryParseExtraArgs(List<string> args)
    {
        // Try parse --interface-only switch
        if (args.Count == 1 && args[0] == "create-template")
        {
            CreateTemplate = true;
            return true;
        }

        return false;
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            if (!CreateTemplate)
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
