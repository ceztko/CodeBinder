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
        internal LanguageConversion() { }

        public abstract SyntaxTreeContext GetSyntaxTreeContext(CompilationContext compilation);

        public virtual string GetWarningsOrNull(CompilationContext compilation)
        {
            return CompilationWarnings.WarningsForCompilation(compilation, "source");
        }

        public virtual IEnumerable<ConversionResult> DefaultResults
        {
            get { yield break; }
        }
    }

    public abstract class LanguageConversion<TSyntaxTreeContext, TTypeContext> : LanguageConversion
        where TSyntaxTreeContext : SyntaxTreeContext<TTypeContext>
        where TTypeContext : TypeContext<TTypeContext>
    {
        public sealed override SyntaxTreeContext GetSyntaxTreeContext(CompilationContext compilation)
        {
            return getSyntaxTreeContext(compilation);
        }

        protected abstract TSyntaxTreeContext getSyntaxTreeContext(CompilationContext compilation);
    }
}
