// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using CodeBinder.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeBinder.Shared
{
    /// <summary>
    /// Context built around a CodeAnalysis.Compilation
    /// </summary>
    /// <remarks>Inherit this class to add contextualized info to CompilationContext</remarks>
    public abstract class CompilationContext<TTypeContext, TLanguageConversion> : CompilationContext<TTypeContext>
        where TTypeContext : TypeContext<TTypeContext>
        where TLanguageConversion : LanguageConversion
    {
        List<TTypeContext> _Types;
        TLanguageConversion _Conversion;

        protected CompilationContext(TLanguageConversion conversion)
        {
            _Conversion = conversion;
            _Types = new List<TTypeContext>();
        }

        public override TLanguageConversion Conversion => _Conversion;

        internal override void addTypeContext(TTypeContext type)
        {
            _Types.Add(type);
        }

        protected override sealed IEnumerable<TTypeContext> GetTypes() => _Types;
    }

    public abstract class CompilationContext<TTypeContext> : CompilationContext
        where TTypeContext : TypeContext<TTypeContext>
    {
        Dictionary<ISymbol, string> _bindedMethodNames;

        internal CompilationContext()
        {
            _bindedMethodNames = new Dictionary<ISymbol, string>(SymbolEqualityComparer.Default);
        }

        public bool TryGetBindedName(IMethodSymbol symbol, [NotNullWhen(true)] out string? name)
        {
            if (_bindedMethodNames.TryGetValue(symbol, out name))
                return true;

            return false;
        }

        internal void AddMethodBinding(IMethodSymbol symbol, string bindedName)
        {
            _bindedMethodNames.Add(symbol, bindedName);
        }

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

        internal abstract void addTypeContext(TTypeContext type);
    }

    public abstract class CompilationContext : ICompilationContextProvider
    {
        HashSet<string> _Namespaces;
        private Dictionary<SyntaxTree, SemanticModel> _modelCache;
        private Compilation _compilation = null!;

        public string LibraryName { get; private set; } = string.Empty;

        internal CompilationContext()
        {
            _modelCache = new Dictionary<SyntaxTree, SemanticModel>();
            _Namespaces = new HashSet<string>();
        }

        public event EventHandler? CompilationSet;

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

        protected internal void AddNamespace(string ns)
        {
            _Namespaces.Add(ns);
        }

        public abstract LanguageConversion Conversion { get; }

        public IReadOnlyCollection<string> Namespaces => _Namespaces;

        public Compilation Compilation
        {
            get { return _compilation; }
            internal set
            {
                _compilation = value;
                try
                {
                    LibraryName = _compilation.Assembly.GetAttribute<NativeLibraryAttribute>().GetConstructorArgument<string>(0);
                }
                catch
                {
                    throw new Exception($"Missing {nameof(NativeLibraryAttribute)}");
                }

                CompilationSet?.Invoke(this, EventArgs.Empty);
            }
        }

        internal protected abstract INodeVisitor CreateVisitor();

        protected abstract IEnumerable<TypeContext> GetTypes();

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

        public IEnumerable<TypeContext> Types => GetTypes();

        public IEnumerable<TypeContext> RootTypes
        {
            get
            {
                foreach (var type in GetTypes())
                {
                    if (type.Parent == null)
                        yield return type;
                }
            }
        }

        CompilationContext ICompilationContextProvider.Compilation
        {
            get { return this; }
        }
    }
}
