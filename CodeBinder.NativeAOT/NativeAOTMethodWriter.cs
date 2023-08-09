// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.NativeAOT;

abstract class NativeAOTMethodWriter : CodeWriter<MethodDeclarationSyntax, NativeAOTModuleConversion>
{
    public bool CppMethod { get; private set; }

    protected NativeAOTMethodWriter(MethodDeclarationSyntax method, bool cppMethod, NativeAOTModuleConversion context)
        : base(method, context)
    {
        CppMethod = cppMethod;
    }

    protected override void Write()
    {
        if (CppMethod)
            Builder.Append("inline");
        else
            Builder.Append(Context.Compilation.LibraryName.ToUpper()).Append("_SHARED_API");

        Builder.Space().Append(ReturnType).Space();
        Builder.Append(MethodName).AppendLine("(");
        using (Builder.Indent())
        {
            WriteParameters();
            Builder.Append(")");
        }

        if (HasBody)
        {
            Builder.AppendLine();
            using (Builder.Block())
                WriteBody();
        }
        else
        {
            Builder.EndOfStatement();
        }    

    }

    public abstract string MethodName
    {
        get;
    }

    public abstract string ReturnType
    {
        get;
    }
    public abstract bool HasBody
    {
        get;
    }

    protected virtual void WriteBody()
    {
        /* Do Nothing */
    }

    protected abstract void WriteParameters();
}

class CLangMethodDeclarationWriter : NativeAOTMethodWriter
{
    public CLangMethodDeclarationWriter(MethodDeclarationSyntax method, bool cppMethod, NativeAOTModuleConversion context)
        : base(method, cppMethod, context) { }

    protected override void WriteParameters()
    {
        Builder.Append(new CLangParameterListWriter(Item.ParameterList, CppMethod, Context));
    }

    public override string ReturnType
    {
        get { return Item.GetCLangReturnType(CppMethod, Context); }
    }

    public override string MethodName
    {
        get { return Item.GetCLangMethodName(); }
    }

    public override bool HasBody => false;
}

class CLangMethodTrampolineWriter : NativeAOTMethodWriter
{
    public CLangMethodTrampolineWriter(MethodDeclarationSyntax method, NativeAOTModuleConversion context)
        : base(method, false, context) { }

    protected override void WriteParameters()
    {
        Builder.Append(new CLangParameterListWriter(Item.ParameterList, false, Context));
    }

    public override string ReturnType
    {
        get { return Item.GetCLangReturnType(false, Context); }
    }

    public override string MethodName
    {
        get { return Item.GetCLangMethodName(); }
    }

    protected override void WriteBody()
    {
        // Write string reference local wrapper first, to avoid "C++ initial
        // value of reference to non-const must be an lvalue" error
        foreach (var param in Item.ParameterList.Parameters)
        {
            var symbol = param.GetDeclaredSymbol<IParameterSymbol>(Context);
            if (symbol.Type.GetFullName() == "CodeBinder.cbstring" && symbol.RefKind != RefKind.None)
            {
                Builder.Append("cbstringpr").Space().Append(param.Identifier.Text)
                    .Append("_(").Append(param.Identifier.Text).Append(")").EndOfStatement();
            }
        }

        if (Item.ReturnType.GetTypeSymbolThrow(Context).SpecialType != SpecialType.System_Void)
            Builder.Append("return").Space();

        Builder.Append(Context.Compilation.LibraryName.ToLower()).Append("::")
            .Append(Item.GetCLangMethodName());

        using (Builder.ParameterList())
        {
            bool first = true;
            foreach (var param in Item.ParameterList.Parameters)
            {
                Builder.CommaSeparator(ref first);
                var symbol = param.GetDeclaredSymbol<IParameterSymbol>(Context);
                if (symbol.Type.GetFullName() == "CodeBinder.cbstring")
                {
                    if (symbol.RefKind == RefKind.None)
                    {
                        Builder.Append("std::move(").Append(param.Identifier.Text).Append(")");
                    }
                    else
                    {
                        // Write local variable wrapper instead
                        Builder.Append(param.Identifier.Text).Append("_");
                    }
                }
                else
                {
                    Builder.Append(param.Identifier.Text);
                }
            }
        }
        Builder.EndOfStatement();
    }

    public override bool HasBody => true;
}

class CLangParameterListWriter : CodeWriter<ParameterListSyntax, ICompilationProvider>
{
    public bool CppMethod { get; private set; }

    public CLangParameterListWriter(ParameterListSyntax list, bool cppMethod,
        ICompilationProvider module)
        : base(list, module)
    {
        CppMethod = cppMethod;
    }

    protected override void Write()
    {
        bool first = true;
        foreach (var parameter in Item.Parameters)
            Builder.CommaSeparator(ref first).Append(parameter, CppMethod, Context);
    }
}
