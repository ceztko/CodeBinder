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
    }
}
