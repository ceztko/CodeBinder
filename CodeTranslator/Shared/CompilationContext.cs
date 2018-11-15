using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    // TODO: Make it language agnostic
    public class CompilationContext : ICompilationContextProvider
    {
        List<TypeContext> _rootTypes;
        private Dictionary<SyntaxTree, SemanticModel> _modelCache;
        public Compilation Compilation { get; private set; }

        public CompilationContext(Compilation compilation)
        {
            _modelCache = new Dictionary<SyntaxTree, SemanticModel>();
            Compilation = compilation;
            _rootTypes = new List<TypeContext>();
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

        internal void AddRootType(TypeContext type)
        {
            _rootTypes.Add(type);
        }

        public IEnumerable<TypeContext> RootTypes
        {
            get { return _rootTypes; }
        }

        CompilationContext ICompilationContextProvider.Compilation
        {
            get { return this; }
        }
    }
}
