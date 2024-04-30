// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.NativeAOT;

public class NAOTCollectionContext : CSharpCollectionContextBase<NAOTCompilationContext>
{
    public NAOTCollectionContext(NAOTCompilationContext context)
        : base(context)
    {
        Init += NAOTCollectionContext_VisitorInit;
    }

    private void NAOTCollectionContext_VisitorInit(CSharpNodeVisitor visitor)
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
        Compilation.AddEnum(node);
    }

    private void visitType(TypeDeclarationSyntax type)
    {
        NAOTModuleContextChild? module = null;
        string? moduleName;
        if (type.TryGetModuleName(this, out moduleName))
        {
            NAOTModuleContextParent? parent;
            if (!Compilation.TryGetModule(moduleName, out parent))
            {
                parent = new NAOTModuleContextParent(moduleName, Compilation);
                Compilation.AddModule(parent);
            }

            module = new NAOTModuleContextChild(Compilation);
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
