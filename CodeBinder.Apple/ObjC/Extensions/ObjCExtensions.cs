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
using CodeBinder.CLang;
using CodeBinder.Apple.Attributes;

namespace CodeBinder.Apple
{
    static partial class ObjCExtensions
    {
        static Dictionary<string, Dictionary<ReplacementIdentity, SymbolReplacement>> _replacements;

        static ObjCExtensions()
        {
            _replacements = new Dictionary<string, Dictionary<ReplacementIdentity, SymbolReplacement>>()
            {
                // NSObject
                { "System.Object", new Dictionary<ReplacementIdentity, SymbolReplacement>() {
                    { new ReplacementIdentity("GetHashCode()", ObjCSymbolUsage.Declaration), new SymbolReplacement() { Name = "hash", Kind = SymbolReplacementKind.Property, ReturnType = "NSUInteger" } },
                    { new ReplacementIdentity("GetHashCode()", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "CBGetHashCode", Kind = SymbolReplacementKind.StaticMethod } },
                    { new ReplacementIdentity("Equals(Object)", ObjCSymbolUsage.Declaration), new SymbolReplacement() { Name = "isEqualTo", Kind = SymbolReplacementKind.Method } },
                    { new ReplacementIdentity("Equals(Object)", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "isEqualTo", Kind = SymbolReplacementKind.Method } },
                    { new ReplacementIdentity("ToString()", ObjCSymbolUsage.Declaration), new SymbolReplacement() { Name = "description", Kind = SymbolReplacementKind.Property } },
                    { new ReplacementIdentity("ToString()", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "CBToString", Kind = SymbolReplacementKind.StaticMethod } },
                } },
                // NSString
                { "System.String", new Dictionary<ReplacementIdentity, SymbolReplacement>() {
                    { new ReplacementIdentity("operator ==(String, String)", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "CBStringEqual", Kind = SymbolReplacementKind.StaticMethod } },
                    { new ReplacementIdentity("operator !=(String, String)", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "CBStringNotEqual", Kind = SymbolReplacementKind.StaticMethod } },
                } },
                // NSMutableArray
                { "System.Collections.Generic.List<T>", new Dictionary<ReplacementIdentity, SymbolReplacement>() {
                    { new ReplacementIdentity("List(Int32)", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "initWithCapacity", Kind = SymbolReplacementKind.Method } },
                    { new ReplacementIdentity("Add(T)", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "addObject", Kind = SymbolReplacementKind.Method } },
                    { new ReplacementIdentity("Clear()", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "removeAllObjects", Kind = SymbolReplacementKind.Method } },
                } },
                // Pointer types
                { "System.IntPtr", new Dictionary<ReplacementIdentity, SymbolReplacement>() {
                    { new ReplacementIdentity("Zero", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "NULL", Kind = SymbolReplacementKind.Literal } },
                } },
                { "System.UIntPtr", new Dictionary<ReplacementIdentity, SymbolReplacement>() {
                    { new ReplacementIdentity("Zero", ObjCSymbolUsage.Normal), new SymbolReplacement() { Name = "NULL", Kind = SymbolReplacementKind.Literal } },
                } },
            };
        }

        public static bool HasObjCReplacement(this IMethodSymbol methodSymbol, [NotNullWhen(true)]out SymbolReplacement? objCReplacement)
        {
            return HasObjCReplacement(methodSymbol, ObjCSymbolUsage.Normal, out objCReplacement);
        }

        public static bool HasObjCReplacement(this IMethodSymbol methodSymbol, ObjCSymbolUsage usage, [NotNullWhen(true)]out SymbolReplacement? objCReplacement)
        {
            methodSymbol = methodSymbol.OriginalDefinition;
            var uniqueMethodName = methodSymbol.GetNameWithParameters();
            var containingType = methodSymbol.ContainingType;
            Dictionary<ReplacementIdentity, SymbolReplacement>? replacements;
            foreach (var iface in containingType.AllInterfaces)
            {
                string ifaceName = iface.GetFullName();
                if (_replacements.TryGetValue(ifaceName, out replacements))
                {
                    foreach (var member in iface.GetMembers())
                    {
                        if (member.Kind != SymbolKind.Method)
                            continue;

                        if (replacements.TryGetValue(new ReplacementIdentity(uniqueMethodName, usage), out var replacement))
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
                if (replacements.TryGetValue(new ReplacementIdentity(uniqueMethodName, usage), out var replacement))
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
                return replacements.TryGetValue(new ReplacementIdentity(propertySymbol.Name, ObjCSymbolUsage.Normal), out replacement);

            replacement = null;
            return false;
        }

        public static bool HasObjCReplacement(this IFieldSymbol fieldSymbol, [NotNullWhen(true)]out SymbolReplacement? replacement)
        {
            if (_replacements.TryGetValue(fieldSymbol.ContainingType.GetFullName(), out var replacements))
                return replacements.TryGetValue(new ReplacementIdentity(fieldSymbol.Name, ObjCSymbolUsage.Normal), out replacement);

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
                // Unsupported
                case SyntaxKind.IsExpression:   // NOTE: Unsupported only as a binary operator
                case SyntaxKind.AsExpression:   // NOTE: Unsupported only as a binary operator
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
        public static bool IsInternalLikeHeader(this ObjCFileType filetype)
        {
            return filetype == ObjCFileType.InternalHeader || filetype == ObjCFileType.InternalOnlyHeader;
        }

        /// <summary>
        /// True when the file type is Public or InternalOnly header
        /// </summary>
        public static bool IsPublicLikeHeader(this ObjCFileType filetype)
        {
            return filetype == ObjCFileType.PublicHeader || filetype == ObjCFileType.InternalOnlyHeader;
        }

        /// <summary>
        /// True when the header type is Public or InternalOnly header
        /// </summary>
        public static bool IsPublicLikeHeader(this ObjCHeaderType filetype)
        {
            return filetype == ObjCHeaderType.Public || filetype == ObjCHeaderType.InternalOnly;
        }

        public static string GetObjCName(this PropertyDeclarationSyntax node, ObjCCompilationContext context)
        {
            return node.Identifier.Text.ToObjCCase();
        }

        public static bool TryGetDistinctObjCName(this CaseSwitchLabelSyntax node,
            ObjCCompilationContext context, [NotNullWhen(true)]out string? name)
        {
            if (node.Value.TryGetSymbol<IFieldSymbol>(context, out var symbol))
            {
                name = getEnumMemberObjCName(symbol, context);
                return true;
            }

            name = null;
            return false;
        }

        public static string GetObjCName(this BaseTypeDeclarationSyntax node, ObjCCompilationContext context)
        {
            return getObjCName(node.GetDeclaredSymbol<ITypeSymbol>(context), context);
        }

        public static string GetObjCName(this EnumMemberDeclarationSyntax enm, ObjCCompilationContext context)
        {
            var fieldSymbol = enm.GetDeclaredSymbol<IFieldSymbol>(context);
            return getEnumMemberObjCName(fieldSymbol, context);
        }

        public static string GetObjCName(this IMethodSymbol method, ObjCCompilationContext context)
        {
            return GetObjCName(method, ObjCSymbolUsage.Normal, context);
        }

        public static string GetObjCName(this IMethodSymbol method, ObjCSymbolUsage usage, ObjCCompilationContext context)
        {
            SymbolReplacement? replacement;
            if (method.HasObjCReplacement(usage, out replacement))
                return replacement.Name;

            if (context.TryGetBindedName(method, out var bindedName))
                return bindedName;

            if (method.IsNative())
            {
                return method.Name;
            }
            else
            {
                if (method.MethodKind == MethodKind.Constructor)
                    return "init";

                return context.Conversion.MethodsLowerCase ? method.Name.ToObjCCase() : method.Name;
            }
        }

        public static string GetObjCName(this IPropertySymbol property, ObjCCompilationContext context)
        {
            return property.Name.ToObjCCase();
        }

        /// <summary>Handle fields with distinct Objective-C name, like enum members</summary>
        public static bool TryGetDistinctObjCName(this IFieldSymbol field, ObjCCompilationContext context, [NotNullWhen(true)]out string? name)
        {
            if (field.ContainingType.TypeKind != TypeKind.Enum)
            {
                name = null;
                return false;
            }

            name = getEnumMemberObjCName(field, context);
            return true;
        }

        public static string getEnumMemberObjCName(IFieldSymbol enm, ObjCCompilationContext context)
        {
            return $"{enm.ContainingType.GetObjCName(context)}_{enm.Name}";
        }

        public static string GetCLangName(this ITypeSymbol type, CLangTypeUsageKind usage, ObjCCompilationContext context)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Enum:
                {
                    return CLangUtils.GetCLangName(type, usage);
                }
                default:
                {
                    throw new NotSupportedException("Not supported symbol for CLangName");
                }
            }
        }

        public static string GetObjCName(this ITypeSymbol type, ObjCCompilationContext context)
        {
            switch (type.Kind)
            {
                case SymbolKind.NamedType:
                {
                    var namedType = (INamedTypeSymbol)type;
                    return getObjCName(namedType.ConstructedFrom, context);
                }
                case SymbolKind.TypeParameter:
                {
                    var typeparam = (ITypeParameterSymbol)type;
                    if (typeparam.DeclaringMethod?.IsGenericMethod == false || typeparam.ConstraintTypes.Length == 0)
                        return typeparam.Name;

                    Debug.Assert(typeparam.ConstraintTypes.Length == 1);
                    // Objective C doesn't support generic methods, just return first constraint
                    // TODO: this is very limited, there should be check in CompilationContext wide types 
                    return getObjCName(typeparam.ConstraintTypes[0], context);
                }
                default:
                    return getObjCName(type, context);
            }
        }

        static string getObjCName(ITypeSymbol symbol, ObjCCompilationContext context)
        {
            if (context.IsCompilationDefinedType(symbol) && !symbol.HasAttribute<IgnoreAttribute>())
                return $"{ConversionCSharpToObjC.ConversionPrefix}{symbol.Name}";
            else
            {
                if (symbol.HasAttribute<CLangTypeAttribute>())
                {
                    // The following is very lacking. anyway it's mostly unuseful
                    if (symbol.TypeKind == TypeKind.Enum)
                        return $"enum {symbol.Name}";
                    else
                        return $"struct {symbol.Name}";
                }
                else
                    return symbol.Name;
            }

        }

        /// <returns>True if the given accessibility requires to export symbol</returns>
        public static bool RequiresApiAttribute(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Public:
                case Accessibility.Protected:
                case Accessibility.ProtectedAndInternal:
                    return true;
                default:
                    return false;
            }
        }


        public static string GetObjCModifierString(this FieldDeclarationSyntax field, ObjCCompilationContext context)
        {
            var accessibility = field.GetAccessibility(context);
            return ObjCUtils.GetFieldModifiersString(accessibility);
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
            return ToObjCHeaderFilename(name, null, use);
        }

        public static string ToObjCHeaderFilename(this string name, string? basePath, ObjCHeaderNameUse use = ObjCHeaderNameUse.Normal)
        {
            const string extension = "_h";
            string headerName;
            basePath = basePath == null ? null : $"{basePath}/";
            if (name.EndsWith(extension))
                headerName = $"{basePath}{name.Substring(0, name.Length - extension.Length)}.{ConversionCSharpToObjC.HeaderExtension}";
            else
                headerName = $"{basePath}{name}.{ConversionCSharpToObjC.HeaderExtension}";

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

        struct ReplacementIdentity
        {
            public string SymbolName;
            public ObjCSymbolUsage Usage;

            public ReplacementIdentity(string symbolName, ObjCSymbolUsage usage)
            {
                SymbolName = symbolName;
                Usage = usage;
            }

            public override bool Equals(object? obj)
            {
                return obj is ReplacementIdentity identity &&
                       SymbolName == identity.SymbolName &&
                       Usage == identity.Usage;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(SymbolName, Usage);
            }
        }
    }
}
