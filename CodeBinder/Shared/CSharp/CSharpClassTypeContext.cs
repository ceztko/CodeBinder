using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public sealed class CSharpClassTypeContext : CSharpTypeContext<ClassDeclarationSyntax, TypeConversion<CSharpClassTypeContext>>
    {
        public CSharpClassTypeContext(ClassDeclarationSyntax node,
                CSharpSyntaxTreeContext treeContext,
                TypeConversion<CSharpClassTypeContext> conversion)
            : base(node, treeContext, conversion)
        {
            conversion.TypeContext = this;
        }
    }
}
