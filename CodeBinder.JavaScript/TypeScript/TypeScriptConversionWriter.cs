// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

abstract class TypeScriptConversionWriter : ConversionWriter
{
    public TypeScriptCompilationContext Context { get; private set; }

    public TypeScriptConversionWriter(TypeScriptCompilationContext context)
    {
        Context = context;
    }
}

abstract class TypeScriptConversionWriterBase : ConversionWriter
{
    protected override string? GetGeneratedPreamble() => ConversionCSharpToTypeScript.SourcePreamble;
}
