// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Shared.CSharp
{

    /// <summary>
    /// CSharp specific language conversion
    /// </summary>
    /// <remarks>Inherit this class if you don't need custom contexts/visitor</remarks>
    public abstract class CSharpLanguageConversion
        : LanguageConversion<CSharpCompilationContext, CSharpSyntaxTreeContext, CSharpBaseTypeContext>
    {
        protected override CSharpCompilationContext createCompilationContext()
        {
            return new CSharpCompilationContextImpl(this);
        }

        public abstract TypeConversion<CSharpClassTypeContext> CreateConversion(CSharpClassTypeContext cls);

        public abstract TypeConversion<CSharpInterfaceTypeContext> CreateConversion(CSharpInterfaceTypeContext iface);

        public abstract TypeConversion<CSharpStructTypeContext> CreateConversion(CSharpStructTypeContext str);

        public abstract TypeConversion<CSharpEnumTypeContext> CreateConversion(CSharpEnumTypeContext enm);
    }
}
