using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    public sealed class CSharpInterfaceTypeContext : CSharpTypeContext<InterfaceDeclarationSyntax, TypeConversion<CSharpInterfaceTypeContext>>
    {
        public CSharpInterfaceTypeContext(InterfaceDeclarationSyntax node,
                CSharpSyntaxTreeContext treeContext,
                TypeConversion<CSharpInterfaceTypeContext> conversion)
            : base(node, treeContext, conversion)
        {
            conversion.TypeContext = this;
        }
    }
}
