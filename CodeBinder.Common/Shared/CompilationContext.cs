// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
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
        protected CompilationContext() { }

        public new TLanguageConversion Conversion => getLanguageConversion();

        protected sealed override LanguageConversion GetLanguageConversion() => getLanguageConversion();

        protected abstract TLanguageConversion getLanguageConversion();
    }

    public abstract class CompilationContext<TTypeContext> : CompilationContext
        where TTypeContext : TypeContext<TTypeContext>
    {
        HashSet<TTypeContext> _types;

        internal CompilationContext()
        {
            _types = new HashSet<TTypeContext>();
        }

        protected internal void AddType(TTypeContext type, TTypeContext? parent)
        {
            if (type.Parent != null)
                throw new Exception("Can't re-add root type");

            if (type == parent)
                throw new Exception("The parent can't be same reference as the given type");

            if (!_types.Add(type))
                throw new Exception("Can't reinsert the same type");

            if (parent != null)
            {
                type.Parent = parent;
                parent.AddChild(type);
            }
        }

        protected override IEnumerable<TypeContext> GetRootTypes()
        {
            return RootTypes;
        }

        public new IEnumerable<TTypeContext> RootTypes
        {
            get
            {
                foreach (var type in _types)
                {
                    if (type.Parent == null)
                        yield return type;
                }
            }
        }
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

        public LanguageConversion Conversion => GetLanguageConversion();

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

        protected abstract LanguageConversion GetLanguageConversion();

        protected abstract IEnumerable<TypeContext> GetRootTypes();

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

        public IEnumerable<TypeContext> RootTypes
        {
            get { return GetRootTypes(); }
        }

        CompilationContext ICompilationContextProvider.Compilation
        {
            get { return this; }
        }
    }
}
