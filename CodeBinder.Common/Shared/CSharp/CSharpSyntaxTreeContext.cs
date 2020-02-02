// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// SyntaxTree context
    /// </summary>
    /// <remarks>Inherit this class to extend the context, implementing both
    /// createVisitor() and getCompilationContext()</remarks> 
    public abstract class CSharpSyntaxTreeContext<TCompilationContext> : CSharpSyntaxTreeContext
        where TCompilationContext : CSharpCompilationContext
    {
        protected CSharpSyntaxTreeContext() { }

        protected abstract TCompilationContext GetCSharpCompilationContext();

        protected sealed override CSharpCompilationContext getCompilationContext() => GetCSharpCompilationContext();
    }

    public abstract class CSharpSyntaxTreeContext : CSharpCompilationContext.SyntaxTree<CSharpCompilationContext, CSharpNodeVisitor>
    {
        internal CSharpSyntaxTreeContext() { }
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
