// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpLanguageConversion
        : LanguageConversion<CSharpCompilationContext, CSharpSyntaxTreeContext, CSharpNodeVisitor, CSharpBaseTypeContext>
    {
        protected override CSharpCompilationContext createCompilationContext()
        {
            return new CSharpCompilationContext(this);
        }

        public abstract TypeConversion<CSharpClassTypeContext> GetClassTypeConversion();

        public abstract TypeConversion<CSharpInterfaceTypeContext> GetInterfaceTypeConversion();

        public abstract TypeConversion<CSharpStructTypeContext> GetStructTypeConversion();

        public abstract TypeConversion<CSharpEnumTypeContext> GetEnumTypeConversion();
    }
}
