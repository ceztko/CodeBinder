using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpClassTypeContext : CSharpTypeContext<ClassDeclarationSyntax, CSharpClassTypeContext>
    {
        protected CSharpClassTypeContext(ClassDeclarationSyntax node)
            : base(node) { }

        protected override TypeConversion<CSharpClassTypeContext> createConversion()
        {
            return Compilation.Conversion.CreateConversion(this);
        }
    }

    sealed class CSharpClassTypeContextImpl : CSharpClassTypeContext
    {
        public new CSharpCompilationContext Compilation { get; private set; }

        public CSharpClassTypeContextImpl(ClassDeclarationSyntax node, CSharpCompilationContext context)
            : base(node)
        {
            Compilation = context;
        }

        protected override CSharpCompilationContext getCompilationContext() => Compilation;
    }
}
