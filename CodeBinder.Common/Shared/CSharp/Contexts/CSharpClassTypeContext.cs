// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
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
    public abstract class CSharpClassTypeContext<TCompilationContext>
            : CSharpClassTypeContext, ITypeContext<TCompilationContext>
        where TCompilationContext : CSharpCompilationContext
    {
        TCompilationContext _Compilation;

        public CSharpClassTypeContext(ClassDeclarationSyntax node, TCompilationContext compilation)
            : base(node)
        {
            _Compilation = compilation;
        }

        public override TCompilationContext Compilation => _Compilation;
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
        CSharpCompilationContext _Compilation;

        public override CSharpCompilationContext Compilation => _Compilation;

        public CSharpClassTypeContextImpl(ClassDeclarationSyntax node, CSharpCompilationContext compilation)
            : base(node)
        {
            _Compilation = compilation;
        }
    }
}
