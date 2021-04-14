// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// TypeConversion specialized for CSharp conversions
    /// </summary>
    /// <remarks>Inherit this if needed to specialize the compilation context</remarks>
    public abstract class CSharpTypeConversion<TTypeContext, TCompilationContext, TLanguageConversion> : TypeConversion<TTypeContext, TCompilationContext, TLanguageConversion>
        where TTypeContext : CSharpBaseTypeContext,ITypeContext<TCompilationContext>
        where TCompilationContext : CSharpCompilationContext
        where TLanguageConversion: CSharpLanguageConversion
    {
        protected CSharpTypeConversion(TTypeContext context, TLanguageConversion conversion)
            : base(context, conversion) { }

        public override TCompilationContext Compilation => (Context as ITypeContext<TCompilationContext>).Compilation;
    }

    /// <summary>
    /// TypeConversion specialized for CSharp conversions
    /// </summary>
    /// <remarks>Inherit this if not needed to specialize the compilation context</remarks>
    public abstract class CSharpTypeConversion<TTypeContext, TLanguageConversion> : CSharpTypeConversion<TTypeContext, CSharpCompilationContext, TLanguageConversion>
        where TTypeContext : CSharpBaseTypeContext
        where TLanguageConversion : CSharpLanguageConversion
    {
        protected CSharpTypeConversion(TTypeContext context, TLanguageConversion conversion)
            : base(context, conversion) { }

        public override CSharpCompilationContext Compilation => Context.Compilation;
    }
}
