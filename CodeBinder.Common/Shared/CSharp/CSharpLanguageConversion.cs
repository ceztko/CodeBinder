// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// CSharp specific language conversion
    /// </summary>
    /// <remarks>Inherit this class if you need custom contexts/visitor</remarks>
    public abstract class CSharpLanguageConversion<TCompilationContext> : CSharpLanguageConversion
        where TCompilationContext : CSharpCompilationContext
    {
        protected abstract TCompilationContext createCSharpCompilationContext();

        internal override CSharpCompilationContext CreateCSharpCompilationContext()
        {
            return createCSharpCompilationContext();
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
    public abstract class CSharpLanguageConversion : LanguageConversion<CSharpCompilationContext, CSharpMemberTypeContext>
    {
        protected sealed override CSharpCompilationContext createCompilationContext()
        {
            return CreateCSharpCompilationContext();
        }

        internal virtual CSharpCompilationContext CreateCSharpCompilationContext()
        {
            return new CSharpCompilationContextImpl(this);
        }

        public abstract IEnumerable<TypeConversion<CSharpClassTypeContext>> GetConversions(CSharpClassTypeContext cls);

        public abstract IEnumerable<TypeConversion<CSharpInterfaceTypeContext>> GetConversions(CSharpInterfaceTypeContext iface);

        public abstract IEnumerable<TypeConversion<CSharpStructTypeContext>> GetConversions(CSharpStructTypeContext str);

        public abstract IEnumerable<TypeConversion<CSharpEnumTypeContext>> GetConversions(CSharpEnumTypeContext enm);

        public abstract IEnumerable<TypeConversion<CSharpDelegateTypeContext>> GetConversions(CSharpDelegateTypeContext dlg);
    }
}
