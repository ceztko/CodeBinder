// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace CodeTranslator.Shared
{
    static class RoslynMethodExtensions
    {
        private static readonly SymbolDisplayFormat Format = new SymbolDisplayFormat(
            SymbolDisplayGlobalNamespaceStyle.Omitted,
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable);

        public static bool IsAttribute<TAttribute>(this AttributeData attribute)
            where TAttribute : Attribute
        {
            return attribute.AttributeClass.GetFullName() == typeof(TAttribute).FullName;
        }

        public static string GetFullName(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var symbol = node.GetTypeSymbol(provider);
            return symbol.GetFullName();
        }

        public static TypeInfo GetTypeInfo(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = node.GetSemanticModel(provider);
            return model.GetTypeInfo(node);
        }

        public static bool HasAttribute<TAttribute>(this SyntaxNode node, ICompilationContextProvider provider)
            where TAttribute : Attribute
        {
            var symbol = node.GetDeclaredSymbol(provider);
            if (symbol == null)
                return false;

            var attributes = symbol.GetAttributes();
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<TAttribute>())
                    return true;
            }
            return false;
        }

        public static ImmutableArray<AttributeData> GetAttributes(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var symbol = node.GetDeclaredSymbol(provider);
            return symbol.GetAttributes();
        }

        public static ISymbol GetDeclaredSymbol(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = node.GetSemanticModel(provider);
            return model.GetDeclaredSymbol(node);
        }

        public static ITypeSymbol GetTypeSymbol(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var info = node.GetTypeInfo(provider);
            return info.ConvertedType;
        }

        public static SemanticModel GetSemanticModel(this SyntaxNode node, ICompilationContextProvider provider)
        {
            return provider.GetSemanticModel(node.SyntaxTree);
        }

        public static object GetValue(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = provider.GetSemanticModel(node.SyntaxTree);
            return model.GetConstantValue(node).Value;
        }

        public static T GetValue<T>(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = provider.GetSemanticModel(node.SyntaxTree);
            return (T)model.GetConstantValue(node).Value;
        }

        // Reference https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/SymbolDisplay/SymbolDisplayFormat.cs
        static readonly SymbolDisplayFormat DisplayFormat = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
            localOptions: SymbolDisplayLocalOptions.IncludeType,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
            memberOptions:
                SymbolDisplayMemberOptions.IncludeParameters |
                SymbolDisplayMemberOptions.IncludeContainingType |
                SymbolDisplayMemberOptions.IncludeType |
                SymbolDisplayMemberOptions.IncludeRef |
                SymbolDisplayMemberOptions.IncludeExplicitInterface,
            kindOptions:
                SymbolDisplayKindOptions.IncludeMemberKeyword,
            parameterOptions:
                SymbolDisplayParameterOptions.IncludeOptionalBrackets |
                SymbolDisplayParameterOptions.IncludeDefaultValue |
                SymbolDisplayParameterOptions.IncludeParamsRefOut |
                SymbolDisplayParameterOptions.IncludeExtensionThis |
                SymbolDisplayParameterOptions.IncludeType |
                SymbolDisplayParameterOptions.IncludeName,
            miscellaneousOptions:
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName |
                SymbolDisplayMiscellaneousOptions.ExpandNullable
        );

        // Other implementations:
        // * https://github.com/GuOrg/Gu.Roslyn.Extensions/blob/master/Gu.Roslyn.AnalyzerExtensions/Symbols/INamedTypeSymbolExtensions.cs
        // * https://stackoverflow.com/a/27106959/213871
        // Reference: https://github.com/dotnet/roslyn/issues/1891
        public static string GetFullName(this ISymbol symbol)
        {
            var test = symbol.GetAttributes();
            return SymbolDisplay.ToDisplayString(symbol, DisplayFormat);
        }
    }
}
