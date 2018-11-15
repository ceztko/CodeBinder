using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Util
{
    public abstract class ContextWriter
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

        public static ContextWriter NullWriter()
        {
            return new NullContextWriter();
        }

        class NullContextWriter : ContextWriter
        {
            protected override void Write()
            {
                Builder.Append("NULL");
            }
        }
    }

    public abstract class ContextWriter<TSyntax> : ContextWriter, ICompilationContextProvider
    {
        public CompilationContext Compilation { get; private set; }
        public TSyntax Context { get; private set; }

        protected ContextWriter(TSyntax context, ICompilationContextProvider provider)
        {
            Context = context;
            Compilation = provider.Compilation;
        }
    }
}
