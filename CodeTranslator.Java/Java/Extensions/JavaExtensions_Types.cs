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
            writeJavaType(builder, typeSymbol?.GetFullName(), type, typeSymbol, null, false, provider, out isInterface);
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
                    var typeSymbol = (ITypeSymbol)symbol;
                    writeJavaType(builder, typeSymbol?.GetFullName(), syntax, typeSymbol, null, false, provider, out isInterface);
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

        static string getJavaType(string typeName, TypeSyntax syntax, ITypeSymbol symbol, JavaTypeFlags flags, ICompilationContextProvider provider)
        {
            bool isByRef = flags.HasFlag(JavaTypeFlags.IsByRef);
            if (symbol != null && flags.HasFlag(JavaTypeFlags.NativeMethod))
            {
                switch (symbol.TypeKind)
                {
                    case TypeKind.Struct:
                        if (isByRef)
                            return "long";
                        break;
                    case TypeKind.Enum:
                        if (isByRef)
                            return "IntegerBox"; // TODO: Box per gli enum?
                        else
                            return "int";
                }
            }

            var builder = new CodeBuilder();
            bool isInterface;
            writeJavaType(builder, typeName, syntax, symbol, null, isByRef, provider, out isInterface);
            return builder.ToString();
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

        static CodeBuilder append(this CodeBuilder builder, TypeArgumentListSyntax syntax, TypeSyntax parent, ICompilationContextProvider provider)
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

                    append(builder, type, parent, provider);
                }
            }

            return builder;
        }

        static CodeBuilder append(this CodeBuilder builder, TypeSyntax type, TypeSyntax parent,
            ICompilationContextProvider provider)
        {
            bool isInterface;
            var typeSymbol = type.GetTypeSymbol(provider);
            writeJavaType(builder, typeSymbol?.GetFullName(), type, typeSymbol, parent, false, provider, out isInterface);
            return builder;
        }

        static void writeJavaType(CodeBuilder builder, string typeName, TypeSyntax type, ITypeSymbol symbol,
            TypeSyntax parent, bool isByRef, ICompilationContextProvider provider, out bool isInterface)
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
            if (IsKnownJavaType(typeName, isByRef, parent, out javaTypeName, out isInterface))
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
                        builder.Append(javaTypeName).append(arrayType.TypeArgumentList, type, provider);
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
                        string boxTypeName;
                        if (getJavaBoxType(javaTypeName, out boxTypeName))
                            builder.Append("boxTypeName");
                        else
                            throw new Exception();
                        break;
                    }
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.IdentifierName:
                        builder.Append(javaTypeName);
                        break;
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
                        builder.append(arrayType.ElementType, type, provider).Append("[]");
                        break;
                    }
                    case SyntaxKind.GenericName:
                    {
                        var genericType = type as GenericNameSyntax;
                        builder.Append(genericType.GetName()).append(genericType.TypeArgumentList, type, provider);
                        break;
                    }
                    case SyntaxKind.NullableType:
                    {
                        Debug.Assert(symbol != null);
                        var nullableType = type as NullableTypeSyntax;
                        switch (symbol.TypeKind)
                        {
                            case TypeKind.Struct:
                                builder.append(nullableType.ElementType, type, provider);
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
                        var qualifiedName = type as QualifiedNameSyntax;
                        builder.append(qualifiedName.Left, type, provider).Dot().append(qualifiedName.Right, type, provider);
                        break;
                    }
                    default:
                        throw new Exception();
                }

                isInterface = symbol?.TypeKind == TypeKind.Interface;
            }
        }

        public static bool IsKnownJavaType(string typeName, bool isByRef, TypeSyntax parent,
            out string knownJavaType, out bool isInterface)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                knownJavaType = null;
                isInterface = false;
                return false;
            }

            if (IsKnowSimpleJavaType(typeName, isByRef, parent, out knownJavaType))
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
                case "System.Collections.Generic.KeyValuePair<TKey, TValue>":
                {
                    knownJavaType = "Map.Entry";
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

        public static bool IsKnowSimpleJavaType(string typeName, bool isByRef, TypeSyntax parent, out string knownJavaType)
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
            else if (parent?.Kind() == SyntaxKind.GenericName)
            {
                switch (typeName)
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
                    case "System.Char":
                        knownJavaType = "Character";
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
                switch (typeName)
                {
                    case "System.Void":
                        knownJavaType = "void";
                        return true;
                    case "System.Object":
                        knownJavaType = "Object";
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
                    case "System.Char":
                        knownJavaType = "char";
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

        static bool getJavaBoxType(string javaTypeName, out string boxTypeName)
        {
            switch (javaTypeName)
            {
                case "boolean":
                    boxTypeName = "Boolean";
                    return true;
                case "char":
                    boxTypeName = "Character";
                    return true;
                case "byte":
                    boxTypeName = "Byte";
                    return true;
                case "short":
                    boxTypeName = "Short";
                    return true;
                case "int":
                    boxTypeName = "Integer";
                    return true;
                case "long":
                    boxTypeName = "Long";
                    return true;
                case "float":
                    boxTypeName = "Float";
                    return true;
                case "double":
                    boxTypeName = "Double";
                    return true;
                default:
                    boxTypeName = null;
                    return false;
            }
        }
    }
}
