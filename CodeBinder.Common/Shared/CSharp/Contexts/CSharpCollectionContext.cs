// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// CSharp specific collection context
/// </summary>
/// <remarks>Inherit this class if you need to collect a full CSharp compilation context</remarks>
public class CSharpCollectionContext<TCompilationContext> : CSharpCollectionContext
    where TCompilationContext : CSharpCompilationContext
{
    TCompilationContext _Compilation;

    protected CSharpCollectionContext(TCompilationContext compilation)
    {
        _Compilation = compilation;
    }

    public override TCompilationContext Compilation => _Compilation;
}

/// <summary>
/// CSharp specific collection context
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public abstract class CSharpCollectionContext : CSharpCollectionContextBase
{
    Dictionary<string, List<CSharpTypeContext>> _types;

    internal CSharpCollectionContext()
    {
        _types = new Dictionary<string, List<CSharpTypeContext>>();
        Init += CSharpCollectionContext_Initialized;
    }

    private void CSharpCollectionContext_Initialized(CSharpNodeVisitor visitor)
    {
        // Declarations
        visitor.ClassDeclarationVisit += Visitor_ClassDeclarationVisit;
        visitor.StructDeclarationVisit += Visitor_StructDeclarationVisit;
        visitor.InterfaceDeclarationVisit += Visitor_InterfaceDeclarationVisit;
        visitor.EnumDeclarationVisit += Visitor_EnumDeclarationVisit;
        visitor.DelegateDeclarationVisit += Visitor_DelegateDeclarationVisit;
        visitor.MethodDeclarationVisit += Visitor_MethodDeclarationVisit;

        visitor.Visited += Visitor_Visited;
    }

    private void Visitor_MethodDeclarationVisit(CSharpNodeVisitor visitor, MethodDeclarationSyntax node)
    {
        if (node.Identifier.Text == "FreeHandle" && node.Body != null)
        {
            var symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
            if (symbol.OverriddenMethod?.ContainingType.GetFullName() == "CodeBinder.HandledObjectBase")
                Compilation.AddFinalizer(node);
        }
    }

    private void Visitor_ClassDeclarationVisit(CSharpNodeVisitor visitor, ClassDeclarationSyntax node)
    {
        // Skip binders
        var symbol = node.GetDeclaredSymbol<ITypeSymbol>(this);
        if (symbol.Inherits<NativeTypeBinder>())
            return;

        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        addTypeContext(typeCtx);
        Compilation.AddClass(typeCtx);
    }

    private void Visitor_StructDeclarationVisit(CSharpNodeVisitor visitor, StructDeclarationSyntax node)
    {
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        addTypeContext(typeCtx);
        Compilation.AddStruct(typeCtx);
    }

    private void Visitor_InterfaceDeclarationVisit(CSharpNodeVisitor visitor, InterfaceDeclarationSyntax node)
    {
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        addTypeContext(typeCtx);
        Compilation.AddInterface(typeCtx);
    }

    private void Visitor_EnumDeclarationVisit(CSharpNodeVisitor visitor, EnumDeclarationSyntax node)
    {
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        Compilation.AddTypeContext(typeCtx, null);
        Compilation.AddEnum(typeCtx);
    }

    private void Visitor_DelegateDeclarationVisit(CSharpNodeVisitor visitor, DelegateDeclarationSyntax node)
    {
        var typeCtx = Compilation.CreateContext(node);
        typeCtx.Init();
        Compilation.AddDelegate(typeCtx);
    }

    private void Visitor_Visited(NodeVisitor visitor)
    {
        var mainTypesMap = new Dictionary<ITypeSymbol, CSharpTypeContext>(SymbolEqualityComparer.Default);
        foreach (var types in _types.Values)
        {
            var main = types[0];
            var symbol = main.Node.GetDeclaredSymbol<ITypeSymbol>(this);
            Compilation.AddNamespace(symbol.GetContainingNamespace());
            for (int i = 0; i < types.Count; i++)
                main.AddPartialDeclaration(types[i]);

            mainTypesMap[symbol] = main;
        }

        foreach (var type in mainTypesMap.Values)
        {
            var symbol = type.Node.GetDeclaredSymbol<ITypeSymbol>(this);
            if (symbol.ContainingType == null)
                Compilation.AddTypeContext(type, null);
            else
                // We assume partial types are contained in partial types, see visitor
                Compilation.AddTypeContext(type, mainTypesMap[symbol.ContainingType]);
        }
    }

    void addTypeContext(CSharpTypeContext type)
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

    public override abstract CSharpCompilationContext Compilation { get; }
}

/// <summary>
/// CSharp specific collection context
/// </summary>
/// <remarks>Inherit this class if you need to collect a generic compilation context</remarks>
public abstract class CSharpCollectionContextBase<TCompilationContext> : CSharpCollectionContextBase
        where TCompilationContext : CompilationContext
{
    TCompilationContext _Compilation;

    protected CSharpCollectionContextBase(TCompilationContext compilation)
    {
        _Compilation = compilation;
    }

    public override TCompilationContext Compilation => _Compilation;
}

/// <summary>
/// CSharp specific collection context
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public abstract class CSharpCollectionContextBase : CollectionContext<CSharpNodeVisitor>
{
    Dictionary<string, List<IMethodSymbol>> _uniqueMethodNames = null!;

    internal CSharpCollectionContextBase()
    {
        Init += CSharpCollectionContextBase_VisitorInit;
    }

    private void CSharpCollectionContextBase_VisitorInit(CSharpNodeVisitor visitor)
    {
        if (Compilation.Conversion.CheckMethodOverloads)
            _uniqueMethodNames = new Dictionary<string, List<IMethodSymbol>>();

        // Declarations
        visitor.MethodDeclarationVisit += Visitor_MethodDeclarationVisit;
        visitor.ConstructorDeclarationVisit += Visitor_ConstructorDeclarationVisit;

        visitor.BeforeNodeVisit += Visitor_BeforeNodeVisit;
    }

    private void Visitor_BeforeNodeVisit(NodeVisitor visitor, SyntaxNode node, NodeVisitorToken token)
    {
        if (node.ShouldDiscard(Compilation))
        {
            token.Cancel();
            return;
        }
    }

    private void Visitor_MethodDeclarationVisit(CSharpNodeVisitor visitor, MethodDeclarationSyntax node)
    {
        var symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
        if (!(node.IsPartialMethod(out var hasEmptyBody) && hasEmptyBody))
            addSymbolBinding(symbol);
    }

    private void Visitor_ConstructorDeclarationVisit(CSharpNodeVisitor visitor, ConstructorDeclarationSyntax node)
    {
        var symbol = node.GetDeclaredSymbol<IMethodSymbol>(this);
        addSymbolBinding(symbol);
    }

    // Construct a method binding to handle overloaded methods/constructors
    void addSymbolBinding(IMethodSymbol methodSymbol)
    {
        var bindedName = methodSymbol.GetBindedName(Compilation.Conversion, out bool isOverload);
        if (Compilation.Conversion.CheckMethodOverloads)
        {
            string qualifiedBindedName = $"{methodSymbol.ContainingType.GetFullName()}.{bindedName}";
            List<IMethodSymbol>? bindedMethods;
            if (_uniqueMethodNames.TryGetValue(qualifiedBindedName, out bindedMethods))
            {
                bool doParameterOverlaps = false;
                foreach (var bindendMethodSymbol in bindedMethods)
                {
                    doParameterOverlaps = methodSymbol.DoParameterCountOverlap(bindendMethodSymbol);
                    if (doParameterOverlaps)
                        break;
                }

                if (!doParameterOverlaps)
                    bindedMethods.Add(methodSymbol);
            }
            else
            {
                bindedMethods = new List<IMethodSymbol> { methodSymbol };
                _uniqueMethodNames.Add(qualifiedBindedName, bindedMethods);
            }
        }

        Compilation.AddBinding(methodSymbol, bindedName, isOverload);
    }
}

class CSharpCollectionContextImpl : CSharpCollectionContext<CSharpCompilationContext>
{
    public CSharpCollectionContextImpl(CSharpCompilationContext compilation)
        : base(compilation) { }
}
