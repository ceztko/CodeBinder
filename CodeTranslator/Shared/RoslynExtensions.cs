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
        public static string GetFullMetadataName(this SyntaxNode node, ISemanticModelProvider provider)
        {
            var info = node.GetTypeInfo(provider);
            return info.GetFullMetadataName();
        }

        public static TypeInfo GetTypeInfo(this SyntaxNode node, ISemanticModelProvider provider)
        {
            var model = node.GetSemanticModel(provider);
            return model.GetTypeInfo(node);
        }

        public static SemanticModel GetSemanticModel(this SyntaxNode node, ISemanticModelProvider provider)
        {
            return provider.GetSemanticModel(node.SyntaxTree);
        }

        public static object GetValue(this SyntaxNode node, ISemanticModelProvider provider)
        {
            var model = provider.GetSemanticModel(node.SyntaxTree);
            return model.GetConstantValue(node).Value;
        }

        public static T GetValue<T>(this SyntaxNode node, ISemanticModelProvider provider)
        {
            var model = provider.GetSemanticModel(node.SyntaxTree);
            return (T)model.GetConstantValue(node).Value;
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
