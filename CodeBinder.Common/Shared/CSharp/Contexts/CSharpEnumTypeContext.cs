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
    /// <summary>
    /// Enum syntax context
    /// </summary>
    /// <remarks>Inherit this class if needed to extend an enum context</remarks>
    public abstract class CSharpEnumTypeContext<TCompilationContext, TTypeContext>
        : CSharpEnumTypeContext, ITypeContext<TCompilationContext>
        where TCompilationContext : CSharpCompilationContext
        where TTypeContext : CSharpEnumTypeContext
    {
        public CSharpEnumTypeContext(EnumDeclarationSyntax node)
            : base(node) { }

        public new TCompilationContext Compilation => getCSharpCompilationContext();

        protected abstract TCompilationContext getCSharpCompilationContext();

        protected override CSharpCompilationContext GetCSharpCompilationContext() => getCSharpCompilationContext();

        protected abstract IEnumerable<TypeConversion<TTypeContext>> getConversions();

        protected override IEnumerable<TypeConversion> GetConversions() => getConversions();
    }

    public abstract class CSharpEnumTypeContext : CSharpBaseTypeContext<EnumDeclarationSyntax, CSharpEnumTypeContext>
    {
        internal CSharpEnumTypeContext(EnumDeclarationSyntax node)
            : base(node) { }

        protected override IEnumerable<TypeConversion> GetConversions()
        {
            return Compilation.Conversion.GetConversions(this);
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

        protected override CSharpCompilationContext GetCSharpCompilationContext() => Compilation;
    }
}
