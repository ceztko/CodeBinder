using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// Class syntax context
    /// </summary>
    /// <remarks>Inherit this class if needed to extend a class context</remarks>
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
