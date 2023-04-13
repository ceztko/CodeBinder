// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using CodeBinder.Java.Shared;
using System.Diagnostics.CodeAnalysis;

namespace CodeBinder.Java
{
    static partial class JavaExtensions
    {
        static Dictionary<string, Dictionary<string, SymbolReplacement>> _replacements;

        static JavaExtensions()
        {
            _replacements = new Dictionary<string, Dictionary<string, SymbolReplacement>>()
            {
                // java.lang.Object
                { "System.Object", new Dictionary<string, SymbolReplacement>() {
                    { "GetHashCode", new SymbolReplacement() { Name = "hashCode", Kind = SymbolReplacementKind.Method } },
                    { "Equals", new SymbolReplacement() { Name = "equals", Kind = SymbolReplacementKind.Method } },
                    { "Clone", new SymbolReplacement() { Name = "clone", Kind = SymbolReplacementKind.Method } },
                    { "ToString", new SymbolReplacement() { Name = "toString", Kind = SymbolReplacementKind.Method } },
                } },
                // java.lang.String
                { "System.String", new Dictionary<string, SymbolReplacement>() {
                    { "op_Equality", new SymbolReplacement() { Name = "CodeBinder.Java.BinderUtils.equals", Kind = SymbolReplacementKind.StaticMethod } },
                    { "op_Inequality", new SymbolReplacement() { Name = "CodeBinder.Java.BinderUtils.equals", Kind = SymbolReplacementKind.StaticMethod, Negate = true } },
                } },
                { "System.IntPtr", new Dictionary<string, SymbolReplacement>() {
                    { "Zero", new SymbolReplacement() { Name = "0", Kind = SymbolReplacementKind.Literal } },
                } },
                { "System.Array", new Dictionary<string, SymbolReplacement>() {
                    { "Length", new SymbolReplacement() { Name = "length", Kind = SymbolReplacementKind.Field } },
                } },
                // java.lang.AutoCloseable
                { "System.Collections.Generic.List<T>", new Dictionary<string, SymbolReplacement>() {
                    { "Add", new SymbolReplacement() { Name = "add", Kind = SymbolReplacementKind.Method } },
                    { "Clear", new SymbolReplacement() { Name = "clear", Kind = SymbolReplacementKind.Method } },
                } },
                // java.lang.AutoCloseable
                { "System.IDisposable", new Dictionary<string, SymbolReplacement>() {
                    { "Dispose", new SymbolReplacement() { Name = "close", Kind = SymbolReplacementKind.Method } }
                } },
                // java.lang.Iterable<T>
                { "System.Collections.Generic.IEnumerable<out T>", new Dictionary<string, SymbolReplacement>() {
                    { "GetEnumerator", new SymbolReplacement() { Name = "iterator", Kind = SymbolReplacementKind.Method } }
                } },
            };
        }

        public static bool HasJavaReplacement(this IMethodSymbol methodSymbol, [NotNullWhen(true)]out SymbolReplacement? javaReplacement)
        {
            var containingType = methodSymbol.ContainingType;
            Dictionary<string, SymbolReplacement>? replacements;
            foreach (var iface in containingType.AllInterfaces)
            {
                string ifaceName = iface.GetFullName();
                if (_replacements.TryGetValue(ifaceName, out replacements))
                {
                    foreach (var member in iface.GetMembers())
                    {
                        if (member.Kind != SymbolKind.Method)
                            continue;

                        if (replacements.TryGetValue(methodSymbol.Name, out var replacement))
                        {
                            if (SymbolEqualityComparer.Default.Equals(containingType.FindImplementationForInterfaceMember(member), methodSymbol))
                            {
                                javaReplacement = replacement;
                                return true;
                            }
                        }
                    }
                }
            }

            string containingTypeFullname;
            if (methodSymbol.OverriddenMethod == null)
                containingTypeFullname = containingType.GetFullName();
            else
                containingTypeFullname = methodSymbol.GetFirstDeclaringType().GetFullName();

            if (_replacements.TryGetValue(containingTypeFullname, out replacements))
            {
                if (replacements.TryGetValue(methodSymbol.Name, out var replacement))
                {
                    javaReplacement = replacement;
                    return true;
                }
            }

            javaReplacement = null;
            return false;
        }

        public static bool HasJavaReplacement(this IPropertySymbol propertySymbol, [NotNullWhen(true)]out SymbolReplacement? javaReplacement)
        {
            // TODO: look for interface/overridden class
            if (_replacements.TryGetValue(propertySymbol.ContainingType.GetFullName(), out var replacements))
                return replacements.TryGetValue(propertySymbol.Name, out javaReplacement);

            javaReplacement = null;
            return false;
        }

        public static bool HasJavaReplacement(this IFieldSymbol fieldSymbol, [NotNullWhen(true)]out SymbolReplacement? replacement)
        {
            if (_replacements.TryGetValue(fieldSymbol.ContainingType.GetFullName(), out var replacements))
                return replacements.TryGetValue(fieldSymbol.Name, out replacement);

            replacement = null;
            return false;
        }

        public static string GetJavaOperator(this AssignmentExpressionSyntax syntax)
        {
            var op = syntax.Kind();
            switch (op)
            {
                case SyntaxKind.AddAssignmentExpression:
                    return "+=";
                case SyntaxKind.AndAssignmentExpression:
                    return "&=";
                case SyntaxKind.DivideAssignmentExpression:
                    return "/=";
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    return "^=";
                case SyntaxKind.LeftShiftAssignmentExpression:
                    return "<<=";
                case SyntaxKind.ModuloAssignmentExpression:
                    return "%=";
                case SyntaxKind.MultiplyAssignmentExpression:
                    return "*=";
                case SyntaxKind.OrAssignmentExpression:
                    return "|=";
                case SyntaxKind.RightShiftAssignmentExpression:
                    return ">>=";
                case SyntaxKind.SimpleAssignmentExpression:
                    return "=";
                case SyntaxKind.SubtractAssignmentExpression:
                    return "-=";
                default:
                    throw new Exception();
            }
        }

        public static string GetJavaOperator(this BinaryExpressionSyntax syntax)
        {
            var op = syntax.Kind();
            switch (op)
            {
                case SyntaxKind.AddExpression:
                    return "+";
                case SyntaxKind.SubtractExpression:
                    return "-";
                case SyntaxKind.MultiplyExpression:
                    return "*";
                case SyntaxKind.DivideExpression:
                    return "/";
                case SyntaxKind.ModuloExpression:
                    return "%";
                case SyntaxKind.LeftShiftExpression:
                    return "<<";
                case SyntaxKind.RightShiftExpression:
                    return ">>";
                case SyntaxKind.LogicalOrExpression:
                    return "||";
                case SyntaxKind.LogicalAndExpression:
                    return "&&";
                case SyntaxKind.BitwiseOrExpression:
                    return "|";
                case SyntaxKind.BitwiseAndExpression:
                    return "&";
                case SyntaxKind.ExclusiveOrExpression:
                    return "^";
                case SyntaxKind.EqualsExpression:
                    return "==";
                case SyntaxKind.NotEqualsExpression:
                    return "!=";
                case SyntaxKind.LessThanExpression:
                    return "<";
                case SyntaxKind.LessThanOrEqualExpression:
                    return ">";
                case SyntaxKind.GreaterThanExpression:
                    return ">";
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return ">=";
                case SyntaxKind.IsExpression:
                    return "instanceof";
                // Unsupported
                case SyntaxKind.AsExpression:   // NOTE: Unsupported as an operator
                case SyntaxKind.CoalesceExpression:
                default:
                    throw new Exception();
            }
        }

        public static string GetJavaOperator(this PrefixUnaryExpressionSyntax syntax)
        {
            var op = syntax.Kind();
            switch (op)
            {
                case SyntaxKind.UnaryPlusExpression:
                    return "+";
                case SyntaxKind.UnaryMinusExpression:
                    return "-";
                case SyntaxKind.BitwiseNotExpression:
                    return "~";
                case SyntaxKind.LogicalNotExpression:
                    return "!";
                case SyntaxKind.PreIncrementExpression:
                    return "++";
                case SyntaxKind.PreDecrementExpression:
                    return "--";
                // Unsupported
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                default:
                    throw new Exception();
            }
        }

        public static string GetJavaOperator(this PostfixUnaryExpressionSyntax syntax)
        {
            var op = syntax.Kind();
            switch (op)
            {
                case SyntaxKind.PostIncrementExpression:
                    return "++";
                case SyntaxKind.PostDecrementExpression:
                    return "--";
                default:
                    throw new Exception();
            }
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

        public static string GetJavaModifiersString(this FieldDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return JavaUtils.GetFieldModifiersString(modifiers);
        }

        public static string GetJavaModifiersString(this BaseTypeDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return JavaUtils.GetTypeModifiersString(modifiers);
        }

        public static string GetJavaModifiersString(this BaseMethodDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return JavaUtils.GetMethodModifiersString(modifiers);
        }

        public static string GetJavaModifiersString(this BasePropertyDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return JavaUtils.GetPropertyModifiersString(modifiers);
        }

        /// <summary>
        /// Lowercase the first character of the identifier
        /// </summary>
        public static string ToJavaLowerCase(this string text)
        {
            if (string.IsNullOrEmpty(text) || char.IsLower(text, 0))
                return text;

            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }
    }
}
