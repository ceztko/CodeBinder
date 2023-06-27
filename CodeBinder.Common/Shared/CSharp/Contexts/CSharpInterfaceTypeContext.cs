// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// Interface syntax context
/// </summary>
/// <remarks>Inherit this class if needed to extend an interface context</remarks>
public abstract class CSharpInterfaceTypeContext<TCompilationContext>
        : CSharpInterfaceTypeContext, ITypeContext<TCompilationContext>
    where TCompilationContext : CSharpCompilationContext
{
    TCompilationContext _Compilation;

    public CSharpInterfaceTypeContext(InterfaceDeclarationSyntax node, TCompilationContext compilation)
        : base(node)
    {
        _Compilation = compilation;
    }

    public override TCompilationContext Compilation => _Compilation;
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
    CSharpCompilationContext _Compilation;

    public override CSharpCompilationContext Compilation => _Compilation;

    public CSharpInterfaceTypeContextImpl(InterfaceDeclarationSyntax node, CSharpCompilationContext compilation)
        : base(node)
    {
        _Compilation = compilation;
    }
}
