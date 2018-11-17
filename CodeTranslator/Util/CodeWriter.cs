using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Util
{
    public abstract class CodeWriter
    {
        protected CodeBuilder Builder { get; private set; }

        // Append an ISyntaxWriter with CodeBuilder
        internal void Write(CodeBuilder builder)
        {
            Builder = builder;
            Write();
            Builder = null;
        }

        protected abstract void Write();

        public static CodeWriter NullWriter()
        {
            return new NullCodeWriter();
        }

        class NullCodeWriter : CodeWriter
        {
            protected override void Write()
            {
                Builder.Append("NULL");
            }
        }
    }

    public abstract class CodeWriter<TContext> : CodeWriter, ICompilationContextProvider
    {
        public CompilationContext Compilation { get; private set; }
        public TContext Context { get; private set; }

        protected CodeWriter(TContext context, ICompilationContextProvider provider)
        {
            Context = context;
            Compilation = provider.Compilation;
        }
    }
}
