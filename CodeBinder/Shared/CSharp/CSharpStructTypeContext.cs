using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public sealed class CSharpStructTypeContext : CSharpTypeContext<StructDeclarationSyntax, TypeConversion<CSharpStructTypeContext>>
    {
        public CSharpStructTypeContext(StructDeclarationSyntax node,
                CSharpCompilationContext compilation,
                TypeConversion<CSharpStructTypeContext> conversion)
            : base(node, compilation, conversion)
        {
            conversion.TypeContext = this;
        }
    }
}
