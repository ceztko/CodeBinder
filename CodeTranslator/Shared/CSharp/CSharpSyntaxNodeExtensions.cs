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
    static class CSharpSyntaxNodeExtensions
    {
        const string FLAG_ATTRIBUTE_FULLANAME = "System.FlagsAttribute";

        public static bool IsFlag(this EnumDeclarationSyntax node, ISemanticModelProvider provider)
        {
            foreach (var attribute in GetAttributes(node))
            {
                var fullName = attribute.GetFullMetadataName(provider);
                if (fullName == FLAG_ATTRIBUTE_FULLANAME)
                    return true;
            }
            return false;
        }

        public static int GetEnumValue(this EnumMemberDeclarationSyntax node, ISemanticModelProvider provider)
        {
            return node.EqualsValue.Value.GetValue<int>(provider);
        }

        public static IEnumerable<AttributeSyntax> GetAttributes(this EnumDeclarationSyntax node)
        {
            foreach (var list in node.AttributeLists)
            {
                foreach (var attribute in list.Attributes)
                    yield return attribute;
            }
        }

        public static TypeInfo GetTypeInfo(this BaseTypeSyntax type, ISemanticModelProvider provider)
        {
            return type.Type.GetTypeInfo(provider);
        }

        public static string GetFullMetadataName(this BaseTypeSyntax type, ISemanticModelProvider provider)
        {
            return type.Type.GetFullMetadataName(provider);
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
