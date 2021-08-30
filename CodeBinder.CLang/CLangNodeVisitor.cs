// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared;
using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace CodeBinder.CLang
{
    public class CLangNodeVisitor : CSharpNodeVisitorBase<CLangCompilationContext, CLangModuleContext, ConversionCSharpToCLang>
    {
        public CLangNodeVisitor(CLangCompilationContext context)
            : base(context)
        {
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var symbol = node.GetDeclaredSymbol<ITypeSymbol>(this)!;
            if (symbol.Inherits<NativeTypeBinder>())
            {
                // These are the binders for types
                Compilation.AddType(node);
            }

            visitType(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            visitType(node);
        }

        private void visitType(TypeDeclarationSyntax type)
        {
            CLangModuleContextChild? module = null;
            string? moduleName;
            if (TryGetModuleName(type, out moduleName))
            {
                CLangModuleContextParent? parent;
                if (!Compilation.TryGetModule(moduleName, out parent))
                {
                    parent = new CLangModuleContextParent(moduleName, Compilation);
                    Compilation.AddModule(Compilation, parent);
                }

                module = new CLangModuleContextChild(Compilation);
                Compilation.AddModuleChild(Compilation, module, parent);
            }

            foreach (var member in type.Members)
            {
                var kind = member.Kind();
                switch (kind)
                {
                    case SyntaxKind.MethodDeclaration:
                        // TODO: Chehck for policies. Fix/extend ShouldDiscard
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

        bool TryGetModuleName(TypeDeclarationSyntax type, [NotNullWhen(true)]out string? moduleName)
        {
            var attributes = type.GetAttributes(this);
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<ModuleAttribute>())
                {
                    moduleName = attribute.GetConstructorArgument<string>(0);
                    return true;
                }
            }

            moduleName = null;
            return false;
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            if (!node.GetAttributes(this).HasAttribute<NativeBindingAttribute>())
                return;

            Compilation.AddEnum(node);
        }
    }
}
