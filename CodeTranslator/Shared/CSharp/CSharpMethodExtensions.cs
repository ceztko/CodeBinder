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
        public static string GetTypeName(ref this MethodParameterInfo parameter, out ITypeSymbol typeSymbol)
        {
            if (parameter.Type.Type == null)
            {
                typeSymbol = null; // TODO: Lookup for proper System.Void ITypeSymbol?
                return "System.Void";
            }

            string constantTypeName = parameter.Type.Type.GetFullName();
            switch (constantTypeName)
            {
                case "System.String":
                {
                    typeSymbol = null;
                    return parameter.Type.Value.ToString();
                }
                case "System.Type":
                {
                    typeSymbol = parameter.Type.Value as ITypeSymbol;
                    return typeSymbol.GetFullName();
                }
                default:
                    throw new Exception();
            }
        }

        public static MethodSignatureInfo[] GetMethodSignatures(this MethodDeclarationSyntax method, ICompilationContextProvider provider)
        {
            var ret = new List<MethodSignatureInfo>();
            var attributes = method.GetAttributes(provider);
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<SignatureAttribute>())
                {
                    var methodData = getMethodDataFromConstructorParameter(method, attribute);
                    ret.Add(methodData);
                }
            }
            return ret.ToArray();
        }

        private static MethodSignatureInfo getMethodDataFromConstructorParameter(MethodDeclarationSyntax method, AttributeData attribute)
        {
            if (attribute.ConstructorArguments.Length != 1)
                throw new Exception("SignatureAttribute must be constructed with single parameter");

            var constructorParam = attribute.ConstructorArguments[0];
            string constructorParamTypeName = constructorParam.Type.GetFullName();
            MethodParameterInfo[] parameters;
            switch (constructorParamTypeName)
            {
                case "System.Object[]":
                {
                    if (constructorParam.Values.Length % 2 != 0)
                        throw new Exception("Object count with parameters name must be divisible by two");

                    int parameterCount = constructorParam.Values.Length / 2;
                    parameters = new MethodParameterInfo[parameterCount];
                    for (int i = 0; i < parameterCount; i++)
                    {
                        var typeConstant = constructorParam.Values[i * 2];
                        string parameterName = constructorParam.Values[i * 2 + 1].Value as string;
                        if (parameterName == null)
                            throw new Exception("Parameter name must be a string");

                        parameters[i] = new MethodParameterInfo(typeConstant, parameterName);
                    }

                    break;
                }
                case "System.Type[]":
                {
                    if (method.ParameterList.Parameters.Count != constructorParam.Values.Length)
                        throw new Exception("Method parameter count must be same as provided type count");

                    int parameterCount = constructorParam.Values.Length;
                    parameters = new MethodParameterInfo[parameterCount];
                    for (int i = 0; i < parameterCount; i++)
                    {
                        var typeConstant = constructorParam.Values[i];
                        string parameterName = method.ParameterList.Parameters[i].Identifier.Text;
                        parameters[i] = new MethodParameterInfo(typeConstant, parameterName);
                    }
                    break;
                }
                default:
                    throw new Exception();
            }

            TypedConstant returnType = new TypedConstant();
            string methodName = method.Identifier.Text;
            foreach (var namedArgument in attribute.NamedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "ReturnType":
                    {
                        returnType = namedArgument.Value;
                        break;
                    }
                    case "MethodName":
                    {
                        methodName = namedArgument.Value.Value?.ToString() ?? methodName;
                        break;
                    }
                    default:
                        throw new Exception();
                }
            }

            var ret = new MethodSignatureInfo();
            ret.MethodName = methodName;
            ret.Modifiers = method.GetCSharpModifiers();
            ret.ReturnType = new MethodParameterInfo(returnType, null);
            ret.Parameters = parameters;
            return ret;
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

        public static SyntaxKind[] GetCSharpModifiers(this BaseFieldDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        explicitAccessibility = true;
                        break;
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.ConstKeyword:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<SyntaxKind>();

            if (!explicitAccessibility)
                ret.Add(SyntaxKind.InternalKeyword);

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Kind());

            return ret.ToArray();
        }

        public static SyntaxKind[] GetCSharpModifiers(this BaseTypeDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        explicitAccessibility = true;
                        break;
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.StaticKeyword:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<SyntaxKind>();

            if (!explicitAccessibility)
                ret.Add(SyntaxKind.InternalKeyword);

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Kind());

            return ret.ToArray();
        }

        public static SyntaxKind[] GetCSharpModifiers(this BaseMethodDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        explicitAccessibility = true;
                        break;
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.ExternKeyword:
                    case SyntaxKind.NewKeyword:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<SyntaxKind>();

            if (!explicitAccessibility)
                ret.Add(SyntaxKind.PrivateKeyword);

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Kind());

            return ret.ToArray();
        }

        public static SyntaxKind[] GetCSharpModifiers(this BasePropertyDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        explicitAccessibility = true;
                        break;
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.NewKeyword:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<SyntaxKind>();

            if (!explicitAccessibility)
                ret.Add(SyntaxKind.PrivateKeyword);

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Kind());

            return ret.ToArray();
        }

        public static SyntaxKind[] GetCSharpModifiers(this AccessorDeclarationSyntax node)
        {
            var ret = new List<SyntaxKind>();
            foreach (var modifier in node.Modifiers)
            {
                var kind = modifier.Kind();
                switch (kind)
                {
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        ret.Add(kind);
                        break;
                    default:
                        throw new Exception();
                }
            }

            return ret.ToArray();
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
