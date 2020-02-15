// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics.CodeAnalysis;
using CodeBinder.Attributes;
using System.Diagnostics;

namespace CodeBinder.Apple
{
    static partial class ObjCExtensions
    {
        static Dictionary<string, Dictionary<string, SymbolReplacement>> _replacements;

        static ObjCExtensions()
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
                    { "op_Equality", new SymbolReplacement() { Name = "BinderUtils.equals", Kind = SymbolReplacementKind.StaticMethod } },
                    { "op_Inequality", new SymbolReplacement() { Name = "BinderUtils.equals", Kind = SymbolReplacementKind.StaticMethod, Negate = true } },
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

        public static bool HasObjCReplacement(this IMethodSymbol methodSymbol, [NotNullWhen(true)]out SymbolReplacement? objCReplacement)
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
                                objCReplacement = replacement;
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
                    objCReplacement = replacement;
                    return true;
                }
            }

            objCReplacement = null;
            return false;
        }

        public static bool HasObjCReplacement(this IPropertySymbol propertySymbol, [NotNullWhen(true)]out SymbolReplacement? replacement)
        {
            // TODO: look for interface/overridden class
            if (_replacements.TryGetValue(propertySymbol.ContainingType.GetFullName(), out var replacements))
                return replacements.TryGetValue(propertySymbol.Name, out replacement);

            replacement = null;
            return false;
        }

        public static bool HasObjCReplacement(this IFieldSymbol fieldSymbol, [NotNullWhen(true)]out SymbolReplacement? replacement)
        {
            if (_replacements.TryGetValue(fieldSymbol.ContainingType.GetFullName(), out var replacements))
                return replacements.TryGetValue(fieldSymbol.Name, out replacement);

            replacement = null;
            return false;
        }

        public static string GetObjCOperator(this AssignmentExpressionSyntax syntax)
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

        public static string GetObjCOperator(this BinaryExpressionSyntax syntax)
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

        public static string GetObjCOperator(this PrefixUnaryExpressionSyntax syntax)
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

        public static string GetObjCOperator(this PostfixUnaryExpressionSyntax syntax)
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

        public static string GetObjcTypeDeclaration(this BaseTypeDeclarationSyntax node, bool isHeaderWriter)
        {
            return node.GetType().Name switch
            {
                nameof(InterfaceDeclarationSyntax) => "@protocol",
                nameof(ClassDeclarationSyntax) => isHeaderWriter ? "@interface" : "@implementation",
                nameof(StructDeclarationSyntax) => isHeaderWriter ? "@interface" : "@implementation",
                _ => throw new Exception("Unsupported"),
            };
        }

        public static bool IsHeader(this ObjCFileType filetype)
        {
            switch (filetype)
            {
                case ObjCFileType.PublicHeader:
                case ObjCFileType.InternalHeader:
                case ObjCFileType.InternalOnlyHeader:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsImplementation(this ObjCFileType filetype)
        {
            return filetype == ObjCFileType.Implementation;
        }

        public static bool IsPublicHeader(this ObjCFileType filetype)
        {
            return filetype == ObjCFileType.PublicHeader;
        }

        /// <summary>
        /// True when the file type is Internal or InternalOnly header
        /// </summary>
        public static bool IsInternalKindHeader(this ObjCFileType filetype)
        {
            return filetype == ObjCFileType.InternalHeader || filetype == ObjCFileType.InternalOnlyHeader;
        }

        public static string GetObjCName(this PropertyDeclarationSyntax node, ObjCCompilationContext context)
        {
            return node.Identifier.Text.ToObjCCase();
        }

        public static string GetObjCName(this BaseTypeDeclarationSyntax node, ObjCCompilationContext context)
        {
            return getObjCName(node.Identifier.Text);
        }

        public static string GetObjCName(this IdentifierNameSyntax node, ICompilationContextProvider context)
        {
            var symbol = node.GetTypeSymbol(context);
            // We assume ignored types are external types and doesn't need prefix
            if (symbol.HasAttribute<IgnoreAttribute>())
                return node.Identifier.Text;
            else
                return getObjCName(node.Identifier.Text);
        }

        public static string GetObjCName(this GenericNameSyntax node, ICompilationContextProvider context)
        {
            var symbol = node.GetTypeSymbol(context);
            // We assume ignored types are external types and doesn't need prefix
            if (symbol.HasAttribute<IgnoreAttribute>())
                return node.Identifier.Text;
            else
                return getObjCName(node.Identifier.Text);
        }

        public static string GetObjCName(this IMethodSymbol method, ObjCCompilationContext context)
        {
            return context.GetBindedName(method);
        }

        public static string GetObjCName(this ITypeParameterSymbol typeparam, ObjCCompilationContext context)
        {
            if (typeparam.DeclaringMethod?.IsGenericMethod == false || typeparam.ConstraintTypes.Length == 0)
                return typeparam.Name;

            Debug.Assert(typeparam.ConstraintTypes.Length == 1);
            // Objective C doesn't support generic methods, just return first constraint
            // TODO: this is very limited, there should be check in CompilationContext wide types 
            return getObjCName(typeparam.ConstraintTypes[0].Name);
        }

        static string getObjCName(string name)
        {
            return $"{ConversionCSharpToObjC.ConversionPrefix}{name}";
        }

        public static string GetObjCModifiersString(this FieldDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return ObjCUtils.GetFieldModifiersString(modifiers);
        }

        public static string GetObjCModifiersString(this BaseMethodDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return ObjCUtils.GetMethodModifiersString(modifiers);
        }

        public static string GetObjCModifiersString(this BasePropertyDeclarationSyntax node)
        {
            var modifiers = node.GetCSharpModifiers();
            return ObjCUtils.GetPropertyModifiersString(modifiers);
        }

        /// <summary>
        /// Lowercase the first character of the identifier
        /// </summary>
        public static string ToObjCCase(this string text)
        {
            if (string.IsNullOrEmpty(text) || char.IsLower(text, 0))
                return text;

            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// Uppercase the first character of the identifier
        /// </summary>
        public static string ToObjCCaseCapitalized(this string text)
        {
            if (string.IsNullOrEmpty(text) || char.IsUpper(text, 0))
                return text;

            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }
        public static string ToObjCHeaderFilename(this string name, ObjCHeaderNameUse use = ObjCHeaderNameUse.Normal)
        {
            const string extension = "_h";
            string headerName;
            if (name.EndsWith(extension))
                headerName = $"{name.Substring(0, name.Length - extension.Length)}.{ConversionCSharpToObjC.HeaderExtension}";
            else
                headerName = $"{name}.{ConversionCSharpToObjC.HeaderExtension}";

            switch (use)
            {
                case ObjCHeaderNameUse.Normal:
                    return headerName;
                case ObjCHeaderNameUse.IncludeRelativeFirst:
                    return $"\"{headerName}\"";
                case ObjCHeaderNameUse.IncludeGlobalFirst:
                    return $"\"{headerName}\"";
                default:
                    throw new Exception();
            }
        }

        public static string ToObjCImplementationFilename(this string name)
        {
            const string extension = "_mm";
            if (name.EndsWith(extension))
                return $"{name.Substring(0, name.Length - extension.Length)}.{ConversionCSharpToObjC.ImplementationExtension}";
            else
                return $"{name}.{ConversionCSharpToObjC.ImplementationExtension}";
        }
    }
}
