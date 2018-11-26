using CodeBinder.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Util
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

    public abstract class CodeWriter<TItem> : CodeWriter, ICompilationContextProvider
    {
        public CompilationContext Compilation { get; private set; }
        public TItem Item { get; private set; }

        protected CodeWriter(TItem item, ICompilationContextProvider context)
        {
            Item = item;
            Compilation = context.Compilation;
        }
    }

    public abstract class CodeWriter<TItem, TContext> : CodeWriter<TItem>
        where TContext : ICompilationContextProvider
    {
        public TContext Context { get; private set; }

        protected CodeWriter(TItem item, TContext context, ICompilationContextProvider provider)
            : base(item, provider)
        {
            Context = context;
        }
    }
}
