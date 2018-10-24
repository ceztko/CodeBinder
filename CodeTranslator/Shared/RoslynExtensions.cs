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
        public static SemanticModel GetSemanticModel(this SyntaxNode node, RoslynSyntaxTreeContext treeContext)
        {
            return treeContext.GetSemanticModel(node.SyntaxTree);
        }

        public static string GetFullMetadataName(this SyntaxNode node, RoslynSyntaxTreeContext treeContext)
        {
            var info = treeContext.GetTypeInfo(node);
            return info.GetFullMetadataName(treeContext);
        }

        public static string GetFullMetadataName(ref this TypeInfo info, RoslynSyntaxTreeContext treeContext)
        {
            return treeContext.GetFullMetadataName(info.ConvertedType);
        }

        public static string GetFullMetadataName(ref this TypeInfo info)
        {
            return info.ConvertedType.GetFullMetadataName();
        }

        public static string GetFullMetadataName(this ISymbol symbol, RoslynSyntaxTreeContext treeContext)
        {
            return treeContext.GetFullMetadataName(symbol);
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
