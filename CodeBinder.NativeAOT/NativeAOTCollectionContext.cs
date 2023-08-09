// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.NativeAOT;

public class NativeAOTCollectionContext : CSharpCollectionContextBase<NativeAOTCompilationContext>
{
    public NativeAOTCollectionContext(NativeAOTCompilationContext context)
        : base(context)
    {
        Init += CLangCollectionContext_VisitorInit;
    }

    private void CLangCollectionContext_VisitorInit(CSharpNodeVisitor visitor)
    {
        visitor.ClassDeclarationVisit += Visitor_ClassDeclarationVisit;
        visitor.StructDeclarationVisit += Visitor_StructDeclarationVisit;
        visitor.EnumDeclarationVisit += Visitor_EnumDeclarationVisit;
    }

    private void Visitor_ClassDeclarationVisit(CSharpNodeVisitor visitor, ClassDeclarationSyntax node)
    {
        var symbol = node.GetDeclaredSymbol<ITypeSymbol>(this);
        if (symbol.Inherits<NativeTypeBinder>())
        {
            // These are the binders for types
            Compilation.AddOpaqueType(node);
        }

        visitType(node);
    }

    private void Visitor_StructDeclarationVisit(CSharpNodeVisitor visitor, StructDeclarationSyntax node)
    {
        if (node.HasAttribute<NativeBindingAttribute>(this))
            Compilation.AddType(node);

        visitType(node);
    }

    private void Visitor_EnumDeclarationVisit(CSharpNodeVisitor visitor, EnumDeclarationSyntax node)
    {
        if (!node.GetAttributes(this).HasAttribute<NativeBindingAttribute>())
            return;

        Compilation.AddEnum(node);
    }

    private void visitType(TypeDeclarationSyntax type)
    {
        CLangModuleContextChild? module = null;
        string? moduleName;
        if (type.TryGetModuleName(this, out moduleName))
        {
            CLangModuleContextParent? parent;
            if (!Compilation.TryGetModule(moduleName, out parent))
            {
                parent = new CLangModuleContextParent(moduleName, Compilation);
                Compilation.AddModule(parent);
            }

            module = new CLangModuleContextChild(Compilation);
            Compilation.AddModuleChild(module, parent);
        }

        foreach (var member in type.Members)
        {
            var kind = member.Kind();
            switch (kind)
            {
                case SyntaxKind.MethodDeclaration:
                    if (module != null && !member.ShouldDiscard(Compilation))
                    {
                        var method = (MethodDeclarationSyntax)member;
                        if (method.IsNative(this))
                            module.AddNativeMethod(method);
                    }
                    break;
                case SyntaxKind.ClassDeclaration:
                    visitType((ClassDeclarationSyntax)member);
                    break;
                case SyntaxKind.StructDeclaration:
                    visitType((StructDeclarationSyntax)member);
                    break;
                case SyntaxKind.DelegateDeclaration:
                    visitType((DelegateDeclarationSyntax)member);
                    break;
            }
        }
    }

    public void visitType(DelegateDeclarationSyntax node)
    {
        if (node.ShouldDiscard(Compilation))
            return;

        Compilation.AddCallback(node);
    }
}
