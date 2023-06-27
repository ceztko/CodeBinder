// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// Struct syntax context
/// </summary>
/// <remarks>Inherit this class if needed to extend a struct context</remarks>
public abstract class CSharpStructTypeContext<TCompilationContext>
        : CSharpStructTypeContext, ITypeContext<TCompilationContext>
    where TCompilationContext : CSharpCompilationContext
{
    TCompilationContext _Compilation;

    public CSharpStructTypeContext(StructDeclarationSyntax node, TCompilationContext compilation)
        : base(node)
    {
        _Compilation = compilation;
    }

    public override TCompilationContext Compilation => _Compilation;
}

public abstract class CSharpStructTypeContext : CSharpTypeContext<StructDeclarationSyntax>
{
    protected CSharpStructTypeContext(StructDeclarationSyntax node)
        : base(node) { }

    protected override IEnumerable<TypeConversion> GetConversions()
    {
        return Compilation.Conversion.GetConversions(this);
    }
}

public sealed class CSharpStructTypeContextImpl : CSharpStructTypeContext
{
    CSharpCompilationContext _Compilation;

    public override CSharpCompilationContext Compilation => _Compilation;

    public CSharpStructTypeContextImpl(StructDeclarationSyntax node, CSharpCompilationContext compilation)
        : base(node)
    {
        _Compilation = compilation;
    }
}
