// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using Microsoft.CodeAnalysis;

namespace CodeBinder.JavaScript.TypeScript;

static partial class TypeScriptExtensions
{
    public static string GetTypeScriptDefaultReturnStatement(this TypeSyntax type, TypeScriptCompilationContext context)
    {
        var builder = new CodeBuilder();
        string? defaultLiteral = type.GetTypeScriptDefaultLiteral(context);
        builder.Append("return");
        if (defaultLiteral != null)
            builder.Space().Append(defaultLiteral);

        return builder.ToString();
    }

    public static string? GetTypeScriptDefaultLiteral(this TypeSyntax type, TypeScriptCompilationContext context)
    {
        var fullName = type.GetFullName(context);
        switch(fullName)
        {
            case "System.Void":
                return null;
            case "System.IntPtr":
                return "0";
            case "System.Boolean":
                return "false";
            case "System.Byte":
            case "System.SByte":
            case "System.Int16":
            case "System.UInt16":
            case "System.Int32":
            case "System.UInt32":
            case "System.Single":
            case "System.Double":
                return "0";
            case "System.Int64":
            case "System.UInt64":
                return "0n";
            default:
                return "null";
        }
    }

    public static string GetTypeScriptType(this PredefinedTypeSyntax syntax)
    {
        var kind = syntax.Kind();
        switch (kind)
        {
            case SyntaxKind.VoidKeyword:
                return "void";
            case SyntaxKind.ObjectKeyword:
                return "Object | null";
            case SyntaxKind.StringKeyword:
                return "string | null";
            case SyntaxKind.BoolKeyword:
                return "boolean";
            case SyntaxKind.SByteKeyword:
                return "number";
            case SyntaxKind.ByteKeyword:
                return "number";
            case SyntaxKind.ShortKeyword:
                return "number";
            case SyntaxKind.UShortKeyword:
                return "number";
            case SyntaxKind.IntKeyword:
                return "number";
            case SyntaxKind.UIntKeyword:
                return "number";
            case SyntaxKind.LongKeyword:
                return "bigint";
            case SyntaxKind.ULongKeyword:
                return "bigint";
            case SyntaxKind.FloatKeyword:
                return "number";
            case SyntaxKind.DoubleKeyword:
                return "number";
            default:
                throw new Exception();
        }
    }

    public static string GetTypeScriptType(this TypeSyntax type, TypeScriptCompilationContext context)
    {
        return GetTypeScriptType(type, TypeScriptTypeFlags.None, context, out _);
    }

    public static string GetTypeScriptType(this TypeSyntax type, TypeScriptCompilationContext context, out bool isInterface)
    {
        return GetTypeScriptType(type, TypeScriptTypeFlags.None, context, out isInterface);
    }

    public static string GetTypeScriptType(this TypeSyntax type, TypeScriptTypeFlags flags, TypeScriptCompilationContext context, out bool isInterface)
    {
        return getTypeScriptType(type, type.GetTypeSymbol(context), flags, context, out isInterface);
    }

    public static string GetTypeScriptType(this TypeSyntax type, TypeScriptTypeFlags flags, TypeScriptCompilationContext context)
    {
        return getTypeScriptType(type, type.GetTypeSymbol(context), flags, context, out _);
    }

    public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax syntax, IPropertySymbol symbol, TypeScriptCompilationContext context)
    {
        writeTypeScriptPropertyIdentifier(builder, syntax, symbol, context);
        return builder;
    }

    // TODO: This method should just handle whole replacements, also member access, example IntPtr.Zero -> 0
    public static bool TryToReplace(this CodeBuilder builder, SyntaxNode syntax, TypeScriptCompilationContext context)
    {
        var symbol = syntax.GetSymbol(context);
        if (symbol == null)
            return false;

        switch (symbol.Kind)
        {
            case SymbolKind.Field:
            {
                var field = (IFieldSymbol)symbol;
                if (field.HasTypeScriptReplacement(out var replacement))
                {
                    builder.Append(replacement.Name);
                    return true;
                }
                break;
            }
            case SymbolKind.Property:
            case SymbolKind.Method:
            {
                // TODO
                break;
            }
        }

        return false;
    }

    public static CodeBuilder Append(this CodeBuilder builder, TypeSyntax syntax, TypeScriptCompilationContext context)
    {
        ISymbol symbol;
        // Symbol can be null https://github.com/dotnet/roslyn/issues/31471
        if (syntax.Kind() == SyntaxKind.ArrayType)
            symbol = syntax.GetTypeSymbol(context);
        else
            symbol = syntax.GetSymbol(context)!;

        switch (symbol.Kind)
        {
            case SymbolKind.TypeParameter:
            case SymbolKind.NamedType:
            case SymbolKind.ArrayType:
            {
                var typeSymbol = (ITypeSymbol)symbol;
                writeTypeScriptType(builder, typeSymbol.GetFullName(), syntax, typeSymbol, null, false, context, out _);
                return builder;
            }
            case SymbolKind.Method:
            {
                writeTypeScriptMethodIdentifier(builder, syntax, (IMethodSymbol)symbol, context);
                break;
            }
            case SymbolKind.Property:
            {
                writeTypeScriptPropertyIdentifier(builder, syntax, (IPropertySymbol)symbol, context);
                break;
            }
            case SymbolKind.Parameter:
            {
                writeTypeScriptParameterIdentifier(builder, syntax, (IParameterSymbol)symbol, context);
                break;
            }
            case SymbolKind.Local:
            case SymbolKind.Field:
            {
                writeTypeScriptIdentifier(builder, syntax, context);
                break;
            }
            case SymbolKind.Namespace:
            {
                // CHECK-ME: Evaluate substitution? Seems ok like this.
                // Maybe better append the syntax instead of the symbol name?
                // Evaluate and comment
                builder.Append(symbol.Name);
                break;
            }
            default:
                throw new Exception();
        }

        return builder;
    }

    static string getTypeScriptType(TypeSyntax syntax, ITypeSymbol symbol, TypeScriptTypeFlags flags,
        TypeScriptCompilationContext context, out bool isInterface)
    {
        var typeName = symbol.GetFullName();
        bool isByRef = flags.HasFlag(TypeScriptTypeFlags.IsByRef);
        var builder = new CodeBuilder();
        writeTypeScriptType(builder, typeName, syntax, symbol, null, isByRef, context, out isInterface);
        return builder.ToString();
    }

    static void writeTypeScriptMethodIdentifier(CodeBuilder builder, TypeSyntax syntax, IMethodSymbol method, TypeScriptCompilationContext context)
    {
        SymbolReplacement? replacement;
        string typeScriptMethodName;
        if (method.HasTypeScriptReplacement(out replacement))
        {
            typeScriptMethodName = replacement.Name;
        }
        else
        {
            if (method.IsNative())
            {
                typeScriptMethodName = $"napi.{method.Name}";
            }
            else
            {
                typeScriptMethodName = context.Conversion.MethodCasing == MethodCasing.LowerCamelCase ? method.Name.ToTypeScriptLowerCase() : method.Name;
            }
        }

        var kind = syntax.Kind();
        switch (kind)
        {
            case SyntaxKind.IdentifierName:
            {
                builder.Append(typeScriptMethodName);
                break;
            }
            case SyntaxKind.GenericName:
            {
                var genericName = (GenericNameSyntax)syntax;
                builder.Append(genericName.TypeArgumentList, genericName, context).Append(typeScriptMethodName);
                break;
            }
            default:
                throw new Exception();
        }
    }

    static void writeTypeScriptPropertyIdentifier(CodeBuilder builder, SyntaxNode syntax, IPropertySymbol property, TypeScriptCompilationContext context)
    {
        bool isSetter = false;
        SyntaxNode child = syntax;
        var parent = syntax.Parent;
        while (parent != null)
        {
            AssignmentExpressionSyntax? assigment;
            if (parent.IsExpression(out assigment))
            {
                // Determine if the LHS of an assiment is the current property symbol
                if (assigment.Left == child && SymbolEqualityComparer.Default.Equals(assigment.Left.GetSymbol(context), property))
                {
                    isSetter = true;
                    break;
                }

                break;
            }

            child = parent;
            parent = child.Parent;
        }

        // TODO: Better handle symbol replacements need/not need of parameter list
        SymbolReplacement? propertyReplacement;
        if (property.HasTypeScriptReplacement(out propertyReplacement))
        {
            if (isSetter)
                builder.Append(propertyReplacement.SetterName);
            else
                builder.Append(propertyReplacement.Name);
        }
        else
        {
            // NOTE: proper use of the setter symbol is done eagerly
            // while writing AssignmentExpressionSyntax
            if (property.IsIndexer)
            {
                if (isSetter)
                    builder.Append("setAt");
                else
                    builder.Append("getAt");
            }
            else
            {
                builder.Append(property.Name.ToLowerCamelCase());
            }
        }
    }

    static void writeTypeScriptParameterIdentifier(CodeBuilder builder, TypeSyntax syntax, IParameterSymbol parameter, TypeScriptCompilationContext context)
    {
        void writeBoxValueAccess()
        {
            writeTypeScriptIdentifier(builder, syntax, context);
            builder.Dot().Append("value");
        }

        if (parameter.RefKind != RefKind.None)
        {
            switch (parameter.Type.TypeKind)
            {
                case TypeKind.Enum:
                {
                    writeBoxValueAccess();
                    return;
                }
                case TypeKind.Struct:
                {
                    if (parameter.Type.IsCLRPrimitiveType())
                    {
                        writeBoxValueAccess();
                        return;
                    }

                    break;
                }
                default:
                    throw new Exception();
            }
        }

        writeTypeScriptIdentifier(builder, syntax, context);
    }

    static void writeTypeScriptIdentifier(CodeBuilder builder, TypeSyntax syntax, TypeScriptCompilationContext context)
    {
        var kind = syntax.Kind();
        switch (kind)
        {
            case SyntaxKind.IdentifierName:
            {
                var identifierName = (IdentifierNameSyntax)syntax;
                builder.Append(identifierName.GetName());
                break;
            }
            default:
                throw new Exception();
        }
    }

    static CodeBuilder Append(this CodeBuilder builder, TypeSyntax type, TypeSyntax parent,
        TypeScriptCompilationContext context)
    {
        var typeSymbol = type.GetTypeSymbol(context);
        writeTypeScriptType(builder, typeSymbol.GetFullName(), type, typeSymbol, parent, false, context, out _);
        return builder;
    }

    static void writeTypeScriptType(CodeBuilder builder, string fullTypeName, TypeSyntax type, ITypeSymbol symbol,
        TypeSyntax? parentType, bool isByRef, TypeScriptCompilationContext context, out bool isInterface)
    {
        if (type.IsVar)
        {
            // Type is inferred
            writeTypeSymbol(builder, fullTypeName, symbol, null, isByRef, out isInterface);
            return;
        }

        // Try to adjust the typename, looking for know types
        switch (symbol.Kind)
        {
            case SymbolKind.NamedType:
            {
                var namedType = (INamedTypeSymbol)symbol;
                if (namedType.IsGenericType)
                    fullTypeName = namedType.ConstructedFrom.GetFullName();

                break;
            }
            case SymbolKind.ArrayType:
            {
                var arrayType = (IArrayTypeSymbol)symbol;
                if (!arrayType.ElementType.IsCLRPrimitiveType())
                    fullTypeName = arrayType.ElementType.GetFullName();
                break;
            }
            case SymbolKind.TypeParameter:
                // Nothing to do
                break;
            default:
                throw new Exception();
        }

        var typeKind = type.Kind();
        string? typeScriptTypeName;
        if (IsKnownTypeScriptType(fullTypeName, isByRef, parentType?.GetTypeSymbol(context), out typeScriptTypeName, out isInterface))
        {
            switch (typeKind)
            {
                case SyntaxKind.GenericName:
                {
                    var genericType = (GenericNameSyntax)type;
                    builder.Append(typeScriptTypeName).Append(genericType.TypeArgumentList, type, context);
                    break;
                }
                case SyntaxKind.ArrayType:
                {
                    var arrayType = (ArrayTypeSyntax)type;
                    Debug.Assert(arrayType.RankSpecifiers.Count == 1);
                    //// CHECK-ME: Handle rank?
                    ////builder.Append(typeScriptTypeName).Append(arrayType.RankSpecifiers[0], context);
                    builder.Append(typeScriptTypeName);
                    break;
                }
                case SyntaxKind.NullableType:
                {
                    string? boxTypeName;
                    if (TypeScriptUtils.TryGetBoxType(fullTypeName, out boxTypeName))
                        builder.Append(boxTypeName);
                    else
                        throw new NotSupportedException();

                    break;
                }
                case SyntaxKind.PredefinedType:
                {
                    builder.Append(typeScriptTypeName);
                    break;
                }
                case SyntaxKind.IdentifierName:
                {
                    builder.Append(typeScriptTypeName);
                    break;
                }
                case SyntaxKind.QualifiedName:
                {
                    //var qualifiedName = type as QualifiedNameSyntax;
                    throw new NotImplementedException();
                }
                default:
                    throw new NotSupportedException();
            }
        }
        else
        {
            switch (typeKind)
            {
                case SyntaxKind.IdentifierName:
                {
                    var identifierName = (IdentifierNameSyntax)type;
                    builder.Append(identifierName.GetName());
                    break;
                }
                case SyntaxKind.ArrayType:
                {
                    var arrayType = (ArrayTypeSyntax)type;
                    Debug.Assert(arrayType.RankSpecifiers.Count == 1);
                    builder.Append(arrayType.ElementType, type, context).Append(arrayType.RankSpecifiers[0], context);
                    break;
                }
                case SyntaxKind.GenericName:
                {
                    var genericType = (GenericNameSyntax)type;
                    builder.Append(genericType.GetName()).Append(genericType.TypeArgumentList, type, context);
                    break;
                }
                case SyntaxKind.NullableType:
                {
                    var nullableType = (NullableTypeSyntax)type;
                    switch (symbol.TypeKind)
                    {
                        case TypeKind.Struct:
                            builder.Append(nullableType.ElementType, type, context);
                            break;
                        case TypeKind.Enum:
                            throw new NotImplementedException("TODO");
                        default:
                            throw new NotSupportedException();
                    }

                    break;
                }
                case SyntaxKind.QualifiedName:
                {
                    var qualifiedName = (QualifiedNameSyntax)type;
                    builder.Append(qualifiedName.Left, type, context).Dot().Append(qualifiedName.Right, type, context);
                    break;
                }
                default:
                    throw new NotSupportedException();
            }

            isInterface = symbol.TypeKind == TypeKind.Interface;
        }

        if (!(skipNullableCondition(type, parentType, symbol)))
            builder.Append(" | null");
    }

    static bool skipNullableCondition(TypeSyntax type, TypeSyntax? parentType, ITypeSymbol typeSymbol)
    {
        switch (parentType?.Kind() ?? SyntaxKind.None)
        {
            case SyntaxKind.GenericName:
            case SyntaxKind.NullableType:
                return true;
        }

        switch (type.Parent?.Kind() ?? SyntaxKind.None)
        {
            case SyntaxKind.SimpleBaseType:
            case SyntaxKind.TypeConstraint:
            case SyntaxKind.SimpleMemberAccessExpression:
                return true;
        }

        if (typeSymbol.SpecialType == SpecialType.System_Void || typeSymbol.IsCLRPrimitiveType())
            return true;

        return false;
    }

    static CodeBuilder Append(this CodeBuilder builder, ITypeSymbol symbol, ITypeSymbol parent)
    {
        writeTypeSymbol(builder, symbol.GetFullName(), symbol, parent, false, out _);
        return builder;
    }

    /// <summary>This method is mainly used for inferred types</summary>
    static void writeTypeSymbol(CodeBuilder builder, string fullTypeName, ITypeSymbol symbol,
        ITypeSymbol? parentSymbol, bool isByRef, out bool isInterface)
    {
        // Try to adjust the typename, looking for know types
        switch (symbol.Kind)
        {
            case SymbolKind.NamedType:
            {
                var namedType = (INamedTypeSymbol)symbol;
                if (namedType.IsGenericType)
                {
                    if (namedType.IsNullable())
                    {
                        switch (symbol.TypeKind)
                        {
                            case TypeKind.Struct:
                                var nullableType = namedType.TypeArguments[0];
                                writeTypeSymbol(builder, nullableType.GetFullName(), nullableType, symbol, isByRef, out isInterface);
                                return;
                            case TypeKind.Enum:
                                throw new Exception("TODO");
                            default:
                                throw new Exception();
                        }
                    }

                    fullTypeName = namedType.ConstructedFrom.GetFullName();
                }

                break;
            }
            case SymbolKind.ArrayType:
            {
                var arrayType = (IArrayTypeSymbol)symbol;
                builder.Append(arrayType.ElementType, symbol).EmptyRankSpecifier();
                isInterface = false;
                return;
            }
            case SymbolKind.TypeParameter:
                // Nothing to do
                break;
            default:
                throw new Exception();
        }

        string? typeScriptTypeName;
        if (!IsKnownTypeScriptType(fullTypeName, isByRef, parentSymbol, out typeScriptTypeName, out isInterface))
        {
            isInterface = symbol.TypeKind == TypeKind.Interface;
            typeScriptTypeName = symbol.GetQualifiedName();
        }

        switch (symbol.Kind)
        {
            case SymbolKind.NamedType:
            {
                builder.Append(typeScriptTypeName);
                var namedType = (INamedTypeSymbol)symbol;
                if (namedType.IsGenericType)
                {
                    using (builder.TypeParameterList())
                    {
                        bool first = true;
                        foreach (var parameter in namedType.TypeArguments)
                            builder.CommaSeparator(ref first).Append(parameter, symbol);
                    }
                }

                break;
            }
            case SymbolKind.TypeParameter:
            {
                builder.Append(typeScriptTypeName);
                break;
            }
            default:
                throw new Exception();
        }
    }

    static CodeBuilder Append(this CodeBuilder builder, TypeArgumentListSyntax syntax, TypeSyntax parent, TypeScriptCompilationContext context)
    {
        using (builder.TypeParameterList())
        {
            bool first = true;
            foreach (var type in syntax.Arguments)
                builder.CommaSeparator(ref first).Append(type, parent, context);
        }

        return builder;
    }

    static CodeBuilder Append(this CodeBuilder builder, ArrayRankSpecifierSyntax syntax, TypeScriptCompilationContext context)
    {
        if (syntax.Sizes.Count > 0)
        {
            Debug.Assert(syntax.Sizes.Count == 1);
            builder.Bracketed().Append(syntax.Sizes[0], context);
        }
        else
        {
            builder.EmptyRankSpecifier();
        }

        return builder;
    }

    public static bool IsKnownTypeScriptType(string fullTypeName, bool isByRef, ITypeSymbol? parent,
        [NotNullWhen(true)]out string? knownTypeScriptType, out bool isInterface)
    {
        if (IsKnowSimpleTypeScriptType(fullTypeName, isByRef, parent, out knownTypeScriptType))
        {
            isInterface = false;
            return true;
        }

        switch (fullTypeName)
        {
            case "System.Exception":
            {
                knownTypeScriptType = "Exception";
                isInterface = false;
                return true;
            }
            case "System.NotImplementedException":
            {
                knownTypeScriptType = "NotImplementedException";
                isInterface = false;
                return true;
            }
            case "System.IDisposable":
            {
                knownTypeScriptType = "IDisposable";
                isInterface = true;
                return true;
            }
            case "System.Collections.Generic.IEnumerable<out T>":
            {
                knownTypeScriptType = "Iterable";
                isInterface = true;
                return true;
            }
            case "System.Byte[]":
            {
                knownTypeScriptType = "Uint8ClampedArray";
                isInterface = false;
                return true;
            }
            case "System.SByte[]":
            {
                knownTypeScriptType = "Int8Array";
                isInterface = false;
                return true;
            }
            case "System.UInt16[]":
            {
                knownTypeScriptType = "Uint16Array";
                isInterface = false;
                return true;
            }
            case "System.Int16[]":
            {
                knownTypeScriptType = "Int16Array";
                isInterface = false;
                return true;
            }
            case "System.UInt32[]":
            {
                knownTypeScriptType = "Uint32Array";
                isInterface = false;
                return true;
            }
            case "System.Int32[]":
            {
                knownTypeScriptType = "Int32Array";
                isInterface = false;
                return true;
            }
            case "System.UInt64[]":
            {
                knownTypeScriptType = "BigUint64Array";
                isInterface = false;
                return true;
            }
            case "System.Int64[]":
            {
                knownTypeScriptType = "BigInt64Array";
                isInterface = false;
                return true;
            }
            case "System.Single[]":
            {
                knownTypeScriptType = "Float32Array";
                isInterface = false;
                return true;
            }
            case "System.Double[]":
            {
                knownTypeScriptType = "Float64Array";
                isInterface = false;
                return true;
            }
            case "System.Collections.Generic.List<T>":
            {
                knownTypeScriptType = "Array";
                isInterface = false;
                return true;
            }
            case "System.Collections.Generic.KeyValuePair<TKey, TValue>":
            {
                knownTypeScriptType = "KeyValuePair";
                isInterface = false;
                return true;
            }
            default:
            {
                knownTypeScriptType = null;
                isInterface = false;
                return false;
            }
        }
    }

    public static bool IsKnowSimpleTypeScriptType(string fullTypeName, bool isByRef, ITypeSymbol? parent, [NotNullWhen(true)]out string? knownTypeScriptType)
    {
        if (isByRef)
        {
            if (TypeScriptUtils.TryGetRefBoxType(fullTypeName, out knownTypeScriptType))
                return true;
            else
                return false;
        }
        else if (parent?.IsGeneric() == true)
        {
            switch (fullTypeName)
            {
                case "System.Object":
                    knownTypeScriptType = "object";
                    return true;
                case "System.String":
                    knownTypeScriptType = "string";
                    return true;
                case "System.Boolean":
                    knownTypeScriptType = "boolean";
                    return true;
                case "System.IntPtr":
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Single":
                case "System.Double":
                    knownTypeScriptType = "number";
                    return true;
                case "System.Int64":
                case "System.UInt64":
                    knownTypeScriptType = "bigint";
                    return true;
                case "System.Void":
                    throw new NotSupportedException();
                default:
                    knownTypeScriptType = null;
                    return false;
            }
        }
        else
        {
            switch (fullTypeName)
            {
                case "System.Void":
                    knownTypeScriptType = "void";
                    return true;
                case "System.Object":
                    knownTypeScriptType = "object";
                    return true;
                case "CodeBinder.cbstring":
                    knownTypeScriptType = "string";
                    return true;
                case "System.String":
                    knownTypeScriptType = "string";
                    return true;
                case "System.Boolean":
                    knownTypeScriptType = "boolean";
                    return true;
                case "System.IntPtr":
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Single":
                case "System.Double":
                    knownTypeScriptType = "number";
                    return true;
                case "System.Int64":
                case "System.UInt64":
                    knownTypeScriptType = "bigint";
                    return true;
                default:
                    knownTypeScriptType = null;
                    return false;
            }
        }
    }
}
