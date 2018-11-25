using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public sealed class CSharpInterfaceTypeContext : CSharpTypeContext<InterfaceDeclarationSyntax, TypeConversion<CSharpInterfaceTypeContext>>
    {
        public CSharpInterfaceTypeContext(InterfaceDeclarationSyntax node,
                CSharpCompilationContext compilation,
                TypeConversion<CSharpInterfaceTypeContext> conversion)
            : base(node, compilation, conversion)
        {
            conversion.TypeContext = this;
        }
    }
}
