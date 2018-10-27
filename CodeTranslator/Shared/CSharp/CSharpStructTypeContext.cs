using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    public sealed class CSharpStructTypeContext : CSharpTypeContext<StructDeclarationSyntax, TypeConversion<CSharpStructTypeContext>>
    {
        public CSharpStructTypeContext(StructDeclarationSyntax node,
                CSharpSyntaxTreeContext treeContext,
                TypeConversion<CSharpStructTypeContext> conversion)
            : base(node, treeContext, conversion)
        {
            conversion.TypeContext = this;
        }
    }
}
