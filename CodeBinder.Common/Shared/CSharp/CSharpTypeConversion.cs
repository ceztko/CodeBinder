using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpTypeConversion<TTypeContext, TCompilationContext, TLanguageConversion> : TypeConversion<TTypeContext, TCompilationContext, TLanguageConversion>
        where TTypeContext : CSharpBaseTypeContext
        where TCompilationContext : CSharpCompilationContext
        where TLanguageConversion: CSharpLanguageConversion
    {
        protected CSharpTypeConversion(TLanguageConversion conversion)
            : base(conversion) { }
    }

    public abstract class CSharpTypeConversion<TTypeContext, TLanguageConversion> : CSharpTypeConversion<TTypeContext, CSharpCompilationContext, TLanguageConversion>
        where TTypeContext : CSharpBaseTypeContext
        where TLanguageConversion : CSharpLanguageConversion
    {
        protected CSharpTypeConversion(TLanguageConversion conversion)
            : base(conversion) { }

        public override CSharpCompilationContext Compilation
        {
            get { return Context.Compilation; }
        }
    }
}
