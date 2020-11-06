// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// Interface syntax context
    /// </summary>
    /// <remarks>Inherit this class if needed to extend an interface context</remarks>
    public abstract class CSharpInterfaceTypeContext<TCompilationContext, TTypeContext>
        : CSharpInterfaceTypeContext, ITypeContext<TCompilationContext>
        where TCompilationContext : CSharpCompilationContext
        where TTypeContext : CSharpInterfaceTypeContext
    {
        public CSharpInterfaceTypeContext(InterfaceDeclarationSyntax node)
            : base(node) { }

        public new TCompilationContext Compilation => getCSharpCompilationContext();

        protected abstract TCompilationContext getCSharpCompilationContext();

        protected override CSharpCompilationContext GetCSharpCompilationContext() => getCSharpCompilationContext();

        protected abstract IEnumerable<TypeConversion<TTypeContext>> getConversions();

        protected override IEnumerable<TypeConversion> GetConversions() => getConversions();
    }

    public abstract class CSharpInterfaceTypeContext : CSharpTypeContext<InterfaceDeclarationSyntax>
    {
        internal CSharpInterfaceTypeContext(InterfaceDeclarationSyntax node)
            : base(node) { }

        protected override IEnumerable<TypeConversion> GetConversions()
        {
            return Compilation.Conversion.GetConversions(this);
        }
    }

    sealed class CSharpInterfaceTypeContextImpl : CSharpInterfaceTypeContext
    {
        public new CSharpCompilationContext Compilation { get; private set; }

        public CSharpInterfaceTypeContextImpl(InterfaceDeclarationSyntax node, CSharpCompilationContext compilation)
            : base(node)
        {
            Compilation = compilation;
        }

        protected override CSharpCompilationContext GetCSharpCompilationContext() => Compilation;
    }
}
