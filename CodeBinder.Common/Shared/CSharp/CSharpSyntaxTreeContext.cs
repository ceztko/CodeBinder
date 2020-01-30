// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpSyntaxTreeContext : CSharpCompilationContext.SyntaxTree<CSharpCompilationContext, CSharpNodeVisitor>
    {
        protected CSharpSyntaxTreeContext() { }
    }

    sealed class CSharpSyntaxTreeContextImpl : CSharpSyntaxTreeContext
    {
        public new CSharpCompilationContext Compilation { get; private set; }

        public CSharpSyntaxTreeContextImpl(CSharpCompilationContext compilation)
        {
            Compilation = compilation;
        }

        protected override CSharpNodeVisitor createVisitor()
        {
            return new CSharpNodeVisitorImpl(this);
        }

        protected override CSharpCompilationContext getCompilationContext() => Compilation;
    }
}
