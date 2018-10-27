using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    public abstract class CSharpTypeConversion<TTypeContext> : TypeConversion<TTypeContext>
        where TTypeContext : CSharpTypeContext { }
}
