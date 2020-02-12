using CodeBinder.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    public class CLangSyntaxTreeContext : CLangCompilationContext.SyntaxTree<CLangCompilationContext, CLangNodeVisitor>
    {
        public new CLangCompilationContext Compilation { get; private set; }

        public CLangSyntaxTreeContext(CLangCompilationContext compilation)
        {
            Compilation = compilation;
        }

        protected override CLangCompilationContext getCompilationContext() => Compilation;

        protected override CLangNodeVisitor createVisitor()
        {
            return new CLangNodeVisitor(this);
        }
    }
}
