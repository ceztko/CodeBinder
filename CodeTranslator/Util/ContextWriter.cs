using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Util
{
    public abstract class ContextWriter<TSyntax> : IContextWriter, ICompilationContextProvider
    {
        protected CodeBuilder Builder { get; private set; }
        public CompilationContext Compilation { get; private set; }
        public TSyntax Context { get; private set; }

        protected ContextWriter(TSyntax context, ICompilationContextProvider provider)
        {
            // Slightly optimize getting semantic model by
            // storing CompilationContext
            Context = context;
            Compilation = provider.Compilation;
        }

        // Append an ISyntaxWriter with CodeBuilder
        void IContextWriter.Write(CodeBuilder builder)
        {
            Builder = builder;
            Write();
            Builder = null;
        }

        protected abstract void Write();
    }

    public class NullContextWriter : IContextWriter
    {
        public void Write(CodeBuilder builder)
        {
            builder.Append("NULL");
        }
    }

    public interface IContextWriter
    {
        void Write(CodeBuilder builder);
    }
}
