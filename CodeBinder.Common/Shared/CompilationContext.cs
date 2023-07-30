// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;

namespace CodeBinder.Shared;

/// <summary>
/// Context built around a CodeAnalysis.Compilation
/// </summary>
/// <remarks>Inherit this class to add contextualized info to CompilationContext</remarks>
public abstract class CompilationContext<TTypeContext, TLanguageConversion, TVisitor> : CompilationContext<TTypeContext, TVisitor>
    where TTypeContext : TypeContext<TTypeContext>
    where TLanguageConversion : LanguageConversion
    where TVisitor : NodeVisitor
{
    TLanguageConversion _Conversion;

    protected CompilationContext(TLanguageConversion conversion)
    {
        _Conversion = conversion;
    }

    public override TLanguageConversion Conversion => _Conversion;
}

/// <summary>
/// Context built around a CodeAnalysis.Compilation
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public abstract class CompilationContext<TTypeContext, TVisitor> : CompilationContext
    where TTypeContext : TypeContext<TTypeContext>
    where TVisitor : NodeVisitor
{
    List<TTypeContext> _Types;

    internal CompilationContext()
    {
        _Types = new List<TTypeContext>();
    }

    internal protected abstract override CollectionContext<TVisitor> CreateCollectionContext();

    protected internal void AddTypeContext(TTypeContext type, TTypeContext? parent)
    {
        if (type.Parent != null)
            throw new Exception("Can't re-add root type");

        if (type == parent)
            throw new Exception("The parent can't be same reference as the given type");

        if (parent != null)
        {
            type.Parent = parent;
            parent.AddChild(type);
        }

        addTypeContext(type);
    }

    void addTypeContext(TTypeContext type)
    {
        _Types.Add(type);
    }

    public override sealed IEnumerable<TTypeContext> Types => _Types;
}

/// <summary>
/// Context built around a CodeAnalysis.Compilation
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public abstract class CompilationContext : CompilationProvider
{
    HashSet<string> _Namespaces;
    Dictionary<ISymbol, BindedName> _bindedMethodNames;

    internal CompilationContext()
    {
        _Namespaces = new HashSet<string>();
        _bindedMethodNames = new Dictionary<ISymbol, BindedName>(SymbolEqualityComparer.Default);
    }

    public bool TryGetBindedName(ISymbol symbol, out BindedName name)
    {
        if (_bindedMethodNames.TryGetValue(symbol, out name))
            return true;

        return false;
    }

    internal void AddBinding(ISymbol symbol, string bindedName, bool isOverload)
    {
        _bindedMethodNames.Add(symbol, new BindedName() { Name = bindedName, IsOverload = isOverload });
    }

    internal protected abstract CollectionContext CreateCollectionContext();

    protected internal void AddNamespace(string ns)
    {
        _Namespaces.Add(ns);
    }

    public abstract LanguageConversion Conversion { get; }

    public IReadOnlyCollection<string> Namespaces => _Namespaces;

    internal IEnumerable<ConversionDelegate> ConversionDelegates
    {
        get
        {
            foreach (var conversion in DefaultConversions)
                yield return new ConversionDelegate(conversion);
        }
    }

    // Compilation wide default converions, see CLangCompilationContext for some examples
    public virtual IEnumerable<IConversionWriter> DefaultConversions
    {
        get { yield break; }
    }

    public abstract IEnumerable<TypeContext> Types { get; }

    public IEnumerable<TypeContext> RootTypes
    {
        get
        {
            foreach (var type in Types)
            {
                if (type.Parent == null)
                    yield return type;
            }
        }
    }
}

/// <summary>
/// Context built around a CodeAnalysis.Compilation
/// </summary>
/// <remarks>This class is for infrastructure only</remarks>
public class CompilationProvider : ICompilationProvider
{
    private Dictionary<SyntaxTree, SemanticModel> _modelCache;
    private Compilation _Compilation = null!;
    public string LibraryName { get; private set; } = null!;

    internal CompilationProvider(Compilation compilation)
    {
        _modelCache = new Dictionary<SyntaxTree, SemanticModel>();
        Compilation = compilation;
    }

    internal CompilationProvider()
    {
        _modelCache = new Dictionary<SyntaxTree, SemanticModel>();
    }

    public SemanticModel GetSemanticModel(SyntaxTree tree)
    {
        SemanticModel? model;
        if (!_modelCache.TryGetValue(tree, out model))
        {
            model = Compilation.GetSemanticModel(tree, true);
            _modelCache.Add(tree, model);
        }

        return model;
    }

    /// The Microsoft.CodeAnalysis compilation
    /// <remarks>The compilation instance is not available during construction</remarks>
    public Compilation Compilation
    {
        get { return _Compilation; }
        internal set
        {
            _Compilation = value;
            try
            {
                LibraryName = value.Assembly.GetAttribute<NativeLibraryAttribute>().GetConstructorArgument<string>(0);
            }
            catch
            {
                throw new Exception($"Missing {nameof(NativeLibraryAttribute)}");
            }
        }
    }

    CompilationProvider ICompilationProvider.Compilation
    {
        get { return this; }
    }
}

public struct BindedName
{
    public string Name;
    public bool IsOverload;
}
