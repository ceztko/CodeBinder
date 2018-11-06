using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Util
{
    public abstract class SyntaxWriter<TSyntax> : ISyntaxWriter, ICompilationContextProvider
    {
        protected CodeBuilder Builder { get; private set; }
        public CompilationContext Compilation { get; private set; }
        public TSyntax Syntax { get; private set; }

        protected SyntaxWriter(TSyntax syntax, ICompilationContextProvider provider)
        {
            // Slightly optimize getting semantic model by
            // storing CompilationContext
            Syntax = syntax;
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

    public class NullSyntaxWriter : ISyntaxWriter
    {
        public void Write(CodeBuilder builder)
        {
            builder.AppendLine("NULL");
        }
    }

    public interface ISyntaxWriter
    {
        void Write(CodeBuilder builder);
    }
}
