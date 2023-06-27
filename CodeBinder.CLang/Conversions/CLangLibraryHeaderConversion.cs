// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.CLang;

class CLangLibraryHeaderConversion : CLangConversionWriter
{
    public CLangLibraryHeaderConversion(CLangCompilationContext compilation)
        : base(compilation) { }

    protected override void write(CodeBuilder builder)
    {
        builder.AppendLine("#pragma once");
        builder.AppendLine();
        builder.AppendLine("#include \"CBInterop.h\"");
        builder.AppendLine();
        builder.AppendLine("// Modules");
        foreach (var module in Compilation.Modules)
            builder.Append("#include \"").Append(module.Name).AppendLine(".h\"");
    }

    protected override string GetGeneratedPreamble() => ConversionCSharpToCLang.SourcePreamble;

    protected override string GetFileName() => $"{Compilation.LibraryName}.h";
}
