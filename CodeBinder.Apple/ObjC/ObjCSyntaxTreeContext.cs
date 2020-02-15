using CodeBinder.Shared.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    public class ObjCSyntaxTreeContext : CSharpSyntaxTreeContext<CSharpCompilationContext>
    {
        public new ObjCCompilationContext Compilation { get; private set; }

        public ObjCSyntaxTreeContext(ObjCCompilationContext compilation)
        {
            Compilation = compilation;
        }

        protected override CSharpNodeVisitor createVisitor()
        {
            return new ObjCNodeVisitor(this);
        }

        protected override CSharpCompilationContext GetCSharpCompilationContext() => Compilation;
    }
}
