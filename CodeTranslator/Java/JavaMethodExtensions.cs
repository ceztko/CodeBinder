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

        public static string GetJavaType(this TypeSyntax type, ICompilationContextProvider provider)
        {
            var symbol = type.GetTypeSymbol(provider);
            return getJavaType(symbol);
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


        private static string getJavaType(ITypeSymbol type, bool nativeMethods = false)
        {
            if (nativeMethods && type.TypeKind == TypeKind.Enum)
                return "int";

            string typeName;
            string javaArraySuffix;
            if (type.TypeKind == TypeKind.Array)
            {
                var arrayType = type as IArrayTypeSymbol;

                typeName = arrayType.ElementType.GetFullName();
                javaArraySuffix = "[]";
            }
            else
            {
                typeName = type.GetFullName();
                javaArraySuffix = string.Empty;
            }

            string javaTypeName;
            switch (typeName)
            {
                case "Void":
                {
                    javaTypeName = "void";
                    break;
                }
                case "Object":
                {
                    javaTypeName = "Object";
                    break;
                }
                case "System.IntPtr":
                {
                    javaTypeName = "long";
                    break;
                }
                case "System.Boolean":
                {
                    javaTypeName = "boolean";
                    break;
                }
                case "System.Char":
                {
                    javaTypeName =  "char";
                    break;
                }
                case "System.String":
                {
                    javaTypeName =  "String";
                    break;
                }
                case "System.Byte":
                {
                    javaTypeName = "byte";
                    break;
                }
                case "System.SByte":
                {
                    javaTypeName = "byte";
                    break;
                }
                case "System.Int16":
                {
                    javaTypeName =  "short";
                    break;
                }
                case "System.UInt16":
                {
                    javaTypeName = "short";
                    break;
                }
                case "System.Int32":
                {
                    javaTypeName = "int";
                    break;
                }
                case "System.UInt32":
                {
                    javaTypeName = "int";
                    break;
                }
                case "System.Int64":
                {
                    javaTypeName = "long";
                    break;
                }
                case "System.UInt64":
                {
                    javaTypeName = "long";
                    break;
                }
                case "System.Single":
                {
                    javaTypeName =  "float";
                    break;
                }
                case "System.Double":
                {
                    javaTypeName = "double";
                    break;
                }
                default:
                {
                    javaTypeName = typeName;
                    break;
                }
            }

            return javaTypeName + javaArraySuffix;
        }
    }
}
