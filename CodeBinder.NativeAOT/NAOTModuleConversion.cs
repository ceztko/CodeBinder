// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

public class NAOTModuleConversion : TypeConversion<NAOTModuleContext, NAOTCompilationContext, ConversionCSharpToNativeAOT>
{
    // True if the conversion is a template conversion,
    // eg. the method are not partial declarations but full definitions
    public bool IsTemplateConversion { get; private set; }

    public NAOTModuleConversion(NAOTModuleContext context, bool isTemplate,
            ConversionCSharpToNativeAOT conversion)
        : base(context, conversion)
    {
        IsTemplateConversion = isTemplate;
    }

    protected override string GetFileName() => $"{ModuleName}.cs";

    public string ModuleName
    {
        get { return Context.Name; }
    }

    protected sealed override void write(CodeBuilder builder)
    {
        builder.AppendLine();
        builder.Append("partial class ").Append(ModuleName).AppendLine();
        using (builder.Block())
        {
            if (!IsTemplateConversion)
                builder.AppendLine("// Partial method declarations").AppendLine();

            writePartialMethodDeclarations(builder);
        }
    }

    private void writePartialMethodDeclarations(CodeBuilder builder)
    {
        foreach (var method in Context.Methods)
        {
            builder.Append(new NAOTMethodWriter(method, IsTemplateConversion, this));
            builder.AppendLine();
        }
    }

    protected override string GetGeneratedPreamble() => ConversionCSharpToNativeAOT.SourcePreamble;

    public override NAOTCompilationContext Compilation
    {
        get { return Context.Compilation; }
    }
}
