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
    /// <summary>
    /// Basic language conversion
    /// </summary>
    /// <remarks>Inherit this class to provide custom contexts</remarks>
    public abstract class LanguageConversion<TCompilationContext, TSyntaxTreeContext, TTypeContext> : LanguageConversion
        where TCompilationContext : CompilationContext<TTypeContext>
        where TSyntaxTreeContext : CompilationContext<TTypeContext>.SyntaxTree
        where TTypeContext : TypeContext<TTypeContext>
    {
        public LanguageConversion() { }

        internal sealed override CompilationContext CreateCompilationContext()
            => createCompilationContext();

        protected abstract TCompilationContext createCompilationContext();
    }

    public abstract class LanguageConversion
    {
        internal LanguageConversion()
        {
            NamespaceMapping = new NamespaceMappingTree();
        }

        /// <summary>
        /// Namespace mapping store
        /// </summary>
        public NamespaceMappingTree NamespaceMapping { get; private set; }

        public virtual IEnumerable<IConversionBuilder> DefaultConversions
        {
            get { yield break; }
        }

        public virtual bool UseUTF8Bom
        {
            get { return true; }
        }

        /// <summary>Conditional compilation symbols</summary>
        public virtual IReadOnlyList<string> PreprocessorDefinitions
        {
            get { return new string[0]; }
        }

        internal IEnumerable<ConversionDelegate> DefaultConversionDelegates
        {
            get
            {
                foreach (var conversion in DefaultConversions)
                    yield return new ConversionDelegate(conversion);
            }
        }

        internal CompilationContext GetCompilationContext(Compilation compilation)
        {
            var ret = CreateCompilationContext();
            ret.Compilation = compilation;
            return ret;
        }

        internal abstract CompilationContext CreateCompilationContext();
    }
}
