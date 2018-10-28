// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Shared.CSharp
{
    public abstract class CSharpLanguageConversion
        : LanguageConversion<CSharpSyntaxTreeContext, CSharpTypeContext>
    {
        protected override CSharpSyntaxTreeContext getSyntaxTreeContext(SourceCompilation compilation)
        {
            return new CSharpSyntaxTreeContext(compilation, this);
        }

        public abstract TypeConversion<CSharpClassTypeContext> GetClassTypeConversion();

        public abstract TypeConversion<CSharpInterfaceTypeContext> GetInterfaceTypeConversion();

        public abstract TypeConversion<CSharpStructTypeContext> GetStructTypeConversion();

        public abstract TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion();
    }
}
