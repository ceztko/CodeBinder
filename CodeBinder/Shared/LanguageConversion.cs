// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Shared
{
    public abstract class LanguageConversion
    {
        internal LanguageConversion() { }

        public abstract SyntaxTreeContext GetSyntaxTreeContext();

        public virtual IEnumerable<ConversionBuilder> DefaultConversions
        {
            get { yield break; }
        }

        public virtual bool UseUTF8Bom
        {
            get { return true; }
        }

        /// <summary>Preprocessor definitions</summary>
        public virtual IEnumerable<string> CondictionaCompilationSymbols
        {
            get { yield break; }
        }

        internal IEnumerable<ConversionDelegate> DefaultConversionDelegates
        {
            get
            {
                foreach (var conversion in DefaultConversions)
                    yield return new ConversionDelegate(conversion);
            }
        }
    }

    public abstract class LanguageConversion<TSyntaxTreeContext, TTypeContext> : LanguageConversion
        where TSyntaxTreeContext : SyntaxTreeContext<TTypeContext>
        where TTypeContext : TypeContext<TTypeContext, TSyntaxTreeContext>
    {
        protected LanguageConversion() { }

        public sealed override SyntaxTreeContext GetSyntaxTreeContext()
        {
            return getSyntaxTreeContext();
        }

        protected abstract TSyntaxTreeContext getSyntaxTreeContext();

        // TODO: Find a method to have types directly on CompilationContext
        // e.g., by having just CompilationContext generic and TypeContext subclass inside
        protected void AddType(CompilationContext compilation, TTypeContext type, TTypeContext parent)
        {
            if (type.Parent != null)
                throw new Exception("Can't re-add root type");

            if (type == parent)
                throw new Exception("The parent can't be same reference as the given type");

            type.Compilation = compilation;
            if (parent == null)
            {
                compilation.AddRootType(type);
            }
            else
            {
                type.Parent = parent;
                parent.AddChild(type);
            }
        }
    }
}
