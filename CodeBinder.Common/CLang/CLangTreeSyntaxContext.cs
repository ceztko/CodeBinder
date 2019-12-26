using CodeBinder.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    public class CLangSyntaxTreeContext : CLangCompilationContext.SyntaxTree<CLangCompilationContext>
    {
        public CLangSyntaxTreeContext(CLangCompilationContext compilation)
            : base(compilation)  { }

        public override void Visit(SyntaxTree tree)
        {
            var walker = new CLangNodeVisitor(this);
            walker.Visit(tree.GetRoot());
        }
    }
}
