// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator
{
    // NOTE: Don't add overloadings to access the cache here:
    // add them as extension methods
    public class SyntaxTreeContext : ISemanticModelProvider
    {
        private Dictionary<SyntaxTree, SemanticModel> _modelCache;
        public Compilation Compilation { get; internal set; }

        public SyntaxTreeContext()
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
    }
}
