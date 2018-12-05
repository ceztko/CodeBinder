using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpTypeConversion<TTypeContext, TLanguageConversion> : TypeConversion<TTypeContext, TLanguageConversion>
        where TTypeContext : CSharpBaseTypeContext
        where TLanguageConversion: CSharpLanguageConversion
    {
        protected CSharpTypeConversion(TLanguageConversion conversion)
            : base(conversion) { }
    }
}
