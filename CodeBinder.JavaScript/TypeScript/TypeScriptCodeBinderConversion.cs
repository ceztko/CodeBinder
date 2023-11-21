// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

/// <summary>
/// This class will write a small preamble with types to exported in a typescript namespace
/// </summary>
class TypeScriptCodeBinderConversion : TypeScriptConversionWriter
{
    public TypeScriptCodeBinderConversion(TypeScriptCompilationContext context)
        : base(context)
    {
    }

    protected override string GetFileName()
    {
        return $"{ConversionCSharpToTypeScript.CodeBinderNamespace}.{Context.Conversion.TypeScriptSourceExtension}";
    }

    protected override void write(CodeBuilder builder)
    {
        builder.AppendLine($"""
import napi_ from './{Context.NAPIWrapperName}{Context.Conversion.TypeScriptModuleLoadSuffix}';
let napi: any = napi_;
""");
        builder.AppendLine();

        foreach (var source in TypeScriptCodeBinderClasses.ClassesSource)
        {
            builder.AppendLine(source);
            builder.AppendLine();
        }
    }
}
