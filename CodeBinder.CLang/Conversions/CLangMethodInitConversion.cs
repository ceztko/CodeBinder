// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.CLang;

class CLangMethodInitConversion : CLangConversionWriter
{
    public CLangMethodInitConversion(CLangCompilationContext compilation)
        : base(compilation) { }

    protected override void write(CodeBuilder builder)
    {
        foreach (var module in Compilation.Modules)
            builder.Append("#include \"").Append(module.Name).AppendLine(".h\"");

        builder.AppendLine();
        builder.AppendLine("// Reference this symbol to ensure all functions are defined");
        builder.AppendLine("// See https://github.com/dotnet/samples/tree/3870722f5c5e80fd6a70946e6e96a5c990620e42/core/nativeaot/NativeLibrary#user-content-building-static-libraries");
        builder.AppendLine("void* CB_CLangExports[] = {");
        using (builder.Indent())
        {
            foreach (var module in Compilation.Modules)
            {
                foreach (var method in module.Methods)
                    builder.Append("(void *)").Append(method.GetCLangMethodName()).AppendLine(",");
            }
        }

        builder.Append("}").EndOfStatement();
    }

    protected override string GetGeneratedPreamble() => ConversionCSharpToCLang.SourcePreamble;

    protected override string GetFileName() => "MethodInit.cpp";
}
