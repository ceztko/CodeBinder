using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// Basic csharp compilation context
    /// </summary>
    /// <remarks>Inherit this class to extend the context</remarks>
    public abstract class CSharpCompilationContext<TSyntaxTreeContext> : CSharpCompilationContext
        where TSyntaxTreeContext : CSharpSyntaxTreeContext
    {
        protected CSharpCompilationContext() { }

        protected abstract TSyntaxTreeContext CreateCSharpSyntaxTreeContext();

        protected sealed override CSharpSyntaxTreeContext createSyntaxTreeContext() => CreateCSharpSyntaxTreeContext();
    }

    public abstract class CSharpCompilationContext : CompilationContext<CSharpBaseTypeContext, CSharpSyntaxTreeContext, CSharpLanguageConversion>
    {
        Dictionary<string, CSharpTypeContext> _partialTypes;

        internal CSharpCompilationContext()
        {
            _partialTypes = new Dictionary<string, CSharpTypeContext>();
        }

        protected override CSharpSyntaxTreeContext createSyntaxTreeContext()
        {
            return new CSharpSyntaxTreeContextImpl(this);
        }

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

        public void AddPartialType(string qualifiedName, CompilationContext compilation, CSharpTypeContext type, CSharpBaseTypeContext? parent)
        {
            if (_partialTypes.TryGetValue(qualifiedName, out var partialType))
            {
                partialType.AddPartialDeclaration(type);
            }
            else
            {
                type.AddPartialDeclaration(type);
                _partialTypes.Add(qualifiedName, type);
                AddType(type, parent);
            }
        }

        public bool TryGetPartialType(string qualifiedName, [NotNullWhen(true)]out CSharpTypeContext? partialType)
        {
            return _partialTypes.TryGetValue(qualifiedName, out partialType);
        }
    }

    sealed class CSharpCompilationContextImpl : CSharpCompilationContext
    {
        public new CSharpLanguageConversion Conversion { get; private set; }

        public CSharpCompilationContextImpl(CSharpLanguageConversion conversion)
        {
            Conversion = conversion;
        }

        protected override CSharpLanguageConversion GetLanguageConversion() => Conversion;
    }
}
