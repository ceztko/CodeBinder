// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    public abstract class CSharpLanguageConversion
        : LanguageConversion<CSharpSyntaxTreeContext, CSharpTypeContext, CSharpNodeVisitor>
    {
        protected override sealed IEnumerable<CSharpTypeContext> SecondPass(CSharpSyntaxTreeContext context)
        {
            foreach (var node in context.Enums)
            {
                var enumContext = new CSharpEnumTypeContext(node, context);
                enumContext.Conversion = GetEnumTypeConversion(enumContext);
                yield return enumContext;
            }

            yield break;
        }

        protected abstract TypeConversion GetEnumTypeConversion(CSharpEnumTypeContext context);
    }
}
