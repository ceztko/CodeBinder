// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptVerbatimConversionWriter : TypeScriptConversionWriterBase
{
    public new string FileName { get; private set; }

    public new string? BasePath { get; private set; }

    public string Code { get; private set; }

    public TypeScriptVerbatimConversionWriter(string filename, string? basePath, string code)
    {
        FileName = filename;
        BasePath = basePath;
        Code = code;
    }

    protected override void write(CodeBuilder builder)
    {
        builder.AppendLine(Code);
    }

    protected override string? GetBasePath()
    {
        return BasePath;
    }

    protected override string GetFileName() => FileName;
}
