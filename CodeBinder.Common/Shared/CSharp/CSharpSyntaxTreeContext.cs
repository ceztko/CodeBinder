// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public class CSharpSyntaxTreeContext : CSharpCompilationContext.SyntaxTree<CSharpCompilationContext>
    {
        public CSharpSyntaxTreeContext(CSharpCompilationContext compilation)
            : base(compilation) { }

        public new void AddType(CSharpBaseTypeContext type, CSharpBaseTypeContext? parent)
        {
            base.AddType(type, parent);
        }
    }
}
