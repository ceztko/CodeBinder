using CodeBinder.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    public class JNISyntaxTreeContext : JNICompilationContext.SyntaxTree<JNICompilationContext, JNINodeVisitor>
    {
        public new JNICompilationContext Compilation { get; private set; }

        public JNISyntaxTreeContext(JNICompilationContext compilation)
        {
            Compilation = compilation;
        }

        protected override JNINodeVisitor createVisitor()
        {
            return new JNINodeVisitor(this);
        }

        protected override JNICompilationContext getCompilationContext() => Compilation;
    }
}
