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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

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

        public static bool ShouldDiscard(this ISymbol symbol, LanguageConversion conversion)
        {
            AttributeData? data;
            var attributes = symbol.GetAttributes();
            if (symbol.TryGetAttribute<IgnoreAttribute>(out data))
            {
                var conversionsToIgnore = data.GetConstructorArgumentOrDefault(0, Conversions.All);
                if (conversion.IsNative)
                {
                    if (conversionsToIgnore.HasFlag(Conversions.Native))
                        return true;
                }
                else
                {
                    if (conversionsToIgnore.HasFlag(Conversions.Regular))
                        return true;
                }
            }

            if (attributes.TryGetAttribute<RequiresAttribute>(out data))
            {
                var policies = data.GetConstructorArgumentArray<string>(0);
                foreach (string policy in policies)
                {
                    if (!conversion.SupportedPolicies.Contains(policy))
                        return true;
                }
            }

            if (conversion.DiscardNative)
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Method:
                    {
                        if (attributes.HasAttribute<DllImportAttribute>())
                            return true;

                        break;
                    }
                    case SymbolKind.NamedType:
                    {
                        INamedTypeSymbol type = (INamedTypeSymbol)symbol;
                        if (type.TypeKind == TypeKind.Delegate && attributes.HasAttribute<UnmanagedFunctionPointerAttribute>())
                            return true;
                        break;
                    }
                }
            }

            return false;
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
            AttributeData? ret;
            if (!TryGetAttribute<TAttribute>(attributes, out ret))
                throw new Exception($"Missing attribute {typeof(TAttribute).Name}");

            return ret;
        }

        public static bool TryGetAttribute<TAttribute>(this IEnumerable<AttributeData> attributes, [NotNullWhen(true)]out AttributeData? attribute)
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

        public static bool TryGetAttribute<TAttribute>(this ISymbol symbol, [NotNullWhen(true)]out AttributeData? attribute)
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

        public static T[] GetConstructorArgumentArray<T>(this AttributeData data, int index)
        {
            T[]? ret;
            if (!TryGetConstructorArgument(data, index, out ret))
                throw new IndexOutOfRangeException();

            return ret;
        }

        public static bool TryGetConstructorArgument<T>(this AttributeData data, int index, [NotNullWhen(true)]out T[]? array)
        {
            if (index < 0 || index >= data.ConstructorArguments.Length)
            {
                array = null;
                return false;
            }

            var values = data.ConstructorArguments[index].Values!;
            array = new T[values.Length];
            for (int i= 0; i < values.Length; i++)
            {
                var value = values[i];
                array[i] = (T)value.Value!;
            }

            return true;
        }

        public static bool TryGetConstructorArgument<T>(this AttributeData data, int index, [MaybeNullWhen(false)]out T value)
        {
            if (index < 0 || index >= data.ConstructorArguments.Length)
            {
                value = default!;
                return false;
            }

            value = (T)data.ConstructorArguments[index].Value!;
            return true;
        }

        public static T GetConstructorArgumentOrDefault<T>(this AttributeData data, int index)
        {
            return GetConstructorArgumentOrDefault(data, index, default(T)!);
        }

        public static T GetConstructorArgumentOrDefault<T>(this AttributeData data, int index, T def)
        {
            T ret;
            if (TryGetConstructorArgument(data, index, out ret))
                return ret;

            return def;
        }

        public static T GetNamedArgument<T>(this AttributeData data, string name)
        {
            T ret;
            if (!TryGetNamedArgument(data, name, out ret))
                throw new KeyNotFoundException();

            return ret;
        }

        public static T[]? GetNamedArgumentArray<T>(this AttributeData data, string name)
        {
            T[]? ret;
            if (!TryGetNamedArgument(data, name, out ret))
                throw new KeyNotFoundException();

            return ret;
        }

        public static bool TryGetNamedArgument<T>(this AttributeData data, string name, [MaybeNull]out T value)
        {
            foreach (var pair in data.NamedArguments)
            {
                if (pair.Key == name)
                {
                    value = (T)pair.Value.Value!;
                    return true;
                }
            }

            value = default!;
            return false;
        }

        public static bool TryGetNamedArgument<T>(this AttributeData data, string name, [MaybeNull]out T[]? array)
        {
            foreach (var pair in data.NamedArguments)
            {
                if (pair.Key == name)
                {
                    var values = pair.Value.Values!;
                    array = new T[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        var value = values[i];
                        array[i] = (T)value.Value!;
                    }
                    return true;
                }
            }

            array = null;
            return false;
        }

        public static T GetNamedArgumentOrDefault<T>(this AttributeData data, string name)
        {
            return GetNamedArgumentOrDefault(data, name, default(T)!);
        }

        public static T GetNamedArgumentOrDefault<T>(this AttributeData data, string name, T def)
        {
            T ret;
            if (TryGetNamedArgument(data, name, out ret))
                return ret;

            return def;
        }

        public static ImmutableArray<AttributeData> GetAttributes(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var symbol = node.GetDeclaredSymbol(provider);
            if (symbol == null)
                throw new Exception($"No attributes for syntax {node}");

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

        public static bool TryGetSymbol<TSymbol>(this SyntaxNode node, ICompilationContextProvider provider, [NotNullWhen(true)]out TSymbol? symbol)
            where TSymbol : class, ISymbol
        {
            symbol = GetSymbol(node, provider) as TSymbol;
            return symbol != null;
        }

        public static TSymbol GetSymbol<TSymbol>(this SyntaxNode node, ICompilationContextProvider provider)
            where TSymbol : class,ISymbol
        {
            return GetSymbol(node, provider) as TSymbol ?? throw new Exception($"Unable to get symbol {typeof(ISymbol).Name} in syntax: {node}");
        }

        public static ISymbol? GetSymbol(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = node.GetSemanticModel(provider);
            return model.GetSymbolInfo(node).Symbol;
        }

        public static TSymbol GetDeclaredSymbol<TSymbol>(this SyntaxNode node, ICompilationContextProvider provider)
            where TSymbol : class,ISymbol
        {
            return GetDeclaredSymbol(node, provider) as TSymbol ?? throw new Exception($"Unable to get declared symbol {typeof(ISymbol).Name} in syntax: {node}");
        }

        public static ISymbol? GetDeclaredSymbol(this SyntaxNode node, ICompilationContextProvider provider)
        {
            var model = node.GetSemanticModel(provider);
            return model.GetDeclaredSymbol(node);
        }

        public static ITypeSymbol? GetTypeSymbol(this SyntaxNode node, ICompilationContextProvider provider)
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

        public static bool HasAccessibility(this ISymbol symbol, Accessibility accessibility)
        {
            if (symbol.DeclaredAccessibility == accessibility)
            {
                return true;
            }
            else if (accessibility == Accessibility.ProtectedOrInternal)
            {
                switch (symbol.DeclaredAccessibility)
                {
                    case Accessibility.Protected:
                    case Accessibility.Internal:
                    case Accessibility.ProtectedAndInternal:
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static SemanticModel GetSemanticModel(this ICompilationContextProvider provider, SyntaxTree tree)
        {
            return provider.Compilation.GetSemanticModel(tree);
        }

        /// <summary>
        /// Get namespace qualified name
        /// </summary>
        // Other implementations:
        // * https://github.com/GuOrg/Gu.Roslyn.Extensions/blob/master/Gu.Roslyn.AnalyzerExtensions/Symbols/INamedTypeSymbolExtensions.cs
        // * https://stackoverflow.com/a/27106959/213871
        // Reference: https://github.com/dotnet/roslyn/issues/1891
        public static string GetFullName(this ISymbol symbol)
        {
            return SymbolDisplay.ToDisplayString(symbol, DisplayFormats.FullnameFormat);
        }

        /// <summary>
        /// Get no namespace qualified name
        /// </summary>
        public static string GetQualifiedName(this ISymbol symbol)
        {
            return SymbolDisplay.ToDisplayString(symbol, DisplayFormats.QualifiedFormat);
        }

        // From https://stackoverflow.com/a/23308759/213871
        // Feature Request in roslyn https://github.com/dotnet/roslyn/issues/1891
        /// <summary>
        /// Get CLR Metadata type name that can be used with Type.GetType(name)
        /// </summary>
        public static string GetAssemblyQualifiedName(this ITypeSymbol symbol)
        {
            return symbol.ContainingNamespace
                + "." + symbol.Name
                + ", " + symbol.ContainingAssembly;
        }

        /// <summary>
        /// Construct an attribute from AttributeData
        /// </summary>
        public static TAttrib Construct<TAttrib>(this AttributeData data)
            where TAttrib : Attribute
        {
            Type[] types = new Type[data.ConstructorArguments.Length];
            object?[] objects = new object[data.ConstructorArguments.Length];
            for (int i = 0; i < data.ConstructorArguments.Length; i++)
            {
                var arg = data.ConstructorArguments[i];

                var type = Type.GetType(arg.Type.GetAssemblyQualifiedName()) ??
                    throw new NullReferenceException($"Unable to find CLR type for {arg.Type}");
                types[i] = type;
                objects[i] = arg.Value;
            }

            var construcor = typeof(TAttrib).GetConstructor(types) ??
                    throw new NullReferenceException($"Unable to attribute constructor for the types {data.ConstructorArguments}");
            var attrib = (TAttrib)construcor.Invoke(objects);
            for (int i = 0; i < data.NamedArguments.Length; i++)
            {
                var arg = data.NamedArguments[i];
                var property = typeof(TAttrib).GetProperty(arg.Key);
                if (property == null)
                {
                    var field = typeof(TAttrib).GetField(arg.Key);
                    if (field == null)
                        throw new Exception($"Missing property or field with name {arg.Key}");

                    field.SetValue(attrib, arg.Value.Value);
                }
                else
                {
                    property.SetValue(attrib, arg.Value.Value);
                }
            }

            return attrib;
        }
    }
}
