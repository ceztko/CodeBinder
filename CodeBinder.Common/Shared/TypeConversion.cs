// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Utils;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    public abstract class TypeConversion<TTypeContext, TCompilationContext, TLanguageConversion> : TypeConversion<TTypeContext, TCompilationContext>
        where TCompilationContext : CompilationContext
        where TTypeContext : TypeContext
        where TLanguageConversion : LanguageConversion
    {
        public TLanguageConversion Conversion { get; private set; }

        protected TypeConversion(TTypeContext context, TLanguageConversion conversion)
            : base(context)
        {
            Conversion = conversion;
        }
    }

    public abstract class TypeConversion<TTypeContext, TCompilationContext> : TypeConversion<TTypeContext>
        where TCompilationContext : CompilationContext
        where TTypeContext : TypeContext
    {
        internal TypeConversion(TTypeContext context)
            : base(context) { }

        public abstract new TCompilationContext Compilation { get; }
    }

    public abstract class TypeConversion<TTypeContext> : TypeConversion
        where TTypeContext : TypeContext
    {
        public new TTypeContext Context { get; private set; }

        internal TypeConversion(TTypeContext context)
        {
            Context = context;
        }

        protected override TypeContext GetContext()
        {
            return Context;
        }
    }

    public abstract class TypeConversion : ConversionWriter, ICompilationContextProvider
    {
        internal TypeConversion() { }

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
}
