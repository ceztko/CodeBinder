// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

public class TypeScriptCompilationContext : CSharpCompilationContext<ConversionCSharpToTypeScript,
    TypeScriptClassContext, TypeScriptStructContext, TypeScriptInterfaceContext,
    TypeScriptEnumContext, TypeScriptDelegateContext>
{
    public TypeScriptCompilationContext(ConversionCSharpToTypeScript conversion)
        : base(conversion)
    {
    }

    public override IEnumerable<IConversionWriter> DefaultConversions
    {
        get
        {
            yield return new TypeScriptCodeBinderConversion(this);
            yield return new TypeScriptNAPIWrapperWriter(this);
            yield return new TypeScriptLibraryConversion(this);
        }
    }

    protected override TypeScriptClassContext CreateContext(ClassDeclarationSyntax cls)
    {
        return new TypeScriptClassContext(cls, this);
    }

    protected override TypeScriptStructContext CreateContext(StructDeclarationSyntax str)
    {
        return new TypeScriptStructContext(str, this);
    }

    protected override TypeScriptInterfaceContext CreateContext(InterfaceDeclarationSyntax iface)
    {
        return new TypeScriptInterfaceContext(iface, this);
    }

    protected override TypeScriptEnumContext CreateContext(EnumDeclarationSyntax enm)
    {
        return new TypeScriptEnumContext(enm, this);
    }

    protected override TypeScriptDelegateContext CreateContext(DelegateDeclarationSyntax dlg)
    {
        return new TypeScriptDelegateContext(dlg, this);
    }

    protected override INodeVisitor CreateVisitor()
    {
        return new TypeScriptNodeVisitor(this);
    }
}
