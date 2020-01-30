// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpEnumTypeContext : CSharpBaseTypeContext<EnumDeclarationSyntax, CSharpEnumTypeContext>
    {
        protected CSharpEnumTypeContext(EnumDeclarationSyntax node)
            : base(node) { }

        protected override TypeConversion<CSharpEnumTypeContext> createConversion()
        {
            return Compilation.Conversion.CreateConversion(this);
        }
    }

    sealed class CSharpEnumTypeContextImpl : CSharpEnumTypeContext
    {
        public new CSharpCompilationContext Compilation { get; private set; }

        public CSharpEnumTypeContextImpl(EnumDeclarationSyntax node,  CSharpCompilationContext compilation)
            : base(node)
        {
            Compilation = compilation;
        }

        protected override CSharpCompilationContext getCompilationContext() => Compilation;
    }
}
