// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// Enum syntax context
/// </summary>
/// <remarks>Inherit this class if needed to extend an enum context</remarks>
public abstract class CSharpEnumTypeContext<TCompilationContext>
        : CSharpEnumTypeContext, ITypeContext<TCompilationContext>
    where TCompilationContext : CSharpCompilationContext
{
    TCompilationContext _Compilation;

    public CSharpEnumTypeContext(EnumDeclarationSyntax node, TCompilationContext compilation)
        : base(node)
    {
        _Compilation = compilation;
    }

    public override TCompilationContext Compilation => _Compilation;
}

public abstract class CSharpEnumTypeContext : CSharpBaseTypeContext<EnumDeclarationSyntax>
{
    internal CSharpEnumTypeContext(EnumDeclarationSyntax node)
        : base(node) { }

    protected override IEnumerable<TypeConversion> GetConversions()
    {
        return Compilation.Conversion.GetConversions(this);
    }

    public override string Name => Node.Identifier.Text;
}


sealed class CSharpEnumTypeContextImpl : CSharpEnumTypeContext
{
    CSharpCompilationContext _Compilation;

    public override CSharpCompilationContext Compilation => _Compilation;

    public CSharpEnumTypeContextImpl(EnumDeclarationSyntax node,  CSharpCompilationContext compilation)
        : base(node)
    {
        _Compilation = compilation;
    }
}
