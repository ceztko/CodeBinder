// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

public class NativeAOTModuleConversion : TypeConversion<NativeAOTModuleContext, NativeAOTCompilationContext, ConversionCSharpToNativeAOT>
{
    public NativeAOTModuleConversion(NativeAOTModuleContext context, ConversionCSharpToNativeAOT conversion)
        : base(context, conversion)
    {
    }

    protected override string GetFileName() => $"{ModuleName}.cs";

    public string ModuleName
    {
        get { return Context.Name; }
    }

    protected sealed override void write(CodeBuilder builder)
    {
        builder.Append("using CodeBinder").EndOfStatement();
        builder.Append("using System.Runtime.InteropServices").EndOfStatement();
        builder.AppendLine();
        builder.Append("partial class ").Append(ModuleName).AppendLine();
        using (builder.Block())
        {
            builder.AppendLine("// Partial method declarations");
            builder.AppendLine();
            writePartialMethodDeclarations(builder);
        }
    }

    private void writePartialMethodDeclarations(CodeBuilder builder)
    {
        foreach (var method in Context.Methods)
        {
            builder.Append(new CLangMethodDeclarationWriter(method, this));
            builder.AppendLine();
        }
    }

    protected override string GetGeneratedPreamble() => ConversionCSharpToNativeAOT.SourcePreamble;

    public override NativeAOTCompilationContext Compilation
    {
        get { return Context.Compilation; }
    }
}
