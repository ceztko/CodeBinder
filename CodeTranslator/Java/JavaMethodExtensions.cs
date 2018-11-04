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

namespace CodeTranslator.Java
{
    enum JavaMethodFlags
    {
        None = 0,
        Native,
        IsByRef
    }

    static class JavaMethodExtensions
    {
        delegate bool ModifierGetter(string modifier, out string javaModifier);

        public static CodeBuilder BeginBlock(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("}", appendLine);
        }

        public static CodeBuilder BeginParameterList(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("(");
            return builder.Indent(")", false);
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

        public static string GetJavaType(this TypeSyntax type, JavaMethodFlags flags, ICompilationContextProvider provider)
        {
            var symbol = type.GetTypeSymbol(provider);
            return getJavaType(type, symbol, flags);
        }

        public static string GetJavaModifiersString(this BaseMethodDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifierStrings();
            return getJavaModifiersString(modifiers, getJavaMethodModifier);
        }

        public static string GetJavaModifiersString(this BaseTypeDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifierStrings();
            return getJavaModifiersString(modifiers, getJavaTypeModifier);
        }

        private static string getJavaModifiersString(List<string> modifiers, ModifierGetter getJavaModifier)
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

        private static bool getJavaTypeModifier(string modifier, out string javaModifier)
        {
            switch (modifier)
            {
                case "public":
                    javaModifier = "public";
                    return true;
                case "protected":
                    javaModifier = "protected";
                    return true;
                case "private":
                    javaModifier = "private";
                    return true;
                case "internal":
                    javaModifier = null;
                    return false;
                default:
                    throw new Exception();
            }
        }

        private static bool getJavaMethodModifier(string modifier, out string javaModifier)
        {
            switch (modifier)
            {
                case "public":
                    javaModifier = "public";
                    return true;
                case "protected":
                    javaModifier = "protected";
                    return true;
                case "private":
                    javaModifier = "private";
                    return true;
                case "internal":
                    javaModifier = null;
                    return false;
                case "static":
                    javaModifier = "static";
                    return true;
                case "virtual":
                    javaModifier = null;
                    return false;
                case "override":
                    javaModifier = null;
                    return false;
                case "sealed":
                    javaModifier = "final";
                    return true;
                case "extern":
                    javaModifier = "native";
                    return true;
                default:
                    throw new Exception();
            }
        }


        private static string getJavaType(TypeSyntax syntax, ITypeSymbol symbol, JavaMethodFlags flags)
        {
            if (flags.HasFlag(JavaMethodFlags.Native) && symbol.TypeKind == TypeKind.Enum)
                return "int";

            string netFullName;
            string javaArraySuffix;
            if (symbol.TypeKind == TypeKind.Array)
            {
                var arrayType = symbol as IArrayTypeSymbol;
                netFullName = arrayType.ElementType.GetFullName();
                javaArraySuffix = "[]";
            }
            else
            {
                netFullName = symbol.GetFullName();
                javaArraySuffix = string.Empty;
            }

            string javaTypeName;
            if (flags.HasFlag(JavaMethodFlags.IsByRef))
                javaTypeName = getJavaByRefType(syntax, netFullName);
            else
                javaTypeName = getJavaType(syntax, netFullName);

            return javaTypeName + javaArraySuffix;
        }

        static string getJavaType(TypeSyntax syntax, string netFullName)
        {
            switch (netFullName)
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
                    return syntax.GetTypeIdentifier();
            }
        }

        static string getJavaByRefType(TypeSyntax syntax, string netFullName)
        {
            switch (netFullName)
            {
                case "System.Boolean":
                    return "BooleanBox";
                case "System.Char":
                    return "CharBox";
                case "System.Byte":
                    return "ByteBox";
                case "System.SByte":
                    return "ByteBox";
                case "System.Int16":
                    return "ShortBox";
                case "System.UInt16":
                    return "ShortBox";
                case "System.Int32":
                    return "IntBox";
                case "System.UInt32":
                    return "IntBox";
                case "System.Int64":
                    return "LongBox";
                case "System.UInt64":
                    return "LongBox";
                case "System.Single":
                    return "FloatBox";
                case "System.Double":
                    return "DoubleBox";
                case "System.IntPtr":
                    return "LongBox";
                default:
                    return syntax.GetTypeIdentifier();
            }
        }
    }
}
