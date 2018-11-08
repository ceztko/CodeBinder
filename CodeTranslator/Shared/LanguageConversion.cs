// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Shared
{
    public abstract class LanguageConversion
    {
        public CompilationContext Compilation { get; internal set; }

        internal LanguageConversion() { }

        public abstract SyntaxTreeContext GetSyntaxTreeContext();

        public virtual string GetWarningsOrNull()
        {
            return CompilationWarnings.WarningsForCompilation(Compilation, "source");
        }

        public virtual IEnumerable<ConversionBuilder> DefaultConversions
        {
            get { yield break; }
        }

        public IEnumerable<TypeContext> RootTypes
        {
            get { return GetRootTypes(); }
        }

        protected abstract IEnumerable<TypeContext> GetRootTypes();

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
        List<TTypeContext> m_rootTypes;

        protected LanguageConversion()
        {
            m_rootTypes = new List<TTypeContext>();
        }

        public sealed override SyntaxTreeContext GetSyntaxTreeContext()
        {
            return getSyntaxTreeContext();
        }

        protected abstract TSyntaxTreeContext getSyntaxTreeContext();

        protected void AddType(TTypeContext type, TTypeContext parent)
        {
            type.Compilation = Compilation;
            SyntaxTreeContext.AddRootType(m_rootTypes, type, parent);
        }

        protected override IEnumerable<TypeContext> GetRootTypes()
        {
            return m_rootTypes;
        }
    }
}
