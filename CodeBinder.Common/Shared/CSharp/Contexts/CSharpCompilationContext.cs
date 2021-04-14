// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpCompilationContext : CompilationContext<CSharpBaseTypeContext, CSharpLanguageConversion>
    {
        public CSharpCompilationContext() { }

        public virtual CSharpClassTypeContext CreateContext(ClassDeclarationSyntax cls)
        {
            return new CSharpClassTypeContextImpl(cls, this);
        }

        public virtual CSharpEnumTypeContext CreateContext(EnumDeclarationSyntax enm)
        {
            return new CSharpEnumTypeContextImpl(enm, this);
        }

        public virtual CSharpInterfaceTypeContext CreateContext(InterfaceDeclarationSyntax iface)
        {
            return new CSharpInterfaceTypeContextImpl(iface, this);
        }

        public virtual CSharpStructTypeContext CreateContext(StructDeclarationSyntax str)
        {
            return new CSharpStructTypeContextImpl(str, this);
        }
    }

    sealed class CSharpCompilationContextImpl : CSharpCompilationContext
    {
        public new CSharpLanguageConversion Conversion { get; private set; }

        public CSharpCompilationContextImpl(CSharpLanguageConversion conversion)
        {
            Conversion = conversion;
        }

        protected internal override INodeVisitor CreateVisitor()
        {
            return new CSharpNodeVisitorImpl(this);
        }

        protected override CSharpLanguageConversion getLanguageConversion() => Conversion;
    }
}
