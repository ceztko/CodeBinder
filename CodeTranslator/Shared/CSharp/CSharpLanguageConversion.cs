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
        protected override sealed IEnumerable<CSharpTypeContext> SecondPass(CSharpSyntaxTreeContext treeContext)
        {
            foreach (var node in treeContext.Enums)
            {
                var conversion = GetEnumTypeConversion();
                yield return new CSharpEnumTypeContext(node, treeContext, conversion);
            }

            yield break;
        }

        protected abstract TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion();
    }
}
