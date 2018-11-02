using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    public abstract class CSharpTypeConversion<TTypeContext, TLanguageConversion> : TypeConversion<TTypeContext, TLanguageConversion>
        where TTypeContext : CSharpTypeContext
        where TLanguageConversion: CSharpLanguageConversion
    {
        protected CSharpTypeConversion(TLanguageConversion conversion)
            : base(conversion) { }
    }
}
