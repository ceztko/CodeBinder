// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Shared;
using System;

namespace CodeBinder.Utils
{
    /// <summary>
    /// Util class to generate code of a generic item and append to a CodeBuilder with a context
    /// </summary>
    /// <typeparam name="TItem">Type of the item to write</typeparam>
    /// <typeparam name="TContext">Type of the contenxt</typeparam>
    public abstract class CodeWriter<TItem, TContext> : CodeWriter<TItem>
    {
        public TContext Context { get; private set; }

        protected CodeWriter(TItem item, TContext context)
            : base(item)
        {
            Context = context;
        }
    }

    /// <summary>
    /// Util class to generate code of a generic item and append to a CodeBuilder
    /// </summary>
    /// <typeparam name="TItem">Type of the item to write</typeparam>
    public abstract class CodeWriter<TItem> : CodeWriter
    {
        public TItem Item { get; private set; }

        protected CodeWriter(TItem item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// Util class to generate code of a generic item and append to a CodeBuilder
    /// </summary>
    public abstract class CodeWriter : ICodeWriter
    {
        CodeBuilder? _builder;

        protected CodeBuilder Builder => _builder ?? throw new Exception($"Can't use {nameof(Builder)} right now");

        // Append an ISyntaxWriter with CodeBuilder
        void ICodeWriter.Write(CodeBuilder builder)
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
                if (nullstr.IsNullOrEmpty())
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

    /// <summary>
    ///  Util interface to generate code of a generic item and append to a CodeBuilder
    /// </summary>
    /// <remarks>Use the inplementation <see cref="CodeWriter"/></remarks>
    public interface ICodeWriter
    {
        void Write(CodeBuilder builder);
    }
}
