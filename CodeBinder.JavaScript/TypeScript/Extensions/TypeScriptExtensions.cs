// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Shared;

namespace CodeBinder.JavaScript.TypeScript;

static partial class TypeScriptExtensions
{
    static Dictionary<string, Dictionary<string, SymbolReplacement>> _replacements;

    static TypeScriptExtensions()
    {
        _replacements = new Dictionary<string, Dictionary<string, SymbolReplacement>>()
        {
            // java.lang.Object
            { "System.Object", new Dictionary<string, SymbolReplacement>() {
                { "ToString", new SymbolReplacement() { Name = "toString", Kind = SymbolReplacementKind.Method } },
            } },
            { "System.IntPtr", new Dictionary<string, SymbolReplacement>() {
                { "Zero", new SymbolReplacement() { Name = "0", Kind = SymbolReplacementKind.Literal } },
            } },
            { "System.Array", new Dictionary<string, SymbolReplacement>() {
                { "Length", new SymbolReplacement() { Name = "length", Kind = SymbolReplacementKind.Field } },
            } },
            { "System.Collections.Generic.List<T>", new Dictionary<string, SymbolReplacement>() {
                { "Add", new SymbolReplacement() { Name = "push", Kind = SymbolReplacementKind.Method } },
                { "Clear", new SymbolReplacement() { Name = "BinderUtils.clear", Kind = SymbolReplacementKind.StaticMethod } }, // FIXME: This is an ugly workaround. Solution is AST manipulation
            } },
            { "System.Collections.Generic.IList<T>", new Dictionary<string, SymbolReplacement>() {
                { "Add", new SymbolReplacement() { Name = "push", Kind = SymbolReplacementKind.Method } },
                { "Clear", new SymbolReplacement() { Name = "BinderUtils.clear", Kind = SymbolReplacementKind.StaticMethod } },// FIXME: This is an ugly workaround. Solution is AST manipulation
            } },
            // java.lang.AutoCloseable
            ////{ "System.IDisposable", new Dictionary<string, SymbolReplacement>() {
            ////    { "Dispose", new SymbolReplacement() { Name = "close", Kind = SymbolReplacementKind.Method } }
            ////} },
            // java.lang.Iterable<T>
            ////{ "System.Collections.Generic.IEnumerable<out T>", new Dictionary<string, SymbolReplacement>() {
             ////   { "GetEnumerator", new SymbolReplacement() { Name = "iterator", Kind = SymbolReplacementKind.Method } }
            ////} },
        };
    }

    public static bool HasTypeScriptReplacement(this IMethodSymbol methodSymbol, [NotNullWhen(true)]out SymbolReplacement? javaReplacement)
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

    public static bool NeedNullForgivingOperator(this ITypeSymbol symbol)
    {
        switch (symbol.TypeKind)
        {
            case TypeKind.Interface:
            case TypeKind.Class:
                return true;
            case TypeKind.Struct:
                if (symbol.IsCLRPrimitiveType())
                    return false;
                else
                    return true;
            default:
                return false;
        }
    }

    public static string GetTypeScriptName(this IMethodSymbol method, TypeScriptCompilationContext context)
    {
        return GetTypeScriptName(method, context, out _);
    }

    public static string GetTypeScriptName(this IMethodSymbol method, TypeScriptCompilationContext context, out bool isOverload)
    {
        if (method.HasTypeScriptReplacement(out var replacement))
        {
            isOverload = false;
            return replacement.Name;
        }


        if (context.TryGetBindedName(method, out var bindedName))
        {
            isOverload = bindedName.IsOverload;
            return bindedName.Name;
        }

        isOverload = false;
        if (method.IsNative())
        {
            return method.Name;
        }
        else
        {
            if (method.MethodKind == MethodKind.Constructor)
                return "constructor";

            return context.Conversion.MethodCasing == MethodCasing.LowerCamelCase ? method.Name.ToTypeScriptLowerCase() : method.Name;
        }
    }

    public static bool HasTypeScriptReplacement(this IPropertySymbol propertySymbol, [NotNullWhen(true)]out SymbolReplacement? javaReplacement)
    {
        // TODO: look for interface/overridden class
        if (_replacements.TryGetValue(propertySymbol.ContainingType.GetFullName(), out var replacements))
            return replacements.TryGetValue(propertySymbol.Name, out javaReplacement);

        javaReplacement = null;
        return false;
    }

    public static bool HasTypeScriptReplacement(this IFieldSymbol fieldSymbol, [NotNullWhen(true)]out SymbolReplacement? replacement)
    {
        if (_replacements.TryGetValue(fieldSymbol.ContainingType.GetFullName(), out var replacements))
            return replacements.TryGetValue(fieldSymbol.Name, out replacement);

        replacement = null;
        return false;
    }

    public static string GetJavaScriptOperator(this AssignmentExpressionSyntax syntax)
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
                throw new NotSupportedException();
        }
    }

    public static string GetJavaScriptOperator(this BinaryExpressionSyntax syntax)
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
                return "===";
            case SyntaxKind.NotEqualsExpression:
                return "!==";
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
                throw new NotSupportedException();
        }
    }

    public static string GetJavaScriptOperator(this PrefixUnaryExpressionSyntax syntax)
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
                throw new NotSupportedException();
        }
    }

    public static string GetJavaScriptOperator(this PostfixUnaryExpressionSyntax syntax)
    {
        var op = syntax.Kind();
        switch (op)
        {
            case SyntaxKind.PostIncrementExpression:
                return "++";
            case SyntaxKind.PostDecrementExpression:
                return "--";
            case SyntaxKind.SuppressNullableWarningExpression:
                return "!";
            default:
                throw new NotSupportedException();
        }
    }

    public static string GetModifiersString(this BaseMethodDeclarationSyntax node, TypeScriptCompilationContext context)
    {
        return getMethodModifiersString(node, context);
    }

    public static string GetModifiersString(this AccessorDeclarationSyntax node, TypeScriptCompilationContext context)
    {
        return getMethodModifiersString(node, context);
    }

    static string getMethodModifiersString(SyntaxNode node, TypeScriptCompilationContext context)
    {
        var symbol = node.GetDeclaredSymbol<IMethodSymbol>(context);
        string modifier = getAccessibility(symbol.DeclaredAccessibility);
        if (symbol.IsStatic)
            modifier = modifier == string.Empty ? "static" : $"{modifier} static";
        if (symbol.IsAbstract)
            modifier = modifier == string.Empty ? "abstract" : $"{modifier} abstract";
        if (symbol.OverriddenMethod != null)
            modifier = modifier == string.Empty ? "override" : $"{modifier} override";

        return modifier;
    }

    public static string GetModifiersString(this FieldDeclarationSyntax node, TypeScriptCompilationContext context)
    {
        var symbol = node.GetDeclaredSymbol<IFieldSymbol>(context);
        string modifier = getAccessibility(symbol.DeclaredAccessibility);
        if (symbol.IsStatic)
            modifier = modifier == string.Empty ? "static" : $"{modifier} static";
        return modifier;
    }

    static string getAccessibility(Accessibility accessibility)
    {
        switch (accessibility)
        {
            case Accessibility.Internal:
            case Accessibility.ProtectedOrInternal:
            case Accessibility.Public:
                // Nothing to do, public is the default and internal is not possible
                return string.Empty;
            case Accessibility.Protected:
                return "protected";
            case Accessibility.Private:
                return "private";
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Lowercase the first character of the identifier
    /// </summary>
    public static string ToTypeScriptLowerCase(this string text)
    {
        if (text.IsNullOrEmpty() || char.IsLower(text, 0))
            return text;

        return char.ToLowerInvariant(text[0]) + text.Substring(1);
    }
}
