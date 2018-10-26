// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    static class RoslynExtensions
    {
        public static string GetFullMetadataName(this SyntaxNode node, SyntaxTreeContext treeContext)
        {
            var info = node.GetTypeInfo(treeContext);
            return info.GetFullMetadataName();
        }

        public static TypeInfo GetTypeInfo(this SyntaxNode node, SyntaxTreeContext treeContext)
        {
            var model = node.GetSemanticModel(treeContext);
            return model.GetTypeInfo(node);
        }

        public static SemanticModel GetSemanticModel(this SyntaxNode node, SyntaxTreeContext treeContext)
        {
            return treeContext.GetSemanticModel(node.SyntaxTree);
        }

        public static string GetFullMetadataName(ref this TypeInfo info)
        {
            return info.ConvertedType.GetFullMetadataName();
        }

        // Reference: https://stackoverflow.com/a/27106959/213871
        // Also look for support in Roslyn https://github.com/dotnet/roslyn/issues/1891
        public static string GetFullMetadataName(this ISymbol symbol)
        {
            if (symbol == null || IsRootNamespace(symbol))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(symbol.MetadataName);
            var last = symbol;

            symbol = symbol.ContainingSymbol;

            while (!IsRootNamespace(symbol))
            {
                if (symbol is ITypeSymbol && last is ITypeSymbol)
                {
                    sb.Insert(0, '+');
                }
                else
                {
                    sb.Insert(0, '.');
                }

                sb.Insert(0, symbol.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                //sb.Insert(0, s.MetadataName);
                symbol = symbol.ContainingSymbol;
            }

            return sb.ToString();
        }

        private static bool IsRootNamespace(ISymbol symbol)
        {
            INamespaceSymbol s = null;
            return ((s = symbol as INamespaceSymbol) != null) && s.IsGlobalNamespace;
        }
    }
}
