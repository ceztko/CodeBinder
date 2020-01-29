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

        public abstract TypeConversion<CSharpClassTypeContext> CreateConversion(CSharpClassTypeContext cls);

        public abstract TypeConversion<CSharpInterfaceTypeContext> CreateConversion(CSharpInterfaceTypeContext iface);

        public abstract TypeConversion<CSharpStructTypeContext> CreateConversion(CSharpStructTypeContext str);

        public abstract TypeConversion<CSharpEnumTypeContext> CreateConversion(CSharpEnumTypeContext enm);
    }
}
