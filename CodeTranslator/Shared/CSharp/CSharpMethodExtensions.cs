// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using CodeTranslator.Attributes;

namespace CodeTranslator.Shared.CSharp
{
    static class CSharpMethodExtensions
    {
        public static List<MethodSignature> GetMethodSignatures(this MethodDeclarationSyntax method, ICompilationContextProvider provider)
        {
            var ret = new List<MethodSignature>();
            var attributes = method.GetAttributes(provider);
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<SignatureAttribute>())
                {
                    var methodData = getMethodDataFromConstructorParameter(method, attribute.ConstructorArguments[0]);
                    ret.Add(methodData);
                }
            }
            return ret;
        }

        private static MethodSignature getMethodDataFromConstructorParameter(MethodDeclarationSyntax method, TypedConstant constant)
        {
            string constructorParamTypeName = constant.Type.GetFullName();
            var parameters = new List<string>();
            switch (constructorParamTypeName)
            {
                case "System.Object[]":
                {
                    foreach (var param in constant.Values)
                    {
                        string paramTypeName = param.Type.GetFullName();
                        switch (paramTypeName)
                        {
                            case "System.String":
                            {
                                parameters.Add(param.Value.ToString());
                                break;
                            }
                            case "System.Type":
                            {
                                var type = param.Value as ITypeSymbol;
                                parameters.Add(type.GetFullName());
                                break;
                            }
                            default:
                                throw new Exception();
                        }
                    }
                    break;
                }
                case "System.Type[]":
                {
                    if (method.ParameterList.Parameters.Count != constant.Values.Length)
                        throw new Exception("Method parameter count must be same as provided type count");

                    for (int i = 0; i < constant.Values.Length; i++)
                    {
                        var type = constant.Values[0].Value as ITypeSymbol;
                        parameters.Add(type.GetFullName());
                        parameters.Add(method.ParameterList.Parameters[i].Identifier.Text);
                    }
                    break;
                }
                default:
                    throw new Exception();
            }

            return MethodSignature.CreateFromParamStrings(method.Identifier.Text, null, parameters);
        }

        public static string GetTypeIdentifier(this TypeSyntax type)
        {
            var kind = type.Kind();
            switch (kind)
            {
                case SyntaxKind.PredefinedType:
                    return (type as PredefinedTypeSyntax).Keyword.Text;
                case SyntaxKind.IdentifierName:
                    return (type as IdentifierNameSyntax).Identifier.Text;
                case SyntaxKind.NullableType:
                    return (type as NullableTypeSyntax).ElementType.GetTypeIdentifier();
                default:
                    return "NULL";
            }
        }

        public static bool IsNative(this MethodDeclarationSyntax method, ICompilationContextProvider provider)
        {
            if (!method.HasAttribute<DllImportAttribute>(provider))
                return false;

            return method.Modifiers.HasModifier("extern");
        }

        public static bool IsFlag(this EnumDeclarationSyntax node, ICompilationContextProvider provider)
        {
            return node.HasAttribute<FlagsAttribute>(provider);
        }

        public static bool IsRef(this ParameterSyntax parameter)
        {
            return parameter.Modifiers.HasModifier("ref");
        }

        public static bool IsOut(this ParameterSyntax parameter)
        {
            return parameter.Modifiers.HasModifier("out");
        }

        public static int GetEnumValue(this EnumMemberDeclarationSyntax node, ICompilationContextProvider provider)
        {
            return node.EqualsValue.Value.GetValue<int>(provider);
        }

        public static List<string> GetCSharpModifierStrings(this BaseFieldDeclarationSyntax node)
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
                    case "new":
                    case "const":
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
                    case "abstract":
                    case "static":
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
                    case "abstract":
                    case "override":
                    case "sealed":
                    case "extern":
                    case "new":
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

        public static List<string> GetCSharpModifierStrings(this BasePropertyDeclarationSyntax node)
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
                    case "abstract":
                    case "override":
                    case "sealed":
                    case "new":
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

        public static List<string> GetCSharpModifierStrings(this AccessorDeclarationSyntax node)
        {
            var ret = new List<string>();
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Text)
                {
                    case "internal":
                    case "protected":
                    case "private":
                        ret.Add(modifier.Text);
                        break;
                    default:
                        throw new Exception();
                }
            }

            return ret;
        }

        public static bool HasModifier(this SyntaxTokenList modifiers, string modifierStr)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier.Text == modifierStr)
                    return true;
            }

            return false;
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
            var idenfitifier = type.Type as IdentifierNameSyntax;
            if (idenfitifier != null)
                return idenfitifier.GetName();

            return "NULL";
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
