using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public sealed class CSharpClassTypeContext : CSharpTypeContext<ClassDeclarationSyntax, TypeConversion<CSharpClassTypeContext>>
    {
        public CSharpClassTypeContext(ClassDeclarationSyntax node,
                CSharpCompilationContext compilation)
            : base(node, compilation, compilation.Conversion.GetClassTypeConversion())
        {
            Conversion.Context = this;
        }
    }
}
