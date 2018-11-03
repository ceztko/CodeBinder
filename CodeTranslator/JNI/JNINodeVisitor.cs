using CodeTranslator.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared;
using CodeTranslator.Attributes;

namespace CodeTranslator.JNI
{
    class JNINodeVisitor : CSharpNodeVisitor<JNISyntaxTreeContext, CSToJNIConversion>
    {
        Dictionary<string, JNIModuleContext> _modules;

        public JNINodeVisitor(JNISyntaxTreeContext treeContext, CSToJNIConversion conversion)
            : base(treeContext, conversion)
        {
            _modules = new Dictionary<string, JNIModuleContext>();
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
            JNIModuleContext module = null;
            string moduleName;
            if (TryGetModule(type, out moduleName))
            {
                if (!_modules.TryGetValue(moduleName, out module))
                {
                    module = new JNIModuleContext(moduleName, TreeContext);
                    TreeContext.AddType(module, null);
                    _modules.Add(moduleName, module);
                }
            }

            foreach (var member in type.Members)
            {
                var kind = member.Kind();
                switch (kind)
                {
                    case SyntaxKind.MethodDeclaration:
                        if (module != null)
                            module.AddNativeMethod(member as MethodDeclarationSyntax);
                        break;
                    case SyntaxKind.ClassDeclaration:
                        visitType(member as ClassDeclarationSyntax);
                        break;
                    case SyntaxKind.StructKeyword:
                        visitType(member as StructDeclarationSyntax);
                        break;
                }
            }
        }

        bool TryGetModule(TypeDeclarationSyntax type, out string moduleName)
        {
            var attributes = type.GetAttributes(this);
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<ModuleAttribute>())
                {
                    moduleName = attribute.ConstructorArguments[0].Value.ToString();
                    return true;
                }
            }

            moduleName = null;
            return false;
        }
    }
}
