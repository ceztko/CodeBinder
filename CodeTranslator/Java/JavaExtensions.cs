// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    static class JavaExtensions
    {
        delegate bool ModifierGetter(string modifier, out string javaModifier);

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
            return getJavaModifiersString(node.Modifiers, getJavaMethodModifier);
        }

        public static string GetJavaModifiersString(this BaseTypeDeclarationSyntax node)
        {
            return getJavaModifiersString(node.Modifiers, getJavaTypeModifier);
        }

        private static string getJavaModifiersString(this SyntaxTokenList tokenList, ModifierGetter getJavaModifier)
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var token in tokenList)
            {
                string javaModifier;
                if (!getJavaModifier(token.Text, out javaModifier))
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
                    throw new Exception("Unsupported");
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
                    throw new Exception("Unsupported");
            }
        }
    }
}
