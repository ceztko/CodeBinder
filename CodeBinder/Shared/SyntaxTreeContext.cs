// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    public abstract class SyntaxTreeContext : ICompilationContextProvider
    {
        public CompilationContext Compilation { get; internal set; }

        public SyntaxTree SyntaxTree { get; private set; }

        internal SyntaxTreeContext() { }

        public abstract void Visit(SyntaxTree node);

        public IEnumerable<TypeContext> RootTypes
        {
            get { return GetRootTypes(); }
        }

        protected abstract IEnumerable<TypeContext> GetRootTypes();
    }

    public abstract class SyntaxTreeContext<TTypeContext> : SyntaxTreeContext
        where TTypeContext : TypeContext<TTypeContext>
    {
        List<TTypeContext> _rootTypes;

        internal SyntaxTreeContext()
        {
            _rootTypes = new List<TTypeContext>();
        }

        protected void AddType(TTypeContext type, TTypeContext parent)
        {
            if (type.Parent != null)
                throw new Exception("Can't re-add root type");

            if (type == parent)
                throw new Exception("The parent can't be same reference as the given type");

            type.Compilation = Compilation;
            if (parent == null)
            {
                _rootTypes.Add(type);
            }
            else
            {
                type.Parent = parent;
                parent.AddChild(type);
            }
        }

        public new IEnumerable<TypeContext> RootTypes
        {
            get { return _rootTypes; }
        }

        protected override IEnumerable<TypeContext> GetRootTypes()
        {
            return RootTypes;
        }
    }

    public abstract class SyntaxTreeContext<TTypeContext, TLanguageConversion> : SyntaxTreeContext<TTypeContext>
        where TTypeContext : TypeContext<TTypeContext>
        where TLanguageConversion : LanguageConversion
    {
        public TLanguageConversion Conversion { get; private set; }

        protected SyntaxTreeContext(TLanguageConversion conversion)
        {
            Conversion = conversion;
        }
    }
}
