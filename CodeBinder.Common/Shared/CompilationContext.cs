using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    // TODO: Make it language agnostic???
    public abstract class CompilationContext : ICompilationContextProvider
    {
        private Dictionary<SyntaxTree, SemanticModel> _modelCache;
        public Compilation Compilation { get; internal set; }

        internal CompilationContext()
        {
            _modelCache = new Dictionary<SyntaxTree, SemanticModel>();
        }

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            SemanticModel model;
            if (!_modelCache.TryGetValue(tree, out model))
            {
                model = Compilation.GetSemanticModel(tree, true);
                _modelCache.Add(tree, model);
            }

            return model;
        }

        internal abstract SyntaxTreeContext CreateSyntaxTreeContext();

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

        protected void AddType(TTypeContext type, TTypeContext parent)
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

            protected void AddType(TTypeContext type, TTypeContext parent)
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
            where TCompilationContext :CompilationContext<TTypeContext>
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

    public abstract class CompilationContext<TSyntaxTreeContext, TTypeContext, TLanguageConversion> : CompilationContext<TTypeContext>
        where TSyntaxTreeContext : CompilationContext<TTypeContext>.SyntaxTree
        where TTypeContext : TypeContext<TTypeContext>
        where TLanguageConversion : LanguageConversion
    {
        public TLanguageConversion Conversion { get; private set; }

        protected CompilationContext(TLanguageConversion conversion)
        {
            Conversion = conversion;
        }

        internal sealed override SyntaxTreeContext CreateSyntaxTreeContext()
        {
            return createSyntaxTreeContext();
        }

        protected abstract TSyntaxTreeContext createSyntaxTreeContext();
    }
}
