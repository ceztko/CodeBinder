using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public sealed class CSharpInterfaceTypeContext : CSharpTypeContext<InterfaceDeclarationSyntax, CSharpCompilationContext, CSharpInterfaceTypeContext>
    {
        public CSharpInterfaceTypeContext(InterfaceDeclarationSyntax node,
                CSharpCompilationContext compilation)
            : base(node, compilation) { }

        protected override TypeConversion<CSharpInterfaceTypeContext> createConversion()
        {
            return Compilation.Conversion.CreateConversion(this);
        }
    }
}
