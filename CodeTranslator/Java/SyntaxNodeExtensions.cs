// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    static class SyntaxNodeExtensions
    {
        public static string GetJavaModifiersString(this EnumDeclarationSyntax node)
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
                    default:
                        throw new Exception();
                }
                builder.Append(javaModifier);
            }

            return builder.ToString();
        }
    }
}
