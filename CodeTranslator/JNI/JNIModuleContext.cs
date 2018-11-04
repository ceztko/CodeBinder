using CodeTranslator.Shared;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.JNI
{
    public class JNIModuleContext : TypeContext<JNIModuleContext, JNISyntaxTreeContext>
    {
        public string ModuleName { get; private set; }
        private List<MethodDeclarationSyntax> _methods;

        public JNIModuleContext(string moduleName, JNISyntaxTreeContext context)
            : base(context)
        {
            ModuleName = moduleName;
            _methods = new List<MethodDeclarationSyntax>();
        }

        protected override TypeConversion GetConversion()
        {
            var ret = new JNIModuleConversion(TreeContext.Conversion);
            ret.TypeContext = this;
            return ret;
        }

        internal void AddNativeMethod(MethodDeclarationSyntax method)
        {
            _methods.Add(method);
        }

        public IReadOnlyList<MethodDeclarationSyntax> Methods { get => _methods; }
    }
}
