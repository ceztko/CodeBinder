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

namespace CodeTranslator.Java
{
    enum JavaTypeFlags
    {
        None = 0,
        NativeMethod,
        IsByRef
    }

    static class JavaExtensions
    {
        delegate bool ModifierGetter(SyntaxKind modifier, out string javaModifier);

        public static string GetJavaType(this TypeSyntax type, ICompilationContextProvider provider, out bool isInterface)
        {
            var typeSymbol = type.GetTypeSymbol(provider);
            string fullName = typeSymbol.GetFullName();
            string javaTypeName;
            if (IsKnownType(fullName, out javaTypeName, out isInterface))
            {

            }
            else
            {
                var typeInfo = type.GetTypeInfo(provider);
                isInterface = typeSymbol.TypeKind == TypeKind.Interface;

                var kind = type.Kind();
                switch (kind)
                {
                    case SyntaxKind.IdentifierName:
                    {
                        javaTypeName = (type as IdentifierNameSyntax).GetName();
                        break;
                    }
                    default:
                        javaTypeName =  "NULL";
                        break;
                }
            }

            return javaTypeName;
        }

        static bool IsKnownType(string typeName, out string convertedKnowType, out bool isInterface)
        {
            switch (typeName)
            {
                case "System.IDisposable":
                {
                    convertedKnowType = "AutoCloseable";
                    isInterface = true;
                    return true;
                }
                default:
                {
                    convertedKnowType = null;
                    isInterface = false;
                    return false;
                }
            }
        }

        public static string GetJavaTypeDeclaration(this BaseTypeDeclarationSyntax node)
        {
            switch (node.GetType().Name)
            {
                case nameof(InterfaceDeclarationSyntax):
                    return "interface";
                case nameof(ClassDeclarationSyntax):
                    return "class";
                case nameof(StructDeclarationSyntax):
                    return "class";
                case nameof(EnumDeclarationSyntax):
                    return "enum";
                default:
                    throw new Exception("Unsupported");
            }
        }

        public static string GetJavaTypeName(ref this MethodParameterInfo parameter, JavaTypeFlags flags)
        {
            ITypeSymbol typeSymbol;
            string typeName = parameter.GetTypeName(out typeSymbol);
            return getJavaType(typeName, null, typeSymbol, flags);
        }

        public static string GetJavaType(this TypeSyntax type, ICompilationContextProvider provider)
        {
            return GetJavaType(type, JavaTypeFlags.None, provider);
        }

        public static string GetJavaType(this TypeSyntax type, JavaTypeFlags flags, ICompilationContextProvider provider)
        {
            var symbol = type.GetTypeSymbol(provider);
            return getJavaType(symbol.GetFullName(), type, symbol, flags);
        }

        public static string GetJavaModifiersString(this FieldDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return GetJavaFieldModifiersString(modifiers);
        }

        public static string GetJavaModifiersString(this BaseTypeDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return GetJavaTypeModifiersString(modifiers);
        }

        public static string GetJavaModifiersString(this BaseMethodDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return GetJavaMethodModifiersString(modifiers);
        }

        public static string GetJavaModifiersString(this BasePropertyDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return GetJavaPropertyModifiersString(modifiers);
        }

        public static string GetJavaModifiersString(this MethodSignatureInfo signature)
        {
            return GetJavaMethodModifiersString(signature.Modifiers);
        }

        public static string GetJavaFieldModifiersString(IEnumerable<SyntaxKind> modifiers)
        {
            return getJavaModifiersString(modifiers, getJavaFieldModifier);
        }

        public static string GetJavaTypeModifiersString(IEnumerable<SyntaxKind> modifiers)
        {
            return getJavaModifiersString(modifiers, getJavaTypeModifier);
        }

        public static string GetJavaMethodModifiersString(IEnumerable<SyntaxKind> modifiers)
        {
            return getJavaModifiersString(modifiers, getJavaMethodModifier);
        }

        public static string GetJavaPropertyModifiersString(IEnumerable<SyntaxKind> modifiers)
        {
            return getJavaModifiersString(modifiers, getJavaMethodModifier);
        }

        private static string getJavaModifiersString(IEnumerable<SyntaxKind> modifiers, ModifierGetter getJavaModifier)
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var modifier in modifiers)
            {
                string javaModifier;
                if (!getJavaModifier(modifier, out javaModifier))
                    continue;

                if (first)
                    first = false;
                else
                    builder.Append(" ");

                builder.Append(javaModifier);
            }

            return builder.ToString();
        }

        private static bool getJavaFieldModifier(SyntaxKind modifier, out string javaModifier)
        {
            switch (modifier)
            {
                case SyntaxKind.PublicKeyword:
                    javaModifier = "public";
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    javaModifier = "protected";
                    return true;
                case SyntaxKind.PrivateKeyword:
                    javaModifier = "private";
                    return true;
                case SyntaxKind.ReadOnlyKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.ConstKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.NewKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.InternalKeyword:
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
            }
        }

        private static bool getJavaTypeModifier(SyntaxKind modifier, out string javaModifier)
        {
            switch (modifier)
            {
                case SyntaxKind.PublicKeyword:
                    javaModifier = "public";
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    javaModifier = "protected";
                    return true;
                case SyntaxKind.PrivateKeyword:
                    javaModifier = "private";
                    return true;
                case SyntaxKind.SealedKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.AbstractKeyword:
                    javaModifier = "abstract";
                    return true;
                case SyntaxKind.InternalKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.StaticKeyword:
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
            }
        }

        private static bool getJavaMethodModifier(SyntaxKind modifier, out string javaModifier)
        {
            switch (modifier)
            {
                case SyntaxKind.PublicKeyword:
                    javaModifier = "public";
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    javaModifier = "protected";
                    return true;
                case SyntaxKind.PrivateKeyword:
                    javaModifier = "private";
                    return true;
                case SyntaxKind.StaticKeyword:
                    javaModifier = "static";
                    return true;
                case SyntaxKind.SealedKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.ExternKeyword:
                    javaModifier = "native";
                    return true;
                case SyntaxKind.AbstractKeyword:
                    javaModifier = "abstract";
                    return true;
                case SyntaxKind.NewKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.InternalKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.VirtualKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.OverrideKeyword:
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
            }
        }

        private static bool getJavaPropertyModifier(SyntaxKind modifier, out string javaModifier)
        {
            switch (modifier)
            {
                case SyntaxKind.PublicKeyword:
                    javaModifier = "public";
                    return true;
                case SyntaxKind.ProtectedKeyword:
                    javaModifier = "protected";
                    return true;
                case SyntaxKind.PrivateKeyword:
                    javaModifier = "private";
                    return true;
                case SyntaxKind.StaticKeyword:
                    javaModifier = "static";
                    return true;
                case SyntaxKind.SealedKeyword:
                    javaModifier = "final";
                    return true;
                case SyntaxKind.AbstractKeyword:
                    javaModifier = "abstract";
                    return true;
                case SyntaxKind.NewKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.InternalKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.VirtualKeyword:
                    javaModifier = null;
                    return false;
                case SyntaxKind.OverrideKeyword:
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
            }
        }

        private static string getJavaType(string typeName, TypeSyntax syntax, ITypeSymbol symbol, JavaTypeFlags flags)
        {
            if (symbol?.TypeKind == TypeKind.Enum)
            {
                if (flags.HasFlag(JavaTypeFlags.IsByRef))
                    return "IntegerBox"; // TODO: Box per gli enum?
                else
                    return "int";
            }

            string javaTypeSuffix = string.Empty;
            if (symbol?.TypeKind == TypeKind.Array)
            {
                var arrayType = symbol as IArrayTypeSymbol;
                typeName = arrayType.ElementType.GetFullName();
                javaTypeSuffix = "[]";
            }

            string javaTypeName;
            if (flags.HasFlag(JavaTypeFlags.IsByRef))
                javaTypeName = getJavaByRefType(typeName, symbol, syntax);
            else
                javaTypeName = getJavaType(typeName, syntax);

            return javaTypeName + javaTypeSuffix;
        }

        static string getJavaType(string typeName, TypeSyntax syntax)
        {
            switch (typeName)
            {
                case "System.Void":
                    return "void";
                case "System.Object":
                    return "Object";
                case "System.IntPtr":
                    return "long";
                case "System.Boolean":
                    return "boolean";
                case "System.Char":
                    return "char";
                case "System.String":
                    return "String";
                case "System.Byte":
                    return "byte";
                case "System.SByte":
                    return "byte";
                case "System.Int16":
                    return "short";
                case "System.UInt16":
                    return "short";
                case "System.Int32":
                    return "int";
                case "System.UInt32":
                    return "int";
                case "System.Int64":
                    return "long";
                case "System.UInt64":
                    return "long";
                case "System.Single":
                    return "float";
                case "System.Double":
                    return "double";
                default:
                {
                    return syntax?.GetTypeIdentifier() ?? typeName;
                }
            }
        }

        static string getJavaByRefType(string typeName, ITypeSymbol symbol, TypeSyntax syntax)
        {
            switch (typeName)
            {
                case "System.Boolean":
                    return "BooleanBox";
                case "System.Char":
                    return "CharacterBox";
                case "System.Byte":
                    return "ByteBox";
                case "System.SByte":
                    return "ByteBox";
                case "System.Int16":
                    return "ShortBox";
                case "System.UInt16":
                    return "ShortBox";
                case "System.Int32":
                    return "IntegerBox";
                case "System.UInt32":
                    return "IntegerBox";
                case "System.Int64":
                    return "LongBox";
                case "System.UInt64":
                    return "LongBox";
                case "System.Single":
                    return "FloatBox";
                case "System.Double":
                    return "DoubleBox";
                case "System.String":
                    return "StringBox";
                case "System.IntPtr":
                    return "LongBox";
                default:
                {
                    if (symbol?.TypeKind == TypeKind.Struct)
                        return "long";
                    else
                        return "NULL";
                        //throw new Exception("Unsupported by ref type " + syntax?.GetTypeIdentifier() ?? typeName);
                }
            }
        }

        public static string ToJavaCase(this string text)
        {
            if (string.IsNullOrEmpty(text) || char.IsLower(text, 0))
                return text;

            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }
    }
}
