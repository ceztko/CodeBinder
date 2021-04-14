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
    public abstract class LanguageConversion<TCompilationContext, TTypeContext> : LanguageConversion
        where TCompilationContext : CompilationContext<TTypeContext>
        where TTypeContext : TypeContext<TTypeContext>
    {
        public LanguageConversion() { }

        internal sealed override CompilationContext CreateCompilationContext()
            => createCompilationContext();

        protected abstract TCompilationContext createCompilationContext();
    }

    public abstract class LanguageConversion
    {
        NamespaceMappingTree _NamespaceMapping;

        internal LanguageConversion()
        {
            _NamespaceMapping = new NamespaceMappingTree();
        }

        /// <summary>
        /// Add here policies supported by this language conversion
        /// </summary>
        public virtual IReadOnlyCollection<string> SupportedPolicies
        {
            get { return new string[] { }; }
        }

        /// <summary>
        /// True for native conversions (eg. CLang)
        /// </summary>
        public virtual bool IsNative => false;

        /// <summary>
        /// Default discard native syntax (eg. [DllImport] methods or [UnmanagedFunctionPointer] delegates)
        /// </summary>
        public virtual bool DiscardNative => false;

        /// <summary>
        /// False if namespace mapping is needed for this conversion
        /// </summary>
        public virtual bool NeedNamespaceMapping => true;

        public virtual bool UseUTF8Bom => true;

        /// <summary>
        /// Namespace mapping store
        /// </summary>
        public NamespaceMappingTree NamespaceMapping
        {
            get
            {
                if (!NeedNamespaceMapping)
                    throw new Exception("Namespace mapping not supported for this conversion");

                return _NamespaceMapping;
            }
        }

        public virtual IEnumerable<IConversionWriter> DefaultConversions
        {
            get { yield break; }
        }

        public virtual bool TryParseExtraArgs(List<string> args)
        {
            return false;
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
