// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.CSharp;
using CodeBinder.Utils;
using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using CodeBinder.Java.Shared;
using System.Diagnostics.CodeAnalysis;

namespace CodeBinder.Java
{
    static partial class JavaExtensions
    {
        public static string GetJavaDefaultReturnStatement(this TypeSyntax type, JavaCodeConversionContext context)
        {
            var builder = new CodeBuilder();
            string? defaultLiteral = type.GetJavaDefaultLiteral(context);
            builder.Append("return");
            if (defaultLiteral != null)
                builder.Space().Append(defaultLiteral);

            return builder.ToString();
        }

        public static string? GetJavaDefaultLiteral(this TypeSyntax type, JavaCodeConversionContext context)
        {
            var fullName = type.GetFullName(context);
            switch(fullName)
            {
                case "System.Void":
                    return null;
                case "System.IntPtr":
                    return "0";
                case "System.Boolean":
                    return "false";
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                    return "0";
                default:
                    return "null";
            }
        }

        public static string GetJavaBoxType(this PredefinedTypeSyntax syntax)
        {
            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.BoolKeyword:
                    return "Boolean";
                case SyntaxKind.SByteKeyword:
                    return "Byte";
                case SyntaxKind.ByteKeyword:
                    return "Byte";
                case SyntaxKind.ShortKeyword:
                    return "Short";
                case SyntaxKind.UShortKeyword:
                    return "Short";
                case SyntaxKind.IntKeyword:
                    return "Integer";
                case SyntaxKind.UIntKeyword:
                    return "Integer";
                case SyntaxKind.LongKeyword:
                    return "Long";
                case SyntaxKind.ULongKeyword:
                    return "Long";
                case SyntaxKind.FloatKeyword:
                    return "Float";
                case SyntaxKind.DoubleKeyword:
                    return "Double";
                default:
                    throw new Exception();
            }
        }

        public static string GetJavaType(this PredefinedTypeSyntax syntax)
        {
            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.VoidKeyword:
                    return "void";
                case SyntaxKind.ObjectKeyword:
                    return "Object";
                case SyntaxKind.StringKeyword:
                    return "String";
                case SyntaxKind.BoolKeyword:
                    return "boolean";
                case SyntaxKind.SByteKeyword:
                    return "byte";
                case SyntaxKind.ByteKeyword:
                    return "byte";
                case SyntaxKind.ShortKeyword:
                    return "short";
                case SyntaxKind.UShortKeyword:
                    return "short";
                case SyntaxKind.IntKeyword:
                    return "int";
                case SyntaxKind.UIntKeyword:
                    return "int";
                case SyntaxKind.LongKeyword:
                    return "long";
                case SyntaxKind.ULongKeyword:
                    return "long";
                case SyntaxKind.FloatKeyword:
                    return "float";
                case SyntaxKind.DoubleKeyword:
                    return "double";
                default:
                    throw new Exception();
            }
        }

        public static string GetJavaType(this TypeSyntax type, JavaCodeConversionContext context)
        {
            var builder = new CodeBuilder();
            bool isInterface;
            return type.GetJavaType(context, out isInterface);
        }


        public static string GetJavaType(this TypeSyntax type, JavaCodeConversionContext context, out bool isInterface)
        {
            var builder = new CodeBuilder();
            var typeSymbol = type.GetTypeSymbol(context);
            writeJavaType(builder, typeSymbol.GetFullName(), type, typeSymbol, null, false, context, out isInterface);
            return builder.ToString();
        }

        public static string GetJavaType(this TypeSyntax type, JavaTypeFlags flags, JavaCodeConversionContext context)
        {
            var symbol = type.GetTypeSymbol(context);
            return getJavaType(symbol.GetFullName(), type, symbol, flags, context);
        }

        public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax syntax, IPropertySymbol symbol, JavaCodeConversionContext context)
        {
            writeJavaPropertyIdentifier(builder, syntax, symbol, context);
            return builder;
        }

        // TODO: This method should just handle whole replacements, also member access, example IntPtr.Zero -> 0
        public static bool TryToReplace(this CodeBuilder builder, SyntaxNode syntax, JavaCodeConversionContext context)
        {
            var symbol = syntax.GetSymbol(context);
            if (symbol == null)
                return false;

            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                {
                    var field = (IFieldSymbol)symbol;
                    if (field.HasJavaReplacement(out var replacement))
                    {
                        builder.Append(replacement.Name);
                        return true;
                    }
                    break;
                }
                case SymbolKind.Property:
                case SymbolKind.Method:
                {
                    // TODO
                    break;
                }
            }

            return false;
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeSyntax syntax, JavaCodeConversionContext context)
        {
            ISymbol symbol;
            // Symbol can be null https://github.com/dotnet/roslyn/issues/31471
            if (syntax.Kind() == SyntaxKind.ArrayType)
                symbol = syntax.GetTypeSymbol(context);
            else
                symbol = syntax.GetSymbol(context)!;

            switch (symbol.Kind)
            {
                case SymbolKind.TypeParameter:
                case SymbolKind.NamedType:
                case SymbolKind.ArrayType:
                {
                    bool isInterface;
                    var typeSymbol = (ITypeSymbol)symbol;
                    writeJavaType(builder, typeSymbol.GetFullName(), syntax, typeSymbol, null, false, context, out isInterface);
                    return builder;
                }
                case SymbolKind.Method:
                {
                    writeJavaMethodIdentifier(builder, syntax, (IMethodSymbol)symbol, context);
                    break;
                }
                case SymbolKind.Property:
                {
                    writeJavaPropertyIdentifier(builder, syntax, (IPropertySymbol)symbol, context);
                    break;
                }
                case SymbolKind.Parameter:
                {
                    writeJavaParameterIdentifier(builder, syntax, (IParameterSymbol)symbol, context);
                    break;
                }
                case SymbolKind.Local:
                case SymbolKind.Field:
                {
                    writeJavaIdentifier(builder, syntax, symbol, context);
                    break;
                }
                case SymbolKind.Namespace:
                {
                    // CHECK-ME: Evaluate substitution? Seems ok like this.
                    // Maybe better append the syntax instead of the symbol name?
                    // Evaluate and comment
                    builder.Append(symbol.Name);
                    break;
                }
                default:
                    throw new Exception();
            }

            return builder;
        }

        static string getJavaType(string typeName, TypeSyntax syntax, ITypeSymbol symbol, JavaTypeFlags flags, JavaCodeConversionContext context)
        {
            bool isByRef = flags.HasFlag(JavaTypeFlags.IsByRef);
            if (flags.HasFlag(JavaTypeFlags.NativeMethod))
            {
                switch (symbol.TypeKind)
                {
                    case TypeKind.Struct:
                    {
                        if (isByRef && !symbol.IsCLRPrimitiveType())
                        {
                            if (symbol.GetFullName() == "CodeBinder.cbstring")
                                return "StringBox";
                            else
                                return "long";
                        }

                        break;
                    }
                    case TypeKind.Enum:
                    {
                        if (isByRef)
                            return "IntegerBox"; // TODO: Box for enums on non native methods
                        else
                            return "int";
                    }
                }
            }

            var builder = new CodeBuilder();
            bool isInterface;
            writeJavaType(builder, typeName, syntax, symbol, null, isByRef, context, out isInterface);
            return builder.ToString();
        }

        static void writeJavaMethodIdentifier(CodeBuilder builder, TypeSyntax syntax, IMethodSymbol method, JavaCodeConversionContext context)
        {
            SymbolReplacement? replacement;
            string javaMethodName;
            if (method.HasJavaReplacement(out replacement))
            {
                javaMethodName = replacement.Name;
            }
            else
            {
                if (method.IsNative())
                {
                    javaMethodName = method.Name;
                }
                else
                {
                    javaMethodName = context.Conversion.MethodCasing == MethodCasing.LowerCamelCase ? method.Name.ToLowerCamelCase() : method.Name;
                }
            }

            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.IdentifierName:
                {
                    builder.Append(javaMethodName);
                    break;
                }
                case SyntaxKind.GenericName:
                {
                    var genericName = (GenericNameSyntax)syntax;
                    builder.Append(genericName.TypeArgumentList, genericName, context).Append(javaMethodName);
                    break;
                }
                default:
                    throw new Exception();
            }
        }

        static void writeJavaPropertyIdentifier(CodeBuilder builder, SyntaxNode syntax, IPropertySymbol property, JavaCodeConversionContext context)
        {
            bool isSetter = false;
            SyntaxNode child = syntax;
            var parent = syntax.Parent;
            while (parent != null)
            {
                AssignmentExpressionSyntax? assigment;
                if (parent.IsExpression(out assigment))
                {
                    // Determine if the LHS of an assiment is the current property symbol
                    if (assigment.Left == child && SymbolEqualityComparer.Default.Equals(assigment.Left.GetSymbol(context), property))
                    {
                        isSetter = true;
                        break;
                    }

                    break;
                }

                child = parent;
                parent = child.Parent;
            }

            // TODO: Better handle symbol replacements need/not need of parameter list
            SymbolReplacement? propertyReplacement;
            if (property.HasJavaReplacement(out propertyReplacement))
            {
                if (isSetter)
                    builder.Append(propertyReplacement.SetterName);
                else
                    builder.Append(propertyReplacement.Name);
            }
            else
            {
                // NOTE: proper use of the setter symbol is done eagerly
                // while writing AssignmentExpressionSyntax
                if (property.IsIndexer)
                {
                    if (isSetter)
                        builder.Append("set");
                    else
                        builder.Append("get");
                }
                else
                {
                    if (isSetter)
                        builder.Append("set").Append(property.Name);
                    else
                        builder.Append("get").Append(property.Name).EmptyParameterList();
                }
            }
        }

        static void writeJavaParameterIdentifier(CodeBuilder builder, TypeSyntax syntax, IParameterSymbol parameter, JavaCodeConversionContext context)
        {
            void writeBoxValueAccess()
            {
                writeJavaIdentifier(builder, syntax, parameter, context);
                builder.Dot().Append("value");
            }

            if (parameter.RefKind != RefKind.None)
            {
                switch (parameter.Type.TypeKind)
                {
                    case TypeKind.Enum:
                    {
                        writeBoxValueAccess();
                        return;
                    }
                    case TypeKind.Struct:
                    {
                        if (parameter.Type.IsCLRPrimitiveType())
                        {
                            writeBoxValueAccess();
                            return;
                        }

                        break;
                    }
                    default:
                        throw new Exception();
                }
            }

            writeJavaIdentifier(builder, syntax, parameter, context);
        }

        static void writeJavaIdentifier(CodeBuilder builder, TypeSyntax syntax, ISymbol symbol, JavaCodeConversionContext context)
        {
            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.IdentifierName:
                {
                    var identifierName = (IdentifierNameSyntax)syntax;
                    builder.Append(identifierName.GetName());
                    break;
                }
                default:
                    throw new Exception();
            }
        }

        static CodeBuilder Append(this CodeBuilder builder, TypeSyntax type, TypeSyntax parent,
            JavaCodeConversionContext context)
        {
            bool isInterface;
            var typeSymbol = type.GetTypeSymbol(context);
            writeJavaType(builder, typeSymbol.GetFullName(), type, typeSymbol, parent, false, context, out isInterface);
            return builder;
        }

        static void writeJavaType(CodeBuilder builder, string fullTypeName, TypeSyntax type, ITypeSymbol symbol,
            TypeSyntax? parent, bool isByRef, JavaCodeConversionContext context, out bool isInterface)
        {
            if (type.IsVar)
            {
                // Type is inferred
                writeTypeSymbol(builder, fullTypeName, symbol, null, isByRef, out isInterface);
                return;
            }

            // Try to adjust the typename, looking for know types
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                {
                    var namedType = (INamedTypeSymbol)symbol;
                    if (namedType.IsGenericType)
                        fullTypeName = namedType.ConstructedFrom.GetFullName();

                    break;
                }
                case SymbolKind.ArrayType:
                {
                    var arrayType = (IArrayTypeSymbol)symbol;
                    fullTypeName = arrayType.ElementType.GetFullName();
                    break;
                }
                case SymbolKind.TypeParameter:
                    // Nothing to do
                    break;
                default:
                    throw new Exception();
            }

            var typeKind = type.Kind();
            string? javaTypeName;
            if (IsKnownJavaType(fullTypeName, isByRef, parent?.GetTypeSymbol(context), out javaTypeName, out isInterface))
            {
                switch (typeKind)
                {
                    case SyntaxKind.GenericName:
                    {
                        var genericType = (GenericNameSyntax)type;
                        builder.Append(javaTypeName).Append(genericType.TypeArgumentList, type, context);
                        break;
                    }
                    case SyntaxKind.ArrayType:
                    {
                        var arrayType = (ArrayTypeSyntax)type;
                        Debug.Assert(arrayType.RankSpecifiers.Count == 1);
                        builder.Append(javaTypeName).Append(arrayType.RankSpecifiers[0], context);
                        break;
                    }
                    case SyntaxKind.NullableType:
                    {
                        string? boxTypeName;
                        if (JavaUtils.TryGetBoxType(fullTypeName, out boxTypeName))
                            builder.Append(boxTypeName);
                        else
                            throw new Exception();
                        break;
                    }
                    case SyntaxKind.PredefinedType:
                    {
                        builder.Append(javaTypeName);
                        break;
                    }
                    case SyntaxKind.IdentifierName:
                    {
                        builder.Append(javaTypeName);
                        break;
                    }
                    case SyntaxKind.QualifiedName:
                    {
                        var qualifiedName = type as QualifiedNameSyntax;
                        throw new Exception();
                    }
                    default:
                        throw new Exception();
                }
            }
            else
            {
                switch (typeKind)
                {
                    case SyntaxKind.IdentifierName:
                    {
                        var identifierName = (IdentifierNameSyntax)type;
                        builder.Append(identifierName.GetName());
                        break;
                    }
                    case SyntaxKind.ArrayType:
                    {
                        var arrayType = (ArrayTypeSyntax)type;
                        Debug.Assert(arrayType.RankSpecifiers.Count == 1);
                        builder.Append(arrayType.ElementType, type, context).Append(arrayType.RankSpecifiers[0], context);
                        break;
                    }
                    case SyntaxKind.GenericName:
                    {
                        var genericType = (GenericNameSyntax)type;
                        builder.Append(genericType.GetName()).Append(genericType.TypeArgumentList, type, context);
                        break;
                    }
                    case SyntaxKind.NullableType:
                    {
                        var nullableType = (NullableTypeSyntax)type;
                        switch (symbol.TypeKind)
                        {
                            case TypeKind.Struct:
                                builder.Append(nullableType.ElementType, type, context);
                                break;
                            case TypeKind.Enum:
                                throw new Exception("TODO");
                            default:
                                throw new Exception();
                        }

                        break;
                    }
                    case SyntaxKind.QualifiedName:
                    {
                        var qualifiedName = (QualifiedNameSyntax)type;
                        builder.Append(qualifiedName.Left, type, context).Dot().Append(qualifiedName.Right, type, context);
                        break;
                    }
                    default:
                        throw new Exception();
                }

                isInterface = symbol.TypeKind == TypeKind.Interface;
            }
        }

        static CodeBuilder Append(this CodeBuilder builder, ITypeSymbol symbol, ITypeSymbol parent)
        {
            bool isInterface;
            writeTypeSymbol(builder, symbol.GetFullName(), symbol, parent, false, out isInterface);
            return builder;
        }

        /// <summary>This method is mainly used for inferred types</summary>
        static void writeTypeSymbol(CodeBuilder builder, string fullTypeName, ITypeSymbol symbol,
            ITypeSymbol? parentSymbol, bool isByRef, out bool isInterface)
        {
            // Try to adjust the typename, looking for know types
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                {
                    var namedType = (INamedTypeSymbol)symbol;
                    if (namedType.IsGenericType)
                    {
                        if (namedType.IsNullable())
                        {
                            switch (symbol.TypeKind)
                            {
                                case TypeKind.Struct:
                                    var nullableType = namedType.TypeArguments[0];
                                    writeTypeSymbol(builder, nullableType.GetFullName(), nullableType, symbol, isByRef, out isInterface);
                                    return;
                                case TypeKind.Enum:
                                    throw new Exception("TODO");
                                default:
                                    throw new Exception();
                            }
                        }

                        fullTypeName = namedType.ConstructedFrom.GetFullName();
                    }

                    break;
                }
                case SymbolKind.ArrayType:
                {
                    var arrayType = (IArrayTypeSymbol)symbol;
                    builder.Append(arrayType.ElementType, symbol).EmptyRankSpecifier();
                    isInterface = false;
                    return;
                }
                case SymbolKind.TypeParameter:
                    // Nothing to do
                    break;
                default:
                    throw new Exception();
            }

            string? javaTypeName;
            if (!IsKnownJavaType(fullTypeName, isByRef, parentSymbol, out javaTypeName, out isInterface))
            {
                isInterface = symbol.TypeKind == TypeKind.Interface;
                javaTypeName = symbol.GetQualifiedName();
            }

            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                {
                    builder.Append(javaTypeName);
                    var namedType = (INamedTypeSymbol)symbol;
                    if (namedType.IsGenericType)
                    {
                        using (builder.TypeParameterList())
                        {
                            bool first = true;
                            foreach (var parameter in namedType.TypeArguments)
                                builder.CommaSeparator(ref first).Append(parameter, symbol);
                        }
                    }

                    break;
                }
                case SymbolKind.TypeParameter:
                {
                    builder.Append(javaTypeName);
                    break;
                }
                default:
                    throw new Exception();
            }
        }

        static CodeBuilder Append(this CodeBuilder builder, TypeArgumentListSyntax syntax, TypeSyntax parent, JavaCodeConversionContext context)
        {
            using (builder.TypeParameterList())
            {
                bool first = true;
                foreach (var type in syntax.Arguments)
                    builder.CommaSeparator(ref first).Append(type, parent, context);
            }

            return builder;
        }

        static CodeBuilder Append(this CodeBuilder builder, ArrayRankSpecifierSyntax syntax, JavaCodeConversionContext context)
        {
            if (syntax.Sizes.Count > 0)
            {
                Debug.Assert(syntax.Sizes.Count == 1);
                builder.Bracketed().Append(syntax.Sizes[0], context);
            }
            else
            {
                builder.EmptyRankSpecifier();
            }

            return builder;
        }

        public static bool IsKnownJavaType(string fullTypeName, bool isByRef, ITypeSymbol? parent,
            [NotNullWhen(true)]out string? knownJavaType, out bool isInterface)
        {
            if (IsKnowSimpleJavaType(fullTypeName, isByRef, parent, out knownJavaType))
            {
                isInterface = false;
                return true;
            }

            switch (fullTypeName)
            {
                case "System.Exception":
                {
                    knownJavaType = "RuntimeException";
                    isInterface = false;
                    return true;
                }
                case "System.NotImplementedException":
                {
                    knownJavaType = "UnsupportedOperationException";
                    isInterface = false;
                    return true;
                }
                case "System.IDisposable":
                {
                    knownJavaType = "AutoCloseable";
                    isInterface = true;
                    return true;
                }
                case "System.Collections.Generic.IEnumerable<out T>":
                {
                    knownJavaType = "Iterable";
                    isInterface = true;
                    return true;
                }
                case "System.Collections.Generic.IList<T>":
                {
                    knownJavaType = "List";
                    isInterface = false;
                    return true;
                }
                case "System.Collections.Generic.List<T>":
                {
                    knownJavaType = "ArrayList";
                    isInterface = false;
                    return true;
                }
                case "System.Collections.Generic.KeyValuePair<TKey, TValue>":
                {
                    knownJavaType = "Map.Entry";
                    isInterface = true;
                    return true;
                }
                default:
                {
                    knownJavaType = null;
                    isInterface = false;
                    return false;
                }
            }
        }

        public static bool IsKnowSimpleJavaType(string fullTypeName, bool isByRef, ITypeSymbol? parent, [NotNullWhen(true)]out string? knownJavaType)
        {
            if (isByRef)
            {
                if (JavaUtils.TryGetRefBoxType(fullTypeName, out knownJavaType))
                    return true;
                else
                    return false;
            }
            else if (parent?.IsGeneric() == true)
            {
                switch (fullTypeName)
                {
                    case "System.Object":
                        knownJavaType = "Object";
                        return true;
                    case "System.String":
                        knownJavaType = "String";
                        return true;
                    // Boxed types
                    case "System.IntPtr":
                        knownJavaType = "Long";
                        return true;
                    case "System.Boolean":
                        knownJavaType = "Boolean";
                        return true;
                    case "System.Byte":
                        knownJavaType = "Byte";
                        return true;
                    case "System.SByte":
                        knownJavaType = "Byte";
                        return true;
                    case "System.Int16":
                        knownJavaType = "Short";
                        return true;
                    case "System.UInt16":
                        knownJavaType = "Short";
                        return true;
                    case "System.Int32":
                        knownJavaType = "Integer";
                        return true;
                    case "System.UInt32":
                        knownJavaType = "Integer";
                        return true;
                    case "System.Int64":
                        knownJavaType = "Long";
                        return true;
                    case "System.UInt64":
                        knownJavaType = "Long";
                        return true;
                    case "System.Single":
                        knownJavaType = "Float";
                        return true;
                    case "System.Double":
                        knownJavaType = "Double";
                        return true;
                    case "System.Void":
                        throw new Exception();
                    default:
                        knownJavaType = null;
                        return false;
                }
            }
            else
            {
                switch (fullTypeName)
                {
                    case "System.Void":
                        knownJavaType = "void";
                        return true;
                    case "System.Object":
                        knownJavaType = "Object";
                        return true;
                    case "CodeBinder.cbstring":
                        knownJavaType = "String";
                        return true;
                    case "System.String":
                        knownJavaType = "String";
                        return true;
                    case "System.IntPtr":
                        knownJavaType = "long";
                        return true;
                    case "System.Boolean":
                        knownJavaType = "boolean";
                        return true;
                    case "System.Byte":
                        knownJavaType = "byte";
                        return true;
                    case "System.SByte":
                        knownJavaType = "byte";
                        return true;
                    case "System.Int16":
                        knownJavaType = "short";
                        return true;
                    case "System.UInt16":
                        knownJavaType = "short";
                        return true;
                    case "System.Int32":
                        knownJavaType = "int";
                        return true;
                    case "System.UInt32":
                        knownJavaType = "int";
                        return true;
                    case "System.Int64":
                        knownJavaType = "long";
                        return true;
                    case "System.UInt64":
                        knownJavaType = "long";
                        return true;
                    case "System.Single":
                        knownJavaType = "float";
                        return true;
                    case "System.Double":
                        knownJavaType = "double";
                        return true;
                    default:
                        knownJavaType = null;
                        return false;
                }
            }
        }
    }
}
