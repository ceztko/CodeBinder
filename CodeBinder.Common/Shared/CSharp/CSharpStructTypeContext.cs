using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpStructTypeContext : CSharpTypeContext<StructDeclarationSyntax, CSharpStructTypeContext>
    {
        protected CSharpStructTypeContext(StructDeclarationSyntax node)
            : base(node) { }

        protected override TypeConversion<CSharpStructTypeContext> createConversion()
        {
            return Compilation.Conversion.CreateConversion(this);
        }
    }

    public sealed class CSharpStructTypeContextImpl : CSharpStructTypeContext
    {
        public new CSharpCompilationContext Compilation { get; private set; }

        public CSharpStructTypeContextImpl(StructDeclarationSyntax node, CSharpCompilationContext compilation)
            : base(node)
        {
            Compilation = compilation;
        }

        protected override CSharpCompilationContext getCompilationContext() => Compilation;
    }
}
