using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared;
using CodeBinder.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace CodeBinder.JNI
{
    public class JNINodeVisitor : CSharpNodeVisitor<JNICompilationContext, JNIModuleContext, JNISyntaxTreeContext, ConversionCSharpToJNI>
    {
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
                        if (module != null && !member.ShouldDiscard(this))
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
    }
}
