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
    static class CSharpTypeContextExtensions
    {
        const string FLAG_ATTRIBUTE_FULLANAME = "System.FlagsAttribute";

        public static bool IsFlag(this CSharpEnumTypeContext node)
        {
            foreach (var attribute in GetAttributes(node))
            {
                var fullName = attribute.GetFullMetadataName(node.TreeContext);
                if (fullName == FLAG_ATTRIBUTE_FULLANAME)
                    return true;
            }
            return false;
        }

        public static IEnumerable<AttributeSyntax> GetAttributes(this CSharpEnumTypeContext type)
        {
            foreach (var list in type.Node.AttributeLists)
            {
                foreach (var attribute in list.Attributes)
                    yield return attribute;
            }
        }

        public static string GetName(this BaseTypeDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }

        public static string GetName(this EnumMemberDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }
    }
}
