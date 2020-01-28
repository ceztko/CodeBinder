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
    public abstract class CompilationContext : ICompilationContextProvider
    {
        private Dictionary<SyntaxTree, SemanticModel> _modelCache;
        private Compilation _compilation = null!;

        internal CompilationContext()
        {
            _modelCache = new Dictionary<SyntaxTree, SemanticModel>();
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

        public Compilation Compilation
        {
            get { return _compilation; }
            internal set
            {
                _compilation = value;
                CompilationSet?.Invoke(this, EventArgs.Empty);
            }
        }

        internal abstract IEnumerable<SyntaxTreeContext> Visit(IEnumerable<SyntaxTree> syntaxTrees);

        protected abstract IEnumerable<TypeContext> GetRootTypes();

        internal IEnumerable<ConversionDelegate> DefaultConversionDelegates
        {
            get
            {
                foreach (var conversion in DefaultConversions)
                    yield return new ConversionDelegate(conversion);
            }
        }

        // Compilation wide default converions, see CLangCompilationContext for some examples
        public virtual IEnumerable<ConversionBuilder> DefaultConversions
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

    public abstract class CompilationContext<TTypeContext> : CompilationContext
        where TTypeContext : TypeContext<TTypeContext>
    {
        List<TTypeContext> _rootTypes;

        internal CompilationContext()
        {
            _rootTypes = new List<TTypeContext>();
        }

        protected void AddType(TTypeContext type, TTypeContext? parent)
        {
            if (type.Parent != null)
                throw new Exception("Can't re-add root type");

            if (type == parent)
                throw new Exception("The parent can't be same reference as the given type");

            if (parent == null)
            {
                _rootTypes.Add(type);
            }
            else
            {
                type.Parent = parent;
                parent.AddChild(type);
            }
        }

        protected override IEnumerable<TypeContext> GetRootTypes()
        {
            return _rootTypes;
        }

        public new IEnumerable<TTypeContext> RootTypes
        {
            get { return _rootTypes; }
        }

        #region SyntaxTreeContext

        public abstract class SyntaxTree : SyntaxTreeContext
        {
            List<TTypeContext> _rootTypes;

            internal SyntaxTree()
            {
                _rootTypes = new List<TTypeContext>();
            }

            protected void AddType(TTypeContext type, TTypeContext? parent)
            {
                if (type.Parent != null)
                    throw new Exception("Can't re-add root type");

                if (type == parent)
                    throw new Exception("The parent can't be same reference as the given type");

                if (parent == null)
                {
                    _rootTypes.Add(type);
                }
                else
                {
                    type.Parent = parent;
                    parent.AddChild(type);
                }
            }

            public new IEnumerable<TypeContext> RootTypes
            {
                get { return _rootTypes; }
            }

            protected override IEnumerable<TypeContext> GetRootTypes()
            {
                return RootTypes;
            }
        }

        public abstract class SyntaxTree<TCompilationContext> : SyntaxTree
            where TCompilationContext : CompilationContext<TTypeContext>
        {
            public new TCompilationContext Compilation { get; private set; }

            protected SyntaxTree(TCompilationContext compilation)
            {
                Compilation = compilation;
            }

            protected sealed override CompilationContext GetCompilationContext()
            {
                return Compilation;
            }
        }

        #endregion
    }

    public abstract class CompilationContext<TTypeContext, TSyntaxTreeContext, TNodeVisitor, TLanguageConversion> : CompilationContext<TTypeContext>
        where TTypeContext : TypeContext<TTypeContext>
        where TSyntaxTreeContext : CompilationContext<TTypeContext>.SyntaxTree
        where TNodeVisitor : class, INodeVisitor<TSyntaxTreeContext>, new()
        where TLanguageConversion : LanguageConversion
    {
        public TLanguageConversion Conversion { get; private set; }

        internal override IEnumerable<SyntaxTreeContext> Visit(IEnumerable<Microsoft.CodeAnalysis.SyntaxTree> syntaxTrees)
        {
            // Visit trees and create contexts
            var visitor = new TNodeVisitor();
            foreach (var tree in syntaxTrees)
            {
                var context = createSyntaxTreeContext();
                context.SyntaxTree = tree;
                visitor.Visit(context);
                yield return context;
            }
        }

        protected CompilationContext(TLanguageConversion conversion)
        {
            Conversion = conversion;
        }

        protected abstract TSyntaxTreeContext createSyntaxTreeContext();
    }
}
