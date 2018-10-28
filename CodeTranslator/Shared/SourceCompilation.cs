using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    // TODO: Make it language agnostic
    public class SourceCompilation : ISemanticModelProvider
    {
        private Dictionary<SyntaxTree, SemanticModel> _modelCache;
        public Compilation Compilation { get; private set; }

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

        public SourceCompilation(Compilation compilation)
        {
            _modelCache = new Dictionary<SyntaxTree, SemanticModel>();
            Compilation = compilation;
        }
    }
}
