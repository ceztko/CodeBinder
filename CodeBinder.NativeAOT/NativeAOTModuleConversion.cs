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
        WriteCppTrampoline(builder);
    }
    void WriteCppTrampoline(CodeBuilder builder)
    {
        builder.Append("partial class ").Append(ModuleName).AppendLine();
        using (builder.Block())
        {
            builder.AppendLine("// Partial method declarations");
            writePartialMethodDeclarations(builder);

            builder.AppendLine("// Native AOT  Trampolines");
            writeNativeAOTTrampoline(builder, true);
        }
    }

    private void writeNativeAOTTrampoline(CodeBuilder builder, bool writeBody)
    {
        foreach (var method in Context.Methods)
        {
            if (writeBody)
                builder.Append(new CLangMethodTrampolineWriter(method, this));
            else
                builder.Append(new CLangMethodDeclarationWriter(method, false, this));

            builder.AppendLine();
        }
    }

    private void writePartialMethodDeclarations(CodeBuilder builder)
    {
        foreach (var method in Context.Methods)
        {
            builder.Append(new CLangMethodDeclarationWriter(method, true, this));
            builder.AppendLine();
        }
    }

    protected override string GetGeneratedPreamble() => ConversionCSharpToNativeAOT.SourcePreamble;

    public override NativeAOTCompilationContext Compilation
    {
        get { return Context.Compilation; }
    }
}
