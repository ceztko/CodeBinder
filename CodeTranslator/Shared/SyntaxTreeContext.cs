// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
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

        internal static void AddRootType<TTypeContext>(List<TTypeContext> types, TTypeContext type, TTypeContext parent)
            where TTypeContext : TypeContext<TTypeContext>
        {
            if (type.Parent != null)
                throw new Exception("Can't re-add root type");

            if (type == parent)
                throw new Exception("The parent can't be same reference as the given type");

            if (parent == null)
            {
                types.Add(type);
            }
            else
            {
                type.Parent = parent;
                parent.AddChild(type);
            }
        }

        protected abstract IEnumerable<TypeContext> GetRootTypes();
    }

    public abstract class SyntaxTreeContext<TTypeContext> : SyntaxTreeContext
        where TTypeContext : TypeContext<TTypeContext>
    {
        public new List<TTypeContext> RootTypes { get; private set; }

        internal SyntaxTreeContext()
        {
            RootTypes = new List<TTypeContext>();
        }

        protected void AddType(TTypeContext type, TTypeContext parent)
        {
            type.Compilation = Compilation;
            AddRootType(RootTypes, type, parent);
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
