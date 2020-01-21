// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.FindSymbols;
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

        public static AttributeData GetAttribute<TAttribute>(this IEnumerable<AttributeData> attributes)
            where TAttribute : Attribute
        {
            AttributeData ret;
            if (!TryGetAttribute<TAttribute>(attributes, out ret))
                throw new Exception($"Missing attribute {typeof(TAttribute).Name}");

            return ret;
        }

        public static bool TryGetAttribute<TAttribute>(this IEnumerable<AttributeData> attributes, out AttributeData attribute)
            where TAttribute : Attribute
        {
            foreach (var attrib in attributes)
            {
                if (attrib.IsAttribute<TAttribute>())
                {
                    attribute = attrib;
                    return true;
                }
            }

            attribute = null;
            return false;
        }

        public static bool HasAttribute<TAttribute>(this IEnumerable<AttributeData> attributes)
            where TAttribute : Attribute
        {
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<TAttribute>())
                    return true;
            }
            return false;
        }

        public static bool HasAttribute<TAttribute>(this ISymbol symbol)
            where TAttribute : Attribute
        {
            return symbol.GetAttributes().HasAttribute<TAttribute>();
        }

        public static bool TryGetAttribute<TAttribute>(this ISymbol symbol, out AttributeData attribute)
            where TAttribute : Attribute
        {
            return symbol.GetAttributes().TryGetAttribute<TAttribute>(out attribute);
        }

        public static AttributeData GetAttribute<TAttribute>(this ISymbol symbol)
            where TAttribute : Attribute
        {
            return symbol.GetAttributes().GetAttribute<TAttribute>();
        }

        public static bool Inherits<T>(this AttributeData data)
            where T : Attribute
        {
            return data.AttributeClass.Inherits<T>();
        }

        public static bool Inherits<T>(this ITypeSymbol symbol)
            where T : class
        {
            var baseType = symbol.BaseType;
            while (baseType != null)
            {
                if (baseType.GetFullName() == typeof(T).FullName)
                    return true;

                baseType = baseType.BaseType;
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

        public static T GetConstructorArgument<T>(this AttributeData data, int index)
        {
            T ret;
            if (!TryGetConstructorArgument(data, index, out ret))
                throw new IndexOutOfRangeException();

            return ret;
        }

        public static bool TryGetConstructorArgument<T>(this AttributeData data, int index, out T value)
        {
            if (index < 0 || index >= data.ConstructorArguments.Length)
            {
                value = default(T);
                return false;
            }

            value = (T)data.ConstructorArguments[index].Value;
            return true;
        }

        public static T GetConstructorArgumentOrDefault<T>(this AttributeData data, int index, T def)
        {
            if (index < 0 || index >= data.ConstructorArguments.Length)
                return def;

            return (T)data.ConstructorArguments[index].Value;
        }

        public static T GetConstructorArgumentOrDefault<T>(this AttributeData data, int index)
        {
            return GetConstructorArgumentOrDefault(data, index, default(T));
        }

        public static T GetNamedArgument<T>(this AttributeData data, string name)
        {
            T ret;
            if (!TryGetNamedArgument(data, name, out ret))
                throw new KeyNotFoundException();

            return ret;
        }

        public static bool TryGetNamedArgument<T>(this AttributeData data, string name, out T value)
        {
            foreach (var pair in data.NamedArguments)
            {
                if (pair.Key == name)
                {
                    value = (T)pair.Value.Value;
                    return true;
                }
            }

            value = default(T);
            return false;
        }

        public static T GetNamedArgumentOrDefault<T>(this AttributeData data, string name)
        {
            return GetNamedArgumentOrDefault(data, name, default(T));
        }

        public static T GetNamedArgumentOrDefault<T>(this AttributeData data, string name, T def)
        {
            foreach (var pair in data.NamedArguments)
            {
                if (pair.Key == name)
                    return (T)pair.Value.Value;
            }

            return def;
        }

        public static ImmutableArray<AttributeData> GetAttributes(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var symbol = node.GetDeclaredSymbol(provider);
            return symbol.GetAttributes();
        }

        public static IOperation GetOperation(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = node.GetSemanticModel(provider);
            return model.GetOperation(node);
        }

        public static TOperation GetOperation<TOperation>(this SyntaxNode node, ICompilationContextProvider provider)
            where TOperation : IOperation
        {
            return (TOperation)GetOperation(node, provider);
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
