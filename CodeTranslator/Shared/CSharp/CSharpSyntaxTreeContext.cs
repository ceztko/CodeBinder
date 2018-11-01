// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeTranslator.Shared.CSharp
{
    public sealed class CSharpSyntaxTreeContext : SyntaxTreeContext<CSharpTypeContext>
    {
        private CSharpLanguageConversion _conversion;

        public CSharpSyntaxTreeContext(CompilationContext compilation, CSharpLanguageConversion conversion)
            : base(compilation)
        {
            _conversion = conversion;
        }

        public override void Visit(SyntaxTree tree)
        {
            var walker = new CSharpNodeVisitor(this, _conversion);
            walker.Visit(tree.GetRoot());
        }
    }
}
