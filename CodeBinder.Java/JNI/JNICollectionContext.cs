// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;

namespace CodeBinder.JNI;

public class JNICollectionContext : CSharpCollectionContextBase<JNICompilationContext>
{
    public JNICollectionContext(JNICompilationContext compilation)
        : base(compilation)
    {
        Init += JNICollectionContext_VisitorInit;
    }

    private void JNICollectionContext_VisitorInit(CSharpNodeVisitor visitor)
    {
        visitor.ClassDeclarationVisit += Visitor_ClassDeclarationVisit;
        visitor.StructDeclarationVisit += Visitor_StructDeclarationVisit;
    }

    private void Visitor_ClassDeclarationVisit(CSharpNodeVisitor visitor, ClassDeclarationSyntax node)
    {
        visitType(node);
    }

    private void Visitor_StructDeclarationVisit(CSharpNodeVisitor visitor, StructDeclarationSyntax node)
    {
        visitType(node);
    }

    private void visitType(TypeDeclarationSyntax type)
    {
        JNIModuleContextChild? module = null;
        string? moduleName;
        if (type.TryGetModuleName(this, out moduleName))
        {
            JNIModuleContextParent? parent;
            if (!Compilation.TryGetModule(moduleName, out parent))
            {
                parent = new JNIModuleContextParent(moduleName, Compilation);
                Compilation.AddModule(Compilation, parent);

                foreach (var attribute in type.GetAttributes<ImportAttribute>(this))
                {
                    var include = new ImportAttribute(attribute.GetConstructorArgument<string>(0)) {
                        Condition = attribute.GetNamedArgument<string?>("Condition") };
                    parent.AddInclude(include);
                }
            }

            module = new JNIModuleContextChild(Compilation);
            Compilation.AddModuleChild(Compilation, module, parent);
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
                case SyntaxKind.StructKeyword:
                    visitType((StructDeclarationSyntax)member);
                    break;
            }
        }
    }
}
