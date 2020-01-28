using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public sealed class CSharpInterfaceTypeContext : CSharpTypeContext<InterfaceDeclarationSyntax, TypeConversion<CSharpInterfaceTypeContext>>
    {
        public CSharpInterfaceTypeContext(InterfaceDeclarationSyntax node,
                CSharpCompilationContext compilation)
            : base(node, compilation, compilation.Conversion.GetInterfaceTypeConversion())
        {
            Conversion.Context = this;
        }
    }
}
