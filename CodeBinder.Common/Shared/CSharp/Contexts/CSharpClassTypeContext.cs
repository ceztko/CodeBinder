// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// Class syntax context
    /// </summary>
    /// <remarks>Inherit this class if needed to extend a class context</remarks>
    public abstract class CSharpClassTypeContext<TCompilationContext, TTypeContext>
        : CSharpClassTypeContext, ITypeContext<TCompilationContext>
        where TCompilationContext : CSharpCompilationContext
        where TTypeContext : CSharpClassTypeContext
    {
        public CSharpClassTypeContext(ClassDeclarationSyntax node)
            : base(node) { }

        public new TCompilationContext Compilation => getCSharpCompilationContext();

        protected abstract TCompilationContext getCSharpCompilationContext();

        protected override CSharpCompilationContext GetCSharpCompilationContext() => getCSharpCompilationContext();

        protected abstract IEnumerable<TypeConversion<TTypeContext>> getConversions();

        protected override IEnumerable<TypeConversion> GetConversions() => getConversions();
    }

    public abstract class CSharpClassTypeContext : CSharpTypeContext<ClassDeclarationSyntax>
    {
        internal CSharpClassTypeContext(ClassDeclarationSyntax node)
            : base(node) { }

        protected override IEnumerable<TypeConversion> GetConversions()
        {
            return Compilation.Conversion.GetConversions(this);
        }
    }

    sealed class CSharpClassTypeContextImpl : CSharpClassTypeContext
    {
        public new CSharpCompilationContext Compilation { get; private set; }

        public CSharpClassTypeContextImpl(ClassDeclarationSyntax node, CSharpCompilationContext context)
            : base(node)
        {
            Compilation = context;
        }

        protected override CSharpCompilationContext GetCSharpCompilationContext() => Compilation;
    }
}
