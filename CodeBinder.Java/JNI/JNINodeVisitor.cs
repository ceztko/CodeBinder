// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared;
using CodeBinder.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace CodeBinder.JNI;

public class JNINodeVisitor : CSharpNodeVisitorBase<JNICompilationContext, JNIModuleContext, ConversionCSharpToJNI>
{
    public JNINodeVisitor(JNICompilationContext compilation)
        : base(compilation)
    {
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        visitType(node);
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        visitType(node);
    }

    private void visitType(TypeDeclarationSyntax type)
    {
        JNIModuleContextChild? module = null;
        string? moduleName;
        if (TryGetModuleName(type, out moduleName))
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
