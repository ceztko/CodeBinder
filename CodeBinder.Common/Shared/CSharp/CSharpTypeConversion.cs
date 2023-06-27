// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// TypeConversion specialized for CSharp conversions
/// </summary>
/// <remarks>Inherit this if needed to specialize the compilation context</remarks>
public abstract class CSharpTypeConversion<TTypeContext, TCompilationContext, TLanguageConversion> : TypeConversion<TTypeContext, TCompilationContext, TLanguageConversion>
    where TTypeContext : CSharpMemberTypeContext,ITypeContext<TCompilationContext>
    where TCompilationContext : CSharpCompilationContext
    where TLanguageConversion: CSharpLanguageConversion
{
    protected CSharpTypeConversion(TTypeContext context, TLanguageConversion conversion)
        : base(context, conversion) { }

    public override TCompilationContext Compilation => (Context as ITypeContext<TCompilationContext>).Compilation;

    protected IReadOnlyList<string> GetImports(SyntaxNode node)
    {
        var ret = new List<string>();
        var attributes = node.GetAttributes(this);
        foreach (var attribute in attributes)
        {
            if (attribute.IsAttribute<ImportAttribute>())
                ret.Add(attribute.GetConstructorArgument<string>(0));
        }
        return ret;
    }
}

/// <summary>
/// TypeConversion specialized for CSharp conversions
/// </summary>
/// <remarks>Inherit this if not needed to specialize the compilation context</remarks>
public abstract class CSharpTypeConversion<TTypeContext, TLanguageConversion> : CSharpTypeConversion<TTypeContext, CSharpCompilationContext, TLanguageConversion>
    where TTypeContext : CSharpMemberTypeContext
    where TLanguageConversion : CSharpLanguageConversion
{
    protected CSharpTypeConversion(TTypeContext context, TLanguageConversion conversion)
        : base(context, conversion) { }

    public override CSharpCompilationContext Compilation => Context.Compilation;
}
