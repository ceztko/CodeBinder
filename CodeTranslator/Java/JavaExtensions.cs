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
            return getJavaModifiersString(node.Modifiers);
        }

        public static string GetJavaModifiersString(this BaseTypeDeclarationSyntax node)
        {
            return getJavaModifiersString(node.Modifiers);
        }

        private static string getJavaModifiersString(this SyntaxTokenList tokenList)
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var token in tokenList)
            {
                if (!first)
                {
                    builder.Append(" ");
                    first = false;
                }

                string javaModifier;
                switch (token.Text)
                {
                    case "public":
                        javaModifier = "public";
                        break;
                    case "protected":
                        javaModifier = "protected";
                        break;
                    case "private":
                        javaModifier = "private";
                        break;
                    case "internal":
                        javaModifier = "";
                        break;
                    // TODO: fare una cosa specifica per tipi e per metodi?
                    case "virtual":
                        javaModifier = "";
                        break;
                    case "static":
                        javaModifier = "static";
                        break;
                    case "override":
                        javaModifier = "";
                        break;
                    case "extern":
                        javaModifier = "native";
                        break;
                    default:
                        throw new Exception();
                }
                builder.Append(javaModifier);
            }

            return builder.ToString();
        }
    }
}
