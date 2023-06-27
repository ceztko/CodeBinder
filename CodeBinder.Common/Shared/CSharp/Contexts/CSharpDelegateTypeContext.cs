// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// Delegate syntax context
    /// </summary>
    /// <remarks>Inherit this class if needed to extend a delegate context</remarks>
    public abstract class CSharpDelegateTypeContext<TCompilationContext>
            : CSharpDelegateTypeContext, ITypeContext<TCompilationContext>
        where TCompilationContext : CSharpCompilationContext
    {
        TCompilationContext _Compilation;

        public CSharpDelegateTypeContext(DelegateDeclarationSyntax node, TCompilationContext compilation)
            : base(node)
        {
            _Compilation = compilation;
        }

        public override TCompilationContext Compilation => _Compilation;
    }

    public abstract class CSharpDelegateTypeContext : CSharpMemberTypeContext<DelegateDeclarationSyntax>
    {
        internal CSharpDelegateTypeContext(DelegateDeclarationSyntax node)
            : base(node) { }

        protected override IEnumerable<TypeConversion> GetConversions()
        {
            return Compilation.Conversion.GetConversions(this);
        }

        public override string Name => Node.Identifier.Text;
    }

    sealed class CSharpDelegateTypeContextImpl : CSharpDelegateTypeContext
    {
        CSharpCompilationContext _Compilation;

        public override CSharpCompilationContext Compilation => _Compilation;

        public CSharpDelegateTypeContextImpl(DelegateDeclarationSyntax node, CSharpCompilationContext compilation)
            : base(node)
        {
            _Compilation = compilation;
        }
    }
}
