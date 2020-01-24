// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    public abstract class TypeConversion : IConversionBuilder, ICompilationContextProvider
    {
        internal TypeConversion() { }

        public abstract void Write(CodeBuilder builder);

        public virtual string GeneratedPreamble
        {
            get { return string.Empty; }
        }

        public virtual IEnumerable<IConversionBuilder> Builders
        {
            get { yield return this; }
        }

        public abstract string FileName { get; }

        public virtual string BasePath
        {
            get { return string.Empty; }
        }

        public TypeContext Context
        {
            get { return GetContext(); }
        }

        public CompilationContext Compilation
        {
            get { return Context.Compilation; }
        }

        protected abstract TypeContext GetContext();
    }

    public abstract class TypeConversion<TTypeContext> : TypeConversion
        where TTypeContext : TypeContext
    {
        internal TypeConversion() { }

        // FIXME: Find a way to make it internal again, look JNI JNIModuleContextParent
        public new TTypeContext Context { get; /* internal */ set; } = null!;

        protected override TypeContext GetContext()
        {
            return Context;
        }
    }

    public abstract class TypeConversion<TTypeContext, TCompilationContext> : TypeConversion<TTypeContext>
        where TCompilationContext : CompilationContext
        where TTypeContext : TypeContext
    {
        internal TypeConversion() { }

        public abstract new TCompilationContext Compilation { get; }
    }

    public abstract class TypeConversion<TTypeContext, TCompilationContext, TLanguageConversion> : TypeConversion<TTypeContext, TCompilationContext>
        where TCompilationContext : CompilationContext
        where TTypeContext : TypeContext
        where TLanguageConversion : LanguageConversion
    {
        public TLanguageConversion Conversion { get; private set; }

        protected TypeConversion(TLanguageConversion conversion)
        {
            Conversion = conversion;
        }
    }
}
