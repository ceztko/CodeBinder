// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

class NAOTMethodWriter : CodeWriter<MethodDeclarationSyntax, NAOTModuleConversion>
{
    public bool IsTemplateCreation { get; private set; }

    public NAOTMethodWriter(MethodDeclarationSyntax method, bool isTemplateCreation, NAOTModuleConversion context)
        : base(method, context)
    {
        IsTemplateCreation = isTemplateCreation;
    }

    protected override void Write()
    {
        if (!IsTemplateCreation)
            Builder.AppendLine("[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]");

        Builder.Append("public static unsafe partial").Space().Append(Item.GetNAOTReturnType(Context)).Space();
        Builder.Append(Item.GetNAOTMethodName()).AppendLine("(");
        using (Builder.Indent())
        {
            Builder.Append(new NAOTParameterListWriter(Item.ParameterList, Context));
            Builder.Append(")");
        }

        if (IsTemplateCreation)
        {
            Builder.AppendLine();
            using (Builder.Block())
            {
                var symbol = Item.ReturnType.GetTypeSymbol(Context)!;
                if (symbol.SpecialType != SpecialType.System_Void)
                    Builder.Append("return").Space().Append("default").EndOfStatement();
            }
        }
        else
        {
            Builder.EndOfStatement();
        }
    }
}

class NAOTParameterListWriter : CodeWriter<ParameterListSyntax, ICompilationProvider>
{
    public NAOTParameterListWriter(ParameterListSyntax list,
        ICompilationProvider module)
        : base(list, module)
    {
    }

    protected override void Write()
    {
        bool first = true;
        foreach (var parameter in Item.Parameters)
            Builder.CommaSeparator(ref first).Append(parameter, Context);
    }
}
