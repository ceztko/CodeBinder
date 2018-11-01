// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;

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
    }
}
