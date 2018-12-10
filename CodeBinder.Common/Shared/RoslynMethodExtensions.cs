// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace CodeBinder.Shared
{
    public static class RoslynMethodExtensions
    {
        /// <summary>Primitive types as defined by https://docs.microsoft.com/en-us/dotnet/api/system.type.isprimitive
        /// </summary>
        /// <returns>Return true if the given symbol is a blittable non-structured system type</returns>
        public static bool IsCLRPrimitiveType(this ITypeSymbol symbol)
        {
            switch (symbol.SpecialType)
            {
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNullable(this ITypeSymbol symbol)
        {
            return symbol.SpecialType == SpecialType.System_Nullable_T;
        }

        public static bool IsGeneric(this ITypeSymbol symbol)
        {
            var named = symbol as INamedTypeSymbol;
            return named?.IsGenericType == true;
        }

        public static bool ShouldDiscard(this IMethodSymbol method)
        {
            // TODO: More support for RequiresAttribute
            return method.HasAttribute<IgnoreAttribute>() || method.HasAttribute<RequiresAttribute>();
        }

        public static bool IsAttribute<TAttribute>(this AttributeData attribute)
            where TAttribute : Attribute
        {
            return attribute.AttributeClass.GetFullName() == typeof(TAttribute).FullName;
        }

        public static TypeInfo GetTypeInfo(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = node.GetSemanticModel(provider);
            return model.GetTypeInfo(node);
        }

        public static bool HasAttribute<TAttribute>(this ISymbol symbol)
            where TAttribute : Attribute
        {
            var attributes = symbol.GetAttributes();
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<TAttribute>())
                    return true;
            }
            return false;
        }

        public static ITypeSymbol GetFirstDeclaringType(this IMethodSymbol method)
        {
            var overriddenMethod = method.OverriddenMethod;
            while (overriddenMethod != null)
            {
                method = overriddenMethod;
                overriddenMethod = method.OverriddenMethod;
            }

            return method.ContainingType;
        }

        public static ImmutableArray<AttributeData> GetAttributes(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var symbol = node.GetDeclaredSymbol(provider);
            return symbol.GetAttributes();
        }

        public static TSymbol GetSymbol<TSymbol>(this SyntaxNode node, ICompilationContextProvider provider)
            where TSymbol : ISymbol
        {
            return (TSymbol)GetSymbol(node, provider);
        }

        public static ISymbol GetSymbol(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = node.GetSemanticModel(provider);
            return model.GetSymbolInfo(node).Symbol;
        }

        public static TSymbol GetDeclaredSymbol<TSymbol>(this SyntaxNode node, ICompilationContextProvider provider)
            where TSymbol : ISymbol
        {
            return (TSymbol)node.GetDeclaredSymbol(provider);
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


        public static SemanticModel GetSemanticModel(this ICompilationContextProvider provider, SyntaxTree tree)
        {
            return provider.Compilation.GetSemanticModel(tree);
        }

        // Other implementations:
        // * https://github.com/GuOrg/Gu.Roslyn.Extensions/blob/master/Gu.Roslyn.AnalyzerExtensions/Symbols/INamedTypeSymbolExtensions.cs
        // * https://stackoverflow.com/a/27106959/213871
        // Reference: https://github.com/dotnet/roslyn/issues/1891
        public static string GetFullName(this ISymbol symbol)
        {
            return SymbolDisplay.ToDisplayString(symbol, DisplayFormats.FullnameFormat);
        }

        /// <summary>No namespace</summary>
        public static string GetQualifiedName(this ISymbol symbol)
        {
            return SymbolDisplay.ToDisplayString(symbol, DisplayFormats.QualifiedFormat);
        }
    }
}
