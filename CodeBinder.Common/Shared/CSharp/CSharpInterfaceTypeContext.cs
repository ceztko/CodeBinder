using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpInterfaceTypeContext : CSharpTypeContext<InterfaceDeclarationSyntax, CSharpInterfaceTypeContext>
    {
        public CSharpInterfaceTypeContext(InterfaceDeclarationSyntax node)
            : base(node) { }

        protected override TypeConversion<CSharpInterfaceTypeContext> createConversion()
        {
            return Compilation.Conversion.CreateConversion(this);
        }
    }

    public sealed class CSharpInterfaceTypeContextImpl : CSharpInterfaceTypeContext
    {
        public new CSharpCompilationContext Compilation { get; private set; }

        public CSharpInterfaceTypeContextImpl(InterfaceDeclarationSyntax node, CSharpCompilationContext compilation)
            : base(node)
        {
            Compilation = compilation;
        }

        protected override CSharpCompilationContext getCompilationContext() => Compilation;
    }
}
