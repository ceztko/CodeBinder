// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodeBinder.JavaScript.TypeScript;

/// <summary>
/// This class will write a small preamble with types to exported in a typescript namespace
/// </summary>
class TypeScriptCodeBinderConversion : TypeScriptConversionWriterBase
{
    public TypeScriptCodeBinderConversion()
    {
    }

    protected override string GetFileName()
    {
        return $"{ConversionCSharpToTypeScript.CodeBinderNamespace}.mts";
    }

    protected override void write(CodeBuilder builder)
    {
        builder.AppendLine($$"""
import napi_ from './NAPIENLibPdf.mjs';
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
