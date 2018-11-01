using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public abstract class BaseWriter : ICompilationContextProvider
    {
        public CompilationContext Compilation { get; private set; }
        public CodeBuilder Builder { get; private set; }

        protected BaseWriter(ICompilationContextProvider provider)
        {
            // Slightly optimize getting semantic model by
            // storing CompilationContext
            Compilation = provider.Compilation;
        }

        public void Write(CodeBuilder builder)
        {
            Builder = builder;
            Write();
            Builder = null;
        }

        protected abstract void Write();

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return Compilation.GetSemanticModel(tree);
        }
    }
}
