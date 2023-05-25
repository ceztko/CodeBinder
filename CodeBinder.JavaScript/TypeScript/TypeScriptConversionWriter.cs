// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

abstract class TypeScriptConversionWriter : ConversionWriter
{
    public ConversionCSharpToTypeScript Conversion { get; private set; }

    public TypeScriptConversionWriter(ConversionCSharpToTypeScript conversion)
    {
        Conversion = conversion;
    }
}

abstract class TypeScriptConversionWriterBase : ConversionWriter
{
    protected override string? GetGeneratedPreamble() => ConversionCSharpToTypeScript.SourcePreamble;
}
