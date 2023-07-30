// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// CSharp specific language conversion
/// </summary>
/// <remarks>Inherit this class if you need custom contexts/visitor</remarks>
public abstract class CSharpLanguageConversion<TCompilationContext> : CSharpLanguageConversion
    where TCompilationContext : CSharpCompilationContext
{
    internal protected abstract TCompilationContext CreateCSharpCompilationContext();

    internal protected sealed override CSharpCompilationContext CreateCompilationContext()
    {
        return CreateCSharpCompilationContext();
    }

    public override IEnumerable<TypeConversion<CSharpClassTypeContext>> GetConversions(CSharpClassTypeContext cls)
    {
        yield break;
    }

    public override IEnumerable<TypeConversion<CSharpEnumTypeContext>> GetConversions(CSharpEnumTypeContext enm)
    {
        yield break;
    }

    public override IEnumerable<TypeConversion<CSharpInterfaceTypeContext>> GetConversions(CSharpInterfaceTypeContext iface)
    {
        yield break;
    }

    public override IEnumerable<TypeConversion<CSharpStructTypeContext>> GetConversions(CSharpStructTypeContext str)
    {
        yield break;
    }

    public override IEnumerable<TypeConversion<CSharpDelegateTypeContext>> GetConversions(CSharpDelegateTypeContext dlg)
    {
        yield break;
    }
}

/// <summary>
/// CSharp specific language conversion
/// </summary>
/// <remarks>Inherit this class if you don't need custom contexts/visitor</remarks>
public abstract class CSharpLanguageConversion : CSharpLanguageConversionBase<CSharpCompilationContext, CSharpMemberTypeContext>
{
    protected CSharpLanguageConversion() { }

    internal protected override CSharpCompilationContext CreateCompilationContext()
    {
        return new CSharpCompilationContextImpl(this);
    }

    public abstract IEnumerable<TypeConversion<CSharpClassTypeContext>> GetConversions(CSharpClassTypeContext cls);

    public abstract IEnumerable<TypeConversion<CSharpInterfaceTypeContext>> GetConversions(CSharpInterfaceTypeContext iface);

    public abstract IEnumerable<TypeConversion<CSharpStructTypeContext>> GetConversions(CSharpStructTypeContext str);

    public abstract IEnumerable<TypeConversion<CSharpEnumTypeContext>> GetConversions(CSharpEnumTypeContext enm);

    public abstract IEnumerable<TypeConversion<CSharpDelegateTypeContext>> GetConversions(CSharpDelegateTypeContext dlg);


    protected internal override CSharpValidationContext? CreateValidationContext()
    {
        return new CSharpValidationContextImpl(this);
    }
}

// TODO: Make a non generic CSharpLanguageConversionBase?
/// <summary>
/// CSharp specific language conversion
/// </summary>
/// <remarks>Inherit this class if you don't need custom visitor</remarks>
public abstract class CSharpLanguageConversionBase<TCompilationContext, TTypeContext> : CSharpLanguageConversionBase
    where TCompilationContext : CompilationContext<TTypeContext, CSharpNodeVisitor>
    where TTypeContext : TypeContext<TTypeContext>
{
    protected CSharpLanguageConversionBase() { }

    internal protected abstract override TCompilationContext CreateCompilationContext();
}

/// <summary>
/// CSharp specific language conversion
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public abstract class CSharpLanguageConversionBase : LanguageConversion
{
    internal CSharpLanguageConversionBase() { }

    internal protected sealed override NodeVisitor CreateVisitor()
    {
        return new CSharpNodeVisitor();
    }

    protected internal override CSharpValidationContextBase? CreateValidationContext()
    {
        return new CSharpValidationContextBaseImpl(this);
    }
}

