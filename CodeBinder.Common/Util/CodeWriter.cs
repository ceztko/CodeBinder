using CodeBinder.Shared;
using System;

namespace CodeBinder.Util
{
    /// <typeparam name="TItem">Type of the item to write</typeparam>
    /// <typeparam name="TContext">Type of the contenxt</typeparam>
    public abstract class CodeWriter<TItem, TContext> : CodeWriter
    {
        public TItem Item { get; private set; }

        public TContext Context { get; private set; }

        protected CodeWriter(TItem item, TContext context)
        {
            Item = item;
            Context = context;
        }
    }

    /// <summary>
    /// Util class to generate code of a generic item and append to a CodeBuilder, optionally with a context
    /// </summary>
    public abstract class CodeWriter
    {
        CodeBuilder? _builder;

        protected CodeBuilder Builder => _builder ?? throw new Exception($"Can't use {nameof(Builder)} right now");

        // Append an ISyntaxWriter with CodeBuilder
        internal void Write(CodeBuilder builder)
        {
            _builder = builder;
            Write();
            _builder = null;
        }

        public static CodeWriter Create(Action<CodeBuilder> action)
        {
            return new ActionCodeWriter(action);
        }

        public static CodeWriter NullWriter(string? nullstr = null)
        {
            return new NullCodeWriter(nullstr);
        }

        protected abstract void Write();

        #region Support

        class NullCodeWriter : CodeWriter
        {
            public string NullStr { get; private set; }

            public NullCodeWriter(string? nullstr)
            {
                if (string.IsNullOrEmpty(nullstr))
                    nullstr = "NULL";

                NullStr = nullstr;
            }

            protected override void Write()
            {
                Builder.Append(NullStr);
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
}
