// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using System.Collections;
using System.Linq;

namespace CodeBinder.Shared.CSharp;

/// <summary>
/// CSharp compilation context with CSharp specific type context
/// </summary>
/// <remarks>Inherit this class if you will use custom CSharp type contexts</remarks>
public abstract class CSharpCompilationContext<TLanguageConversion,
        TClassContext, TStructContext, TInterfaceContext,
        TEnumContext, TDelegateContext> : CSharpCompilationContext
    where TLanguageConversion : CSharpLanguageConversion
    where TClassContext : CSharpClassTypeContext
    where TStructContext : CSharpStructTypeContext
    where TInterfaceContext : CSharpInterfaceTypeContext
    where TEnumContext : CSharpEnumTypeContext
    where TDelegateContext : CSharpDelegateTypeContext
{
    TLanguageConversion _Conversion;
    TypeContextCollection<TClassContext, ClassDeclarationSyntax> _classes;
    TypeContextCollection<TStructContext, StructDeclarationSyntax> _structs;
    TypeContextCollection<TInterfaceContext, InterfaceDeclarationSyntax> _interfaces;
    TypeContextCollection<TEnumContext, EnumDeclarationSyntax> _enums;
    TypeContextCollection<TDelegateContext, DelegateDeclarationSyntax> _delegates;

    protected CSharpCompilationContext(TLanguageConversion conversion)
    {
        _Conversion = conversion;
        _classes = new TypeContextCollection<TClassContext, ClassDeclarationSyntax>(this);
        _structs = new TypeContextCollection<TStructContext, StructDeclarationSyntax>(this);
        _interfaces = new TypeContextCollection<TInterfaceContext, InterfaceDeclarationSyntax>(this);
        _enums = new TypeContextCollection<TEnumContext, EnumDeclarationSyntax>(this);
        _delegates = new TypeContextCollection<TDelegateContext, DelegateDeclarationSyntax>(this);
    }

    public override TLanguageConversion Conversion => _Conversion;

    public override ICSharpTypeContextCollection<TClassContext, ClassDeclarationSyntax> Classes => _classes;

    public override ICSharpTypeContextCollection<TStructContext, StructDeclarationSyntax> Structs => _structs;

    public override ICSharpTypeContextCollection<TInterfaceContext, InterfaceDeclarationSyntax> Interfaces => _interfaces;

    public override ICSharpTypeContextCollection<TEnumContext, EnumDeclarationSyntax> Enums => _enums;

    public override ICSharpTypeContextCollection<TDelegateContext, DelegateDeclarationSyntax> Delegates => _delegates;

    protected internal override abstract TClassContext CreateContext(ClassDeclarationSyntax cls);

    protected internal override abstract TStructContext CreateContext(StructDeclarationSyntax str);

    protected internal override abstract TInterfaceContext CreateContext(InterfaceDeclarationSyntax iface);

    protected internal override abstract TEnumContext CreateContext(EnumDeclarationSyntax enm);

    protected internal override abstract TDelegateContext CreateContext(DelegateDeclarationSyntax dlg);

    internal override void addClass(CSharpClassTypeContext ctx)
    {
        _classes.Add((TClassContext)ctx);
    }

    internal override void addStruct(CSharpStructTypeContext ctx)
    {
        _structs.Add((TStructContext)ctx);
    }

    internal override void addInterface(CSharpInterfaceTypeContext ctx)
    {
        _interfaces.Add((TInterfaceContext)ctx);
    }

    internal override void addEnum(CSharpEnumTypeContext ctx)
    {
        _enums.Add((TEnumContext)ctx);
    }

    internal override void addDelegate(CSharpDelegateTypeContext ctx)
    {
        _delegates.Add((TDelegateContext)ctx);
    }
}

/// <summary>
/// CSharp compilation context with CSharp specific type context
/// </summary>
/// <remarks>Inherit this class if you will use base CSharp type contexts</remarks>
public abstract class CSharpCompilationContext<TLanguageConversion> : CSharpCompilationContext
    where TLanguageConversion : CSharpLanguageConversion
{
    TLanguageConversion _Conversion;
    TypeContextCollection<CSharpClassTypeContext, ClassDeclarationSyntax> _classes;
    TypeContextCollection<CSharpStructTypeContext, StructDeclarationSyntax> _structs;
    TypeContextCollection<CSharpInterfaceTypeContext, InterfaceDeclarationSyntax> _interfaces;
    TypeContextCollection<CSharpEnumTypeContext, EnumDeclarationSyntax> _enums;
    TypeContextCollection<CSharpDelegateTypeContext, DelegateDeclarationSyntax> _delegates;

    protected CSharpCompilationContext(TLanguageConversion conversion)
    {
        _Conversion = conversion;
        _classes = new TypeContextCollection<CSharpClassTypeContext, ClassDeclarationSyntax>(this);
        _structs = new TypeContextCollection<CSharpStructTypeContext, StructDeclarationSyntax>(this);
        _interfaces = new TypeContextCollection<CSharpInterfaceTypeContext, InterfaceDeclarationSyntax>(this);
        _enums = new TypeContextCollection<CSharpEnumTypeContext, EnumDeclarationSyntax>(this);
        _delegates = new TypeContextCollection<CSharpDelegateTypeContext, DelegateDeclarationSyntax>(this);
    }

    public override TLanguageConversion Conversion => _Conversion;

    public override ICSharpTypeContextCollection<CSharpClassTypeContext, ClassDeclarationSyntax> Classes => _classes;

    public override ICSharpTypeContextCollection<CSharpStructTypeContext, StructDeclarationSyntax> Structs => _structs;

    public override ICSharpTypeContextCollection<CSharpInterfaceTypeContext, InterfaceDeclarationSyntax> Interfaces => _interfaces;

    public override ICSharpTypeContextCollection<CSharpEnumTypeContext, EnumDeclarationSyntax> Enums => _enums;

    public override ICSharpTypeContextCollection<CSharpDelegateTypeContext, DelegateDeclarationSyntax> Delegates => _delegates;

    internal override void addClass(CSharpClassTypeContext ctx)
    {
        _classes.Add(ctx);
    }

    internal override void addStruct(CSharpStructTypeContext ctx)
    {
        _structs.Add(ctx);
    }

    internal override void addInterface(CSharpInterfaceTypeContext ctx)
    {
        _interfaces.Add(ctx);
    }

    internal override void addEnum(CSharpEnumTypeContext ctx)
    {
        _enums.Add(ctx);
    }

    internal override void addDelegate(CSharpDelegateTypeContext ctx)
    {
        _delegates.Add(ctx);
    }
}

/// <summary>
/// CSharp compilation context with CSharp specific type context
/// </summary>
/// <remarks>This class is infrastructural, you can't inherit it</remarks>
public abstract class CSharpCompilationContext : CSharpCompilationContextBase<CSharpMemberTypeContext>
{
    Dictionary<CSharpSyntaxNode, CSharpMemberTypeContext> _NodeMap;
    Dictionary<ITypeSymbol, List<CSharpMemberTypeContext>> _symbolMap;
    SymbolMapWrapper _SymbolMapWrapper;
    List<MethodDeclarationSyntax> _finalizers;

    internal CSharpCompilationContext()
    {
        _NodeMap = new Dictionary<CSharpSyntaxNode, CSharpMemberTypeContext>();
        _symbolMap = new Dictionary<ITypeSymbol, List<CSharpMemberTypeContext>>(SymbolEqualityComparer.Default);
        _SymbolMapWrapper = new SymbolMapWrapper(_symbolMap);
        _finalizers = new List<MethodDeclarationSyntax>();
    }

    protected internal override CSharpCollectionContext CreateCollectionContext()
    {
        return new CSharpCollectionContextImpl(this);
    }

    public bool IsCompilationDefined(ITypeSymbol symbol)
    {
        return _symbolMap.ContainsKey(symbol);
    }

    internal void AddClass(CSharpClassTypeContext ctx)
    {
        addToSymbolMap(ctx);
        addClass(ctx);
        _NodeMap.Add(ctx.Node, ctx);
    }

    internal void AddStruct(CSharpStructTypeContext ctx)
    {
        addToSymbolMap(ctx);
        addStruct(ctx);
        _NodeMap.Add(ctx.Node, ctx);
    }

    internal void AddInterface(CSharpInterfaceTypeContext ctx)
    {
        addToSymbolMap(ctx);
        addInterface(ctx);
        _NodeMap.Add(ctx.Node, ctx);
    }

    internal void AddEnum(CSharpEnumTypeContext ctx)
    {
        addToSymbolMap(ctx);
        addEnum(ctx);
        _NodeMap.Add(ctx.Node, ctx);
    }

    internal void AddDelegate(CSharpDelegateTypeContext ctx)
    {
        addToSymbolMap(ctx);
        addDelegate(ctx);
        _NodeMap.Add(ctx.Node, ctx);
    }

    internal void AddFinalizer(MethodDeclarationSyntax finalizer)
    {
        _finalizers.Add(finalizer);
    }

    protected internal virtual CSharpClassTypeContext CreateContext(ClassDeclarationSyntax cls)
    {
        return new CSharpClassTypeContextImpl(cls, this);
    }

    protected internal virtual CSharpStructTypeContext CreateContext(StructDeclarationSyntax str)
    {
        return new CSharpStructTypeContextImpl(str, this);
    }

    protected internal virtual CSharpInterfaceTypeContext CreateContext(InterfaceDeclarationSyntax iface)
    {
        return new CSharpInterfaceTypeContextImpl(iface, this);
    }
    protected internal virtual CSharpDelegateTypeContext CreateContext(DelegateDeclarationSyntax dlg)
    {
        return new CSharpDelegateTypeContextImpl(dlg, this);
    }

    protected internal virtual CSharpEnumTypeContext CreateContext(EnumDeclarationSyntax enm)
    {
        return new CSharpEnumTypeContextImpl(enm, this);
    }

    public IEnumerable<MethodDeclarationSyntax> Finalizers { get { return _finalizers; } }

    public abstract ICSharpTypeContextCollection<CSharpClassTypeContext, ClassDeclarationSyntax> Classes { get; }

    public abstract ICSharpTypeContextCollection<CSharpStructTypeContext, StructDeclarationSyntax> Structs { get; }

    public abstract ICSharpTypeContextCollection<CSharpInterfaceTypeContext, InterfaceDeclarationSyntax> Interfaces { get; }

    public abstract ICSharpTypeContextCollection<CSharpEnumTypeContext, EnumDeclarationSyntax> Enums { get; }

    public abstract ICSharpTypeContextCollection<CSharpDelegateTypeContext, DelegateDeclarationSyntax> Delegates { get; }

    /// <summary>
    /// Returns classes, structs, interfaces, enums and delegates
    /// </summary>
    public new ICSharpTypeContextEnumerable<CSharpMemberTypeContext, MemberDeclarationSyntax> Types
    {
        get
        {
            return new TypeContextEnumerable<CSharpMemberTypeContext, MemberDeclarationSyntax>(
                (Classes.AllContexts as IEnumerable<CSharpMemberTypeContext>)
                    .Concat(Structs.AllContexts).Concat(Interfaces.AllContexts)
                    .Concat(Enums.AllContexts).Concat(Delegates.AllContexts));
        }
    }

    /// <summary>
    /// Returns classes, structs, interfaces, enums
    /// </summary>
    public ICSharpTypeContextEnumerable<CSharpBaseTypeContext, BaseTypeDeclarationSyntax> NestingTypes
    {
        get
        {
            return new TypeContextEnumerable<CSharpBaseTypeContext, BaseTypeDeclarationSyntax>(
                (Classes.AllContexts as IEnumerable<CSharpBaseTypeContext>)
                    .Concat(Structs.AllContexts).Concat(Interfaces.AllContexts).Concat(Enums.AllContexts));
        }
    }

    /// <summary>
    /// Returns classes, structs and interfaces
    /// </summary>
    public ICSharpTypeContextEnumerable<CSharpTypeContext, TypeDeclarationSyntax> CallableTypes
    {
        get
        {
            return new TypeContextEnumerable<CSharpTypeContext, TypeDeclarationSyntax>(
                (Classes.AllContexts as IEnumerable<CSharpTypeContext>)
                    .Concat(Structs.AllContexts).Concat(Interfaces.AllContexts));
        }
    }

    /// <summary>
    /// Returns classes and structs
    /// </summary>
    public ICSharpTypeContextEnumerable<CSharpTypeContext, TypeDeclarationSyntax> StorageTypes
    {
        get
        {
            return new TypeContextEnumerable<CSharpTypeContext, TypeDeclarationSyntax>(
                (Classes.AllContexts as IEnumerable<CSharpTypeContext>).Concat(Structs.AllContexts));
        }
    }

    public override abstract CSharpLanguageConversion Conversion { get; }

    public IReadOnlyDictionary<CSharpSyntaxNode, CSharpMemberTypeContext> NodeMap => _NodeMap;

    public IReadOnlyDictionary<ITypeSymbol, IReadOnlyList<CSharpMemberTypeContext>> SymbolMap => _SymbolMapWrapper;

    internal abstract void addClass(CSharpClassTypeContext ctx);

    internal abstract void addStruct(CSharpStructTypeContext ctx);

    internal abstract void addInterface(CSharpInterfaceTypeContext ctx);

    internal abstract void addEnum(CSharpEnumTypeContext ctx);

    internal abstract void addDelegate(CSharpDelegateTypeContext ctx);

    void addToSymbolMap(CSharpMemberTypeContext ctx)
    {
        if (!_symbolMap.TryGetValue(ctx.Symbol, out var types))
        {
            types = new List<CSharpMemberTypeContext>();
            _symbolMap[ctx.Symbol] = types;
        }

        types.Add(ctx);
    }

    internal class TypeContextEnumerable<TTypeContext, TSyntaxNode> : ICSharpTypeContextEnumerable<TTypeContext, TSyntaxNode>
        where TTypeContext : CSharpMemberTypeContext
        where TSyntaxNode : MemberDeclarationSyntax
    {
        IEnumerable<TTypeContext> _types;

        public TypeContextEnumerable(IEnumerable<TTypeContext> types)
        {
            _types = types;
        }

        public IEnumerable<TSyntaxNode> AllDeclarations
        {
            get
            {
                foreach (var type in _types)
                    yield return (TSyntaxNode)type.Node;
            }
        }

        public IEnumerable<TSyntaxNode> Declarations
        {
            get
            {
                foreach (var type in _types)
                {
                    if (type.IsMainDeclaration)
                        yield return (TSyntaxNode)type.Node;
                }
            }
        }

        public IEnumerable<TTypeContext> AllContexts => _types;

        public IEnumerator<TTypeContext> GetEnumerator()
        {
            foreach (var type in _types)
            {
                if (type.IsMainDeclaration)
                    yield return type;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class TypeContextCollection<TTypeContext, TSyntaxNode> : ICSharpTypeContextCollection<TTypeContext, TSyntaxNode>
        where TTypeContext : CSharpMemberTypeContext
        where TSyntaxNode : MemberDeclarationSyntax
    {
        CSharpCompilationContext _compilation;
        List<TTypeContext> _types;

        internal TypeContextCollection(CSharpCompilationContext ctx)
        {
            _compilation = ctx;
            _types = new List<TTypeContext>();
        }

        public void Add(TTypeContext ctx)
        {
            _types.Add(ctx);
        }

        public TTypeContext this[TSyntaxNode node] => (TTypeContext)_compilation.NodeMap[node];

        public bool Contains(TSyntaxNode node)
        {
            return _compilation.NodeMap.ContainsKey(node);
        }

        public IReadOnlyList<TTypeContext> AllContexts => _types;

        IEnumerable<TTypeContext> ICSharpTypeContextEnumerable<TTypeContext, TSyntaxNode>.AllContexts => _types;

        public IEnumerable<TSyntaxNode> AllDeclarations
        {
            get
            {
                foreach (var ctx in _types)
                    yield return (TSyntaxNode)ctx.Node;
            }
        }

        public IEnumerable<TSyntaxNode> Declarations
        {
            get
            {
                foreach (var ctx in _types)
                {
                    if (ctx.IsMainDeclaration)
                        yield return (TSyntaxNode)ctx.Node;
                }
            }
        }

        public IEnumerator<TTypeContext> GetEnumerator()
        {
            foreach (var ctx in _types)
            {
                if (ctx.IsMainDeclaration)
                    yield return ctx;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _types.GetEnumerator();
        }
    }

    class SymbolMapWrapper : IReadOnlyDictionary<ITypeSymbol, IReadOnlyList<CSharpMemberTypeContext>>
    {
        Dictionary<ITypeSymbol, List<CSharpMemberTypeContext>> _symbolMap;

        public SymbolMapWrapper(Dictionary<ITypeSymbol, List<CSharpMemberTypeContext>> symbolMap)
        {
            _symbolMap = symbolMap;
        }

        public IReadOnlyList<CSharpMemberTypeContext> this[ITypeSymbol key] => _symbolMap[key];

        public IEnumerable<ITypeSymbol> Keys => _symbolMap.Keys;

        public IEnumerable<IReadOnlyList<CSharpMemberTypeContext>> Values
        {
            get
            {
                foreach (var value in _symbolMap.Values)
                    yield return value;
            }
        }

        public int Count => _symbolMap.Count;

        public bool ContainsKey(ITypeSymbol key)
        {
            return _symbolMap.ContainsKey(key);
        }

        public bool TryGetValue(ITypeSymbol key, [MaybeNullWhen(false)] out IReadOnlyList<CSharpMemberTypeContext> value)
        {
            List<CSharpMemberTypeContext>? list;
            if (!_symbolMap.TryGetValue(key, out list))
            {
                value = null;
                return false;
            }

            value = list;
            return true;
        }

        public IEnumerator<KeyValuePair<ITypeSymbol, IReadOnlyList<CSharpMemberTypeContext>>> GetEnumerator()
        {
            foreach (var pair in _symbolMap)
                yield return KeyValuePair.Create(pair.Key, (IReadOnlyList<CSharpMemberTypeContext>)pair.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

/// <summary>
/// CSharp compilation context
/// </summary>
/// <remarks>Inherit this class if you will use custom type contexts</remarks>
public abstract class CSharpCompilationContextBase<TTypeContext, TLanguageConversion> : CompilationContext<TTypeContext, TLanguageConversion, CSharpNodeVisitor>
    where TTypeContext : TypeContext<TTypeContext>
    where TLanguageConversion : LanguageConversion
{
    protected CSharpCompilationContextBase(TLanguageConversion conversion)
        : base(conversion) { }

    internal protected abstract override CSharpCollectionContextBase CreateCollectionContext();
}

/// <summary>
/// CSharp compilation context
/// </summary>
/// <remarks>This class is infrastructural, you can't inherit it</remarks>
public abstract class CSharpCompilationContextBase<TTypeContext> : CompilationContext<TTypeContext, CSharpNodeVisitor>
    where TTypeContext : TypeContext<TTypeContext>
{
    internal CSharpCompilationContextBase() { }

    internal protected abstract override CSharpCollectionContextBase CreateCollectionContext();
}

/// <summary>
/// Collection of type contexts iterable on main declarations that can be looked up on nodes
/// </summary>
public interface ICSharpTypeContextCollection<out TTypeContext, TSyntaxNode> : ICSharpTypeContextEnumerable<TTypeContext, TSyntaxNode>
    where TTypeContext : CSharpMemberTypeContext
    where TSyntaxNode : MemberDeclarationSyntax
{
    bool Contains(TSyntaxNode node);

    TTypeContext this[TSyntaxNode node] { get; }

    /// <summary>
    /// Returns all partial declaration contexts
    /// </summary>
    new IReadOnlyList<TTypeContext> AllContexts { get; }
}

/// <summary>
/// Collection of type contexts iterable on main declarations
/// </summary>
public interface ICSharpTypeContextEnumerable<out TTypeContext, TSyntaxNode> : IEnumerable<TTypeContext>
    where TTypeContext : CSharpMemberTypeContext
    where TSyntaxNode : MemberDeclarationSyntax
{
    /// <summary>
    ///  Return all partial declaration syntax nodes
    /// </summary>
    IEnumerable<TSyntaxNode> AllDeclarations { get; }

    /// <summary>
    /// Returns main declaration syntax nodes
    /// </summary>
    IEnumerable<TSyntaxNode> Declarations { get; }

    /// <summary>
    /// Returns all partial declaration contexts
    /// </summary>
    IEnumerable<TTypeContext> AllContexts { get; }
}

sealed class CSharpCompilationContextImpl : CSharpCompilationContext<CSharpLanguageConversion>
{
    public CSharpCompilationContextImpl(CSharpLanguageConversion conversion)
        : base(conversion)
    {
    }
}
