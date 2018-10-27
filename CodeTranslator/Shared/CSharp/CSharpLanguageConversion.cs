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
            foreach (var node in treeContext.Interfaces)
                yield return new CSharpInterfaceTypeContext(node, treeContext, GetInterfaceTypeConversion());

            foreach (var node in treeContext.Classes)
                yield return new CSharpClassTypeContext(node, treeContext, GetClassTypeConversion());

            foreach (var node in treeContext.Structs)
                yield return new CSharpStructTypeContext(node, treeContext, GetStructTypeConversion());

            foreach (var node in treeContext.Enums)
                yield return new CSharpEnumTypeContext(node, treeContext, GetEnumTypeConversion());

            yield break;
        }

        protected abstract TypeConversion<CSharpInterfaceTypeContext> GetInterfaceTypeConversion();

        protected abstract TypeConversion<CSharpClassTypeContext> GetClassTypeConversion();

        protected abstract TypeConversion<CSharpStructTypeContext> GetStructTypeConversion();

        protected abstract TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion();
    }
}
