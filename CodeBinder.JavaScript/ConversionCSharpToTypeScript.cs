// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;
using CodeBinder.JavaScript.NAPI;
using CodeBinder.JavaScript.TypeScript;

namespace CodeBinder.JavaScript;

[ConversionLanguageName("TypeScript")]
public class ConversionCSharpToTypeScript : CSharpLanguageConversion<TypeScriptCompilationContext>
{
    internal const string CodeBinderNamespace = "CodeBinder";

    public bool SkipBody { get; set; }

    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

    public ConversionCSharpToTypeScript()
    {
    }

    protected override TypeScriptCompilationContext createCSharpCompilationContext()
    {
        return new TypeScriptCompilationContext(this);
    }

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "JavaScript", "TypeScript" }; }
    }

    public override bool UseUTF8Bom
    {
        get { return false; }
    }

    public override MethodCasing MethodCasing => MethodCasing.LowerCamelCase;

    public override OverloadFeature? OverloadFeatures => OverloadFeature.None;

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new TypeScriptCodeBinderConversion();
        }
    }
}
