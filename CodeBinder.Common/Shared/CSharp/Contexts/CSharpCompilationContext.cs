using Microsoft.CodeAnalysis;
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
        Dictionary<string, List<CSharpTypeContext>> _types;

        internal CSharpCompilationContext()
        {
            _types = new Dictionary<string, List<CSharpTypeContext>>();
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

        // FIXME: Remove me and do this in a compilation context scoped visitor
        public void AddType(CSharpTypeContext type)
        {
            string fullName = type.Node.GetFullName(this);
            if (!_types.TryGetValue(fullName, out var types))
            {
                types = new List<CSharpTypeContext>();
                _types.Add(fullName, types);
            }

            // If the type is the main declaration, put it first, otherwise just put back
            if (type.Node.BaseList == null)
                types.Add(type);
            else
                types.Insert(0, type);
        }

        // FIXME: Remove me and do this in a compilation context scoped visitor
        protected internal override void AfterVisit()
        {
            var mainTypesMap = new Dictionary<ITypeSymbol, CSharpTypeContext>();
            foreach (var types in _types.Values)
            {
                var main = types[0];
                var symbol = main.Node.GetDeclaredSymbol<ITypeSymbol>(this);
                for (int i = 0; i < types.Count; i++)
                    main.AddPartialDeclaration(types[i]);

                mainTypesMap[symbol] = main;
            }

            foreach (var type in mainTypesMap.Values)
            {
                var symbol = type.Node.GetDeclaredSymbol<ITypeSymbol>(this);
                if (symbol.ContainingType == null)
                    AddType(type, null);
                else
                    // We assume partial types are contained in partial types, see visitor
                    AddType(type, mainTypesMap[symbol.ContainingType]);
            }
        }
    }

    sealed class CSharpCompilationContextImpl : CSharpCompilationContext
    {
        public new CSharpLanguageConversion Conversion { get; private set; }

        public CSharpCompilationContextImpl(CSharpLanguageConversion conversion)
        {
            Conversion = conversion;
        }

        protected override CSharpLanguageConversion getLanguageConversion() => Conversion;
    }
}
