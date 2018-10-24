// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    // NOTE: Don't add overloadings to access the cache here:
    // add them as extension methods
    public class RoslynSyntaxTreeContext : SyntaxTreeContext
    {
        private Dictionary<ISymbol, string> _fullMetadataNameCache;
        private Dictionary<SyntaxNode, TypeInfo> _typeCache;

        public RoslynSyntaxTreeContext()
        {
            _fullMetadataNameCache = new Dictionary<ISymbol, string>();
            _typeCache = new Dictionary<SyntaxNode, TypeInfo>();
        }

        public string GetFullMetadataName(ISymbol symbol)
        {
            string fullName;
            if (!_fullMetadataNameCache.TryGetValue(symbol, out fullName))
            {
                fullName = symbol.GetFullMetadataName();
                _fullMetadataNameCache.Add(symbol, fullName);
            }

            return fullName;
        }

        public TypeInfo GetTypeInfo(SyntaxNode node)
        {
            TypeInfo info;
            if (!_typeCache.TryGetValue(node, out info))
            {
                var model = node.GetSemanticModel(this);
                info = model.GetTypeInfo(node);
                _typeCache.Add(node, info);
            }

            return info;
        }
    }
}
