// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CodeTranslator.Shared.CSharp
{
    static class CSharpMethodExtensions
    {
        const string FLAG_ATTRIBUTE_FULLANAME = "System.FlagsAttribute";

        public static bool IsFlag(this EnumDeclarationSyntax node, ICompilationContextProvider provider)
        {
            foreach (var attribute in GetAttributes(node))
            {
                var fullName = attribute.GetFullName(provider);
                if (fullName == FLAG_ATTRIBUTE_FULLANAME)
                    return true;
            }
            return false;
        }

        public static int GetEnumValue(this EnumMemberDeclarationSyntax node, ICompilationContextProvider provider)
        {
            return node.EqualsValue.Value.GetValue<int>(provider);
        }

        public static List<string> GetCSharpModifierStrings(this BaseTypeDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Text)
                {
                    case "public":
                    case "internal":
                    case "protected":
                    case "private":
                        explicitAccessibility = true;
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<string>();

            if (!explicitAccessibility)
                ret.Add("internal");

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Text);

            return ret;
        }

        public static List<string> GetCSharpModifierStrings(this BaseMethodDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Text)
                {
                    case "public":
                    case "internal":
                    case "protected":
                    case "private":
                        explicitAccessibility = true;
                        break;
                    case "static":
                    case "virtual":
                    case "override":
                    case "sealed":
                    case "extern":
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<string>();

            if (!explicitAccessibility)
                ret.Add("private");

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Text);

            return ret;
        }

        public static IEnumerable<AttributeSyntax> GetAttributes(this EnumDeclarationSyntax node)
        {
            foreach (var list in node.AttributeLists)
            {
                foreach (var attribute in list.Attributes)
                    yield return attribute;
            }
        }

        public static TypeInfo GetTypeInfo(this BaseTypeSyntax type, ICompilationContextProvider provider)
        {
            return type.Type.GetTypeInfo(provider);
        }

        public static string GetFullName(this BaseTypeSyntax type, ICompilationContextProvider provider)
        {
            return type.Type.GetFullName(provider);
        }

        public static string GetName(this BaseTypeSyntax type)
        {
            return (type.Type as IdentifierNameSyntax).GetName();
        }

        public static string GetName(this IdentifierNameSyntax node)
        {
            return node.Identifier.Text;
        }

        public static string GetName(this BaseTypeDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }

        public static string GetName(this EnumMemberDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }

        public static string GetName(this MethodDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }
    }
}
