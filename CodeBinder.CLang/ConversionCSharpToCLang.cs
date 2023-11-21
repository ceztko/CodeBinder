// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.CLang;

[ConversionLanguageName(LanguageName)]
[ConfigurationSwitch("interface-only", "Only output public interface (CLang)")]
public class ConversionCSharpToCLang : CSharpLanguageConversionBase<CLangCompilationContext, CLangModuleContext>
{
    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";
    public const string LanguageName = "CLang";

    public ConversionCSharpToCLang() { }

    public override bool IsNative => true;

    public override bool NeedNamespaceMapping => false;

    public bool PublicInterfaceOnly { get; set; }

    public override IReadOnlyCollection<string> SupportedPolicies => new[] { Features.Delegates };

    protected override CLangCompilationContext CreateCompilationContext()
    {
        return new CLangCompilationContext(this);
    }

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "CLang" }; }
    }

    internal const string BaseTypesHeader = "CBBaseTypes.h";

    public override bool TryParseExtraArgs(List<string> args)
    {
        // Try parse --interface-only switch
        if (args.Count == 1 && args[0] == "interface-only")
        {
            PublicInterfaceOnly = true;
            return true;
        }

        return false;
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new StringConversionWriter(BaseTypesHeader, () => CLangResources.CBBaseTypes_h) { GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("CBInterop.h", () => CLangResources.CBInterop_h) { GeneratedPreamble = SourcePreamble };
            yield return new StringConversionWriter("CBInterop.hpp", () => CLangResources.CBInterop_hpp) { GeneratedPreamble = SourcePreamble };
        }
    }
}
