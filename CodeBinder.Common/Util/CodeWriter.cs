using CodeBinder.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Util
{
    /// <summary>
    /// Util class to generate code of a generic item and append to a CodeBuilder, optionally with a context
    /// </summary>
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

        public static CodeWriter Create(Action<CodeBuilder> action)
        {
            return new ActionCodeWriter(action);
        }

        public static CodeWriter NullWriter()
        {
            return new NullCodeWriter();
        }

        protected abstract void Write();

        #region Support

        class NullCodeWriter : CodeWriter
        {
            protected override void Write()
            {
                Builder.Append("NULL");
            }
        }

        class ActionCodeWriter : CodeWriter
        {
            Action<CodeBuilder> _action;

            public ActionCodeWriter(Action<CodeBuilder> action)
            {
                _action = action;
            }

            protected override void Write()
            {
                _action(Builder);
            }
        }

        #endregion // Support
    }

    /// <typeparam name="TItem">Type of the item to write</typeparam>
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

    /// <typeparam name="TItem">Type of the item to write</typeparam>
    /// <typeparam name="TContext">Type of the context</typeparam>
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
