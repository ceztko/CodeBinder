using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public sealed class CSharpClassTypeContext : CSharpTypeContext<ClassDeclarationSyntax, CSharpCompilationContext, CSharpClassTypeContext>
    {
        public CSharpClassTypeContext(ClassDeclarationSyntax node, CSharpCompilationContext compilation)
            : base(node, compilation)
        {
        }

        protected override TypeConversion<CSharpClassTypeContext> createConversion()
        {
            return Compilation.Conversion.CreateConversion(this);
        }
    }
}
