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

        public TypeContext TypeContext
        {
            get { return GetTypeContext(); }
        }

        public CompilationContext Compilation
        {
            get { return TypeContext.Compilation; }
        }

        protected abstract TypeContext GetTypeContext();
    }

    public abstract class TypeConversion<TTypeContext> : TypeConversion
        where TTypeContext : TypeContext
    {
        // FIXME: Find a way to make it internal again, look JNI JNIModuleContextParent
        public new TTypeContext TypeContext { get; /* internal */ set; }

        protected override TypeContext GetTypeContext()
        {
            return TypeContext;
        }
    }

    public abstract class TypeConversion<TTypeContext, TLanguageConversion> : TypeConversion<TTypeContext>
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
