// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;
using CodeBinder.JavaScript.TypeScript;

namespace CodeBinder.JavaScript;

[ConversionLanguageName("TypeScript")]
[ConfigurationSwitch("commonjs", "Output is CommonJS compatible (TypeScript)")]
public class ConversionCSharpToTypeScript : CSharpLanguageConversion<TypeScriptCompilationContext>
{
    internal const string CodeBinderNamespace = "CodeBinder";

    public bool SkipBody { get; set; }

    internal const string SourcePreamble = "/* This file was generated. DO NOT EDIT! */";

    public ConversionCSharpToTypeScript()
    {
    }

    public override string GetMethodBaseName(IMethodSymbol symbol)
    {
        if (symbol.MethodKind == MethodKind.Constructor)
            return "constructor";
        else
            return base.GetMethodBaseName(symbol);
    }

    protected override TypeScriptCompilationContext CreateCSharpCompilationContext()
    {
        return new TypeScriptCompilationContext(this);
    }

    protected override CSharpValidationContext? CreateValidationContext()
    {
        return new TypeScriptValidationContext(this);
    }

    public override IReadOnlyCollection<string> SupportedPolicies => new string[] { Policies.GarbageCollection, Policies.Iterators, Policies.Generators };

    public override IReadOnlyList<string> PreprocessorDefinitions
    {
        get { return new string[] { "JAVASCRIPT", "TYPESCRIPT", "NODEJS" }; }
    }

    public override bool UseUTF8Bom
    {
        get { return false; }
    }

    public override bool TryParseExtraArgs(List<string> args)
    {
        // Try parse --commonjs switch
        if (args.Count == 1 && args[0] == "commonjs")
        {
            GenerationFlags |= TypeScriptGenerationFlags.CommonJSCompat;
            return true;
        }

        return false;
    }

    internal string TypeScriptModuleLoadSuffix
    {
        get
        {
            if (GenerationFlags.HasFlag(TypeScriptGenerationFlags.CommonJSCompat))
                return string.Empty;
            else
                return ".mjs";
        }
    }

    internal string TypeScriptSourceExtension
    {
        get
        {
            if (GenerationFlags.HasFlag(TypeScriptGenerationFlags.CommonJSCompat))
                return "ts";
            else
                return "mts";
        }
    }

    public TypeScriptGenerationFlags GenerationFlags { get; set; }

    public override MethodCasing MethodCasing => MethodCasing.LowerCamelCase;

    public override OverloadFeature? OverloadFeatures => OverloadFeature.None;
}

[Flags]
public enum TypeScriptGenerationFlags
{
    None = 0,
    /// <summary>
    /// Generate TypeScript code that can be safely transpiled to a CommonJS module
    /// </summary>
    /// <remarks>The code iteself will still use ES Module syntax, but it
    /// will avoid use of constructs like "import.meta". Also it will
    /// emit ".ts" files instead of ".mts", allowing to load modules
    /// without extension in import directives
    /// </remarks>
    CommonJSCompat = 1,
}
