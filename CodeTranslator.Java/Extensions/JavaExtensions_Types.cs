// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using CodeTranslator.Shared;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using CodeTranslator.Shared.Java;

namespace CodeTranslator.Java
{
    static partial class JavaExtensions
    {
        public static string GetJavaDefaultReturnStatement(this TypeSyntax type, ICompilationContextProvider provider)
        {
            var builder = new CodeBuilder();
            string defaultLiteral = type.GetJavaDefaultLiteral(provider);
            builder.Append("return");
            if (!string.IsNullOrEmpty(defaultLiteral))
                builder.Space().Append(defaultLiteral);

            return builder.ToString();
        }

        public static string GetJavaDefaultLiteral(this TypeSyntax type, ICompilationContextProvider provider)
        {
            var fullName = type.GetFullName(provider);
            switch(fullName)
            {
                case "System.Void":
                    return null;
                case "System.IntPtr":
                    return "0";
                case "System.Boolean":
                    return "false";
                case "System.Char":
                    return "'\0'";
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
                case SyntaxKind.CharKeyword:
                    return "Character";
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
                case SyntaxKind.CharKeyword:
                    return "char";
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

        public static string GetJavaType(this TypeSyntax type, ICompilationContextProvider provider)
        {
            var builder = new CodeBuilder();
            bool isInterface;
            return type.GetJavaType(provider, out isInterface);
        }


        public static string GetJavaType(this TypeSyntax type, ICompilationContextProvider provider, out bool isInterface)
        {
            var builder = new CodeBuilder();
            var typeSymbol = type.GetTypeSymbol(provider);
            writeJavaType(builder, typeSymbol?.GetFullName(), type, typeSymbol, false, provider, out isInterface);
            return builder.ToString();
        }

        public static string GetJavaType(ref this MethodParameterInfo parameter, JavaTypeFlags flags)
        {
            ITypeSymbol typeSymbol;
            string typeName = parameter.GetTypeName(out typeSymbol);
            return getJavaType(typeName, null, typeSymbol, flags, null);
        }

        public static string GetJavaType(this TypeSyntax type, JavaTypeFlags flags, ICompilationContextProvider provider)
        {
            var symbol = type.GetTypeSymbol(provider);
            return getJavaType(symbol.GetFullName(), type, symbol, flags, provider);
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeSyntax syntax, ICompilationContextProvider provider)
        {
            var symbol = syntax.GetSymbol(provider);
            switch (symbol.Kind)
            {
                case SymbolKind.TypeParameter:
                case SymbolKind.NamedType:
                case SymbolKind.ArrayType:
                {
                    bool isInterface;
                    var typeSymbol = syntax.GetTypeSymbol(provider);
                    writeJavaType(builder, typeSymbol?.GetFullName(), syntax, typeSymbol, false, provider, out isInterface);
                    return builder;
                }
                case SymbolKind.Local:
                case SymbolKind.Field:
                case SymbolKind.Property:
                case SymbolKind.Parameter:
                case SymbolKind.Method:
                {
                    writeJavaIdentifier(builder, syntax, symbol, provider);
                    break;
                }
                default:
                    throw new Exception();
            }

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeArgumentListSyntax syntax, ICompilationContextProvider provider)
        {
            using (builder.TypeParameterList())
            {
                bool first = true;
                foreach (var type in syntax.Arguments)
                {
                    if (first)
                        first = false;
                    else
                        builder.CommaSeparator();

                    builder.Append(type, provider);
                }
            }

            return builder;
        }

        static void writeJavaIdentifier(CodeBuilder builder, TypeSyntax syntax, ISymbol symbol, ICompilationContextProvider provider)
        {
            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.IdentifierName:
                {
                    var identifierName = syntax as IdentifierNameSyntax;
                    builder.Append(identifierName.GetName());
                    break;
                }
                default:
                    throw new Exception();
            }
        }

        static void writeJavaType(CodeBuilder builder, string typeName, TypeSyntax type, ITypeSymbol symbol,
            bool isByRef, ICompilationContextProvider provider, out bool isInterface)
        {
            if (symbol != null)
            {
                // Try to adjust the typename, looking for know types
                switch (symbol.Kind)
                {
                    case SymbolKind.NamedType:
                    {
                        var namedType = symbol as INamedTypeSymbol;
                        if (namedType.IsGenericType)
                            typeName = namedType.ConstructedFrom.GetFullName();

                        break;
                    }
                    case SymbolKind.ArrayType:
                    {
                        var arrayType = symbol as IArrayTypeSymbol;
                        typeName = arrayType.ElementType.GetFullName();
                        break;
                    }
                    case SymbolKind.TypeParameter:
                        // Nothing to do
                        break;
                    default:
                        throw new Exception();
                }
            }

            string javaTypeName;
            if (IsKnownJavaType(typeName, isByRef, out javaTypeName, out isInterface))
            {
                if (type == null)
                {
                    builder.Append(javaTypeName);
                    return;
                }

                Debug.Assert(provider != null);

                var kind = type.Kind();
                switch (kind)
                {
                    case SyntaxKind.GenericName:
                    {
                        var arrayType = type as GenericNameSyntax;
                        builder.Append(javaTypeName).Append(arrayType.TypeArgumentList, provider);
                        break;
                    }
                    case SyntaxKind.ArrayType:
                    {
                        var genericType = type as ArrayTypeSyntax;
                        builder.Append(javaTypeName).Append("[]");
                        break;
                    }
                    case SyntaxKind.NullableType:
                    {
                        switch (javaTypeName)
                        {
                            // Boxable types
                            case "boolean":
                                builder.Append("Boolean");
                                break;
                            case "char":
                                builder.Append("Character");
                                break;
                            case "byte":
                                builder.Append("Byte");
                                break;
                            case "short":
                                builder.Append("Short");
                                break;
                            case "int":
                                builder.Append("Integer");
                                break;
                            case "long":
                                builder.Append("Long");
                                break;
                            case "float":
                                builder.Append("Float");
                                break;
                            case "double":
                                builder.Append("Double");
                                break;
                            default:
                                throw new Exception();
                        }
                        break;
                    }
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                        builder.Append(javaTypeName);
                        break;
                    default:
                        throw new Exception();
                }
            }
            else
            {
                if (type == null)
                {
                    builder.Append(typeName);
                    return;
                }

                Debug.Assert(provider != null);

                var kind = type.Kind();
                switch (kind)
                {
                    case SyntaxKind.IdentifierName:
                    {
                        var identifierName = type as IdentifierNameSyntax;
                        builder.Append(identifierName.GetName());
                        break;
                    }
                    case SyntaxKind.ArrayType:
                    {
                        var arrayType = type as ArrayTypeSyntax;
                        builder.Append(arrayType.ElementType, provider).Append("[]");
                        break;
                    }
                    case SyntaxKind.GenericName:
                    {
                        var genericType = type as GenericNameSyntax;
                        builder.Append(genericType.GetName()).Append(genericType.TypeArgumentList, provider);
                        break;
                    }
                    case SyntaxKind.NullableType:
                    {
                        Debug.Assert(symbol != null);
                        var nullableType = type as NullableTypeSyntax;
                        switch (symbol.TypeKind)
                        {
                            case TypeKind.Struct:
                                builder.Append(nullableType.ElementType, provider);
                                break;
                            case TypeKind.Enum:
                                throw new Exception("TODO");
                            default:
                                throw new Exception();
                        }

                        break;
                    }
                    default:
                        throw new Exception();
                }

                isInterface = symbol?.TypeKind == TypeKind.Interface;
            }
        }

        static string getJavaType(string typeName, TypeSyntax syntax, ITypeSymbol symbol, JavaTypeFlags flags, ICompilationContextProvider provider)
        {
            bool isByRef = flags.HasFlag(JavaTypeFlags.IsByRef);
            if (symbol?.TypeKind == TypeKind.Enum)
            {
                if (isByRef)
                    return "IntegerBox"; // TODO: Box per gli enum?
                else
                    return "int";
            }

            if (symbol?.TypeKind == TypeKind.Struct && isByRef && flags.HasFlag( JavaTypeFlags.NativeMethod))
                return "long";

            var builder = new CodeBuilder();
            bool isInterface;
            writeJavaType(builder, typeName, syntax, symbol, isByRef, provider, out isInterface);
            return builder.ToString();
        }

        public static bool IsKnownJavaType(string typeName, bool isByRef, out string knownJavaType, out bool isInterface)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                knownJavaType = null;
                isInterface = false;
                return false;
            }

            if (IsKnowSimpleJavaType(typeName, isByRef, out knownJavaType))
            {
                isInterface = false;
                return true;
            }

            switch (typeName)
            {
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
                case "System.Collections.Generic.List<T>":
                {
                    knownJavaType = "ArrayList";
                    isInterface = false;
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

        public static bool IsKnowSimpleJavaType(string typeName, bool isByRef, out string knownJavaType)
        {
            if (isByRef)
            {
                switch (typeName)
                {
                    case "System.Boolean":
                        knownJavaType = "BooleanBox";
                        return true;
                    case "System.Char":
                        knownJavaType = "CharacterBox";
                        return true;
                    case "System.Byte":
                        knownJavaType = "ByteBox";
                        return true;
                    case "System.SByte":
                        knownJavaType = "ByteBox";
                        return true;
                    case "System.Int16":
                        knownJavaType = "ShortBox";
                        return true;
                    case "System.UInt16":
                        knownJavaType = "ShortBox";
                        return true;
                    case "System.Int32":
                        knownJavaType = "IntegerBox";
                        return true;
                    case "System.UInt32":
                        knownJavaType = "IntegerBox";
                        return true;
                    case "System.Int64":
                        knownJavaType = "LongBox";
                        return true;
                    case "System.UInt64":
                        knownJavaType = "LongBox";
                        return true;
                    case "System.Single":
                        knownJavaType = "FloatBox";
                        return true;
                    case "System.Double":
                        knownJavaType = "DoubleBox";
                        return true;
                    case "System.String":
                        knownJavaType = "StringBox";
                        return true;
                    case "System.IntPtr":
                        knownJavaType = "LongBox";
                        return true;
                    default:
                        knownJavaType = null;
                        return false;
                }
            }
            else
            {
                switch (typeName)
                {
                    case "System.Void":
                        knownJavaType = "void";
                        return true;
                    case "System.Object":
                        knownJavaType = "Object";
                        return true;
                    case "System.IntPtr":
                        knownJavaType = "long";
                        return true;
                    case "System.Boolean":
                        knownJavaType = "boolean";
                        return true;
                    case "System.Char":
                        knownJavaType = "char";
                        return true;
                    case "System.String":
                        knownJavaType = "String";
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
