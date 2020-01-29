using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public sealed class CSharpStructTypeContext : CSharpTypeContext<StructDeclarationSyntax, CSharpStructTypeContext>
    {
        public CSharpStructTypeContext(StructDeclarationSyntax node,
                CSharpCompilationContext compilation)
            : base(node, compilation) { }

        protected override TypeConversion<CSharpStructTypeContext> createConversion()
        {
            return Compilation.Conversion.CreateConversion(this);
        }
    }
}
