// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.JavaScript.NAPI;
using CodeBinder.Shared;
using System.Drawing;
using System.Security.Policy;
using System.Xml.Linq;

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
        var symbol = type.GetTypeSymbolThrow(context);
        var fullName = symbol.GetFullName();
        switch(fullName)
        {
            case "System.Void":
                return null;
            case "System.IntPtr":
                return "0";
            case "CodeBinder.cbbool":
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
                if (symbol.TypeKind == TypeKind.Enum)
                {
                    return $"0 as {symbol.Name}";
                }
                else
                {
                    if (type.IsKind(SyntaxKind.NullableType))
                        return "null";
                    else
                        return "null!";
                }
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
                throw new NotSupportedException();
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
        return getTypeScriptType(type, type.GetTypeSymbolThrow(context), flags, context, out isInterface);
    }

    public static string GetTypeScriptType(this TypeSyntax type, TypeScriptTypeFlags flags, TypeScriptCompilationContext context)
    {
        return getTypeScriptType(type, type.GetTypeSymbolThrow(context), flags, context, out _);
    }

    public static string GetTypeScriptType(this ITypeSymbol type, TypeScriptTypeFlags flags = TypeScriptTypeFlags.None)
    {
        var builder = new CodeBuilder();
        bool isByRef = flags.HasFlag(TypeScriptTypeFlags.ByRef);
        bool isTypeArgument = flags.HasFlag(TypeScriptTypeFlags.TypeArgument);
        writeTypeSymbol(builder, type.GetFullName(), type, isByRef, isTypeArgument, out _);
        return builder.ToString();
    }

    public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax syntax, IPropertySymbol symbol, TypeScriptCompilationContext context)
    {
        writeTypeScriptPropertyIdentifier(builder, syntax, symbol, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ITypeSymbol symbol)
    {
        writeTypeSymbol(builder, symbol.GetFullName(), symbol, false, false, out _);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, TypeSyntax syntax, TypeScriptCompilationContext context)
    {
        ISymbol symbol;
        // Symbol can be null https://github.com/dotnet/roslyn/issues/31471
        if (syntax.IsVar || syntax.Kind() == SyntaxKind.ArrayType)
            symbol = syntax.GetTypeSymbolThrow(context);
        else
            symbol = syntax.GetSymbol(context)!;

        switch (symbol.Kind)
        {
            case SymbolKind.TypeParameter:
            case SymbolKind.NamedType:
            case SymbolKind.ArrayType:
            {
                var typeSymbol = (ITypeSymbol)symbol;
                writeTypeScriptType(builder, typeSymbol.GetFullName(), syntax, typeSymbol, false, false, context, out _);
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
                throw new NotSupportedException();
        }

        return builder;
    }

    static string getTypeScriptType(TypeSyntax syntax, ITypeSymbol symbol, TypeScriptTypeFlags flags,
        TypeScriptCompilationContext context, out bool isInterface)
    {
        var typeName = symbol.GetFullName();
        bool isByRef = flags.HasFlag(TypeScriptTypeFlags.ByRef);
        bool isTypeArgument = flags.HasFlag(TypeScriptTypeFlags.TypeArgument);
        var builder = new CodeBuilder();
        writeTypeScriptType(builder, typeName, syntax, symbol, isByRef, isTypeArgument, context, out isInterface);
        return builder.ToString();
    }

    static void writeTypeScriptMethodIdentifier(CodeBuilder builder, TypeSyntax syntax, IMethodSymbol method, TypeScriptCompilationContext context)
    {
        string typeScriptMethodName = method.GetTypeScriptName(context);
        if (method.IsNative())
            typeScriptMethodName = $"napi.{typeScriptMethodName}";

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
                builder.append(genericName.TypeArgumentList, context).Append(typeScriptMethodName);
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
                    throw new NotSupportedException();
            }
        }

        writeTypeScriptIdentifier(builder, syntax, context);
    }

    static void writeTypeScriptIdentifier(CodeBuilder builder, TypeSyntax syntax, TypeScriptCompilationContext context)
    {
        _ = context;
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
                throw new NotSupportedException();
        }
    }

    static void writeTypeScriptType(CodeBuilder builder, string fullTypeName, TypeSyntax type, ITypeSymbol symbol,
        bool isByRef, bool isTypeArgument, TypeScriptCompilationContext context, out bool isInterface)
    {
        if (type.IsVar)
        {
            // Type is inferred
            writeTypeSymbol(builder, fullTypeName, symbol, isByRef, isTypeArgument, out isInterface);
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
                var arraySymbol = (IArrayTypeSymbol)symbol;
                if (!arraySymbol.ElementType.IsCLRPrimitiveType())
                    fullTypeName = arraySymbol.ElementType.GetFullName();
                break;
            }
            case SymbolKind.TypeParameter:
                // Nothing to do
                break;
            default:
                throw new NotSupportedException();
        }

        var typeKind = type.Kind();
        string? typeScriptTypeName;
        if (isKnownTypeScriptType(fullTypeName, isByRef, isTypeArgument, out typeScriptTypeName, out isInterface))
        {
            switch (typeKind)
            {
                case SyntaxKind.GenericName:
                {
                    var genericType = (GenericNameSyntax)type;
                    builder.Append(typeScriptTypeName).append(genericType.TypeArgumentList, context);
                    break;
                }
                case SyntaxKind.ArrayType:
                {
                    var arrayType = (ArrayTypeSyntax)type;
                    var arraySymbol = (IArrayTypeSymbol)symbol;
                    Debug.Assert(arrayType.RankSpecifiers.Count == 1);
                    if (arraySymbol.ElementType.IsCLRPrimitiveType())
                    {
                        builder.Append(typeScriptTypeName).append(arrayType.RankSpecifiers[0], context);
                    }
                    else
                    {
                        builder.Append("Array").AngleBracketed().Append(typeScriptTypeName).Close()
                            .append(arrayType.RankSpecifiers[0], context);
                    }    
                    break;
                }
                case SyntaxKind.NullableType:
                {
                    string? boxTypeName;
                    if (TypeScriptUtils.TryGetBoxType(fullTypeName, out boxTypeName))
                    {
                        builder.Append(boxTypeName);
                    }
                    else
                    {
                        var nullableType = (NullableTypeSyntax)type;
                        switch (symbol.TypeKind)
                        {
                            case TypeKind.Class:
                            case TypeKind.Struct:
                            case TypeKind.Interface:
                            case TypeKind.TypeParameter:
                            case TypeKind.Array:
                                var elementTypeSymbol = nullableType.ElementType.GetSymbol<ITypeSymbol>(context);
                                writeTypeScriptType(builder, elementTypeSymbol.GetFullName(), nullableType.ElementType, elementTypeSymbol, false, true, context, out _);
                                builder.Append(" | null");
                                break;
                            case TypeKind.Enum:
                                throw new NotImplementedException("TODO");
                            default:
                                throw new NotSupportedException();
                        }
                    }

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
                    builder.Append("Array").AngleBracketed().Append(arrayType.ElementType, context).Close()
                        .append(arrayType.RankSpecifiers[0], context);
                    break;
                }
                case SyntaxKind.GenericName:
                {
                    var genericType = (GenericNameSyntax)type;
                    builder.Append(genericType.GetName()).append(genericType.TypeArgumentList, context);
                    break;
                }
                case SyntaxKind.NullableType:
                {
                    var nullableType = (NullableTypeSyntax)type;
                    switch (symbol.TypeKind)
                    {
                        case TypeKind.Class:
                        case TypeKind.Struct:
                        case TypeKind.Interface:
                        case TypeKind.TypeParameter:
                        case TypeKind.Array:
                            var elementTypeSymbol = nullableType.ElementType.GetSymbol<ITypeSymbol>(context);
                            writeTypeScriptType(builder, elementTypeSymbol.GetFullName(), nullableType.ElementType, elementTypeSymbol, false, true, context, out _);
                            builder.Append(" | null");
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
                    builder.Append(qualifiedName.Left, context).Dot().Append(qualifiedName.Right, context);
                    break;
                }
                default:
                    throw new NotSupportedException();
            }

            isInterface = symbol.TypeKind == TypeKind.Interface;
        }
    }

    static CodeBuilder append(this CodeBuilder builder, TypeArgumentListSyntax syntax, TypeScriptCompilationContext context)
    {
        using (builder.TypeParameterList())
        {
            bool first = true;
            foreach (var type in syntax.Arguments)
            {
                builder.CommaSeparator(ref first);
                var argumentTypeSymbol = type.GetSymbol<ITypeSymbol>(context);
                writeTypeScriptType(builder, argumentTypeSymbol.GetFullName(), type, argumentTypeSymbol, false, true, context, out _);
            }
        }

        return builder;
    }

    static CodeBuilder append(this CodeBuilder builder, ArrayRankSpecifierSyntax syntax, TypeScriptCompilationContext context)
    {
        if (syntax.Sizes.Count != 0)
        {
            Debug.Assert(syntax.Sizes.Count == 1);
            var size = syntax.Sizes[0];
            // Ignore omitted array size expression. It can be either:
            // - An initialization "new Array<number>", which valid TS syntax
            // - A declaration of any kind, eg. a parameter "param: Array<number>",
            // which will work without any rank specifier
            if (!size.IsKind(SyntaxKind.OmittedArraySizeExpression))
                builder.Parenthesized().Append(size, context).Close();
        }

        return builder;
    }

    /// <summary>This method is mainly used for inferred types</summary>
    static void writeTypeSymbol(CodeBuilder builder, string fullTypeName, ITypeSymbol symbol,
        bool isByRef, bool isTypeArgument, out bool isInterface)
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
                                writeTypeSymbol(builder, nullableType.GetFullName(), nullableType, isByRef, isTypeArgument, out isInterface);
                                return;
                            case TypeKind.Enum:
                                throw new Exception("TODO");
                            default:
                                throw new NotSupportedException();
                        }
                    }

                    fullTypeName = namedType.ConstructedFrom.GetFullName();
                }

                break;
            }
            case SymbolKind.ArrayType:
            {
                var arrayType = (IArrayTypeSymbol)symbol;
                switch (arrayType.ElementType.SpecialType)
                {
                    case SpecialType.System_Byte:
                    {
                        builder.Append("Uint8Array");
                        break;
                    }
                    case SpecialType.System_SByte:
                    {
                        builder.Append("Int8Array");
                        break;
                    }
                    case SpecialType.System_UInt16:
                    {
                        builder.Append("Uint16Array");
                        break;
                    }
                    case SpecialType.System_Int16:
                    {
                        builder.Append("Int16Array");
                        break;
                    }
                    case SpecialType.System_UInt32:
                    {
                        builder.Append("Uint32Array");
                        break;
                    }
                    case SpecialType.System_Int32:
                    {
                        builder.Append("Int32Array");
                        break;
                    }
                    case SpecialType.System_UInt64:
                    {
                        builder.Append("BigUint64Array");
                        break;
                    }
                    case SpecialType.System_Int64:
                    {
                        builder.Append("BigInt64Array");
                        break;
                    }
                    case SpecialType.System_Single:
                    {
                        builder.Append("Float32Array");
                        break;
                    }
                    case SpecialType.System_Double:
                    {
                        builder.Append("Float64Array");
                        break;
                    }
                    case SpecialType.System_IntPtr:
                    {
                        builder.Append("Float64Array");
                        break;
                    }
                    case SpecialType.System_Boolean:
                    {
                        builder.Append("BooleanArray");
                        break;
                    }
                    case SpecialType.System_String:
                    {
                        builder.Append("string[]");
                        break;
                    }
                    case SpecialType.None:
                    {
                        var typeName = arrayType.ElementType.GetFullName();
                        switch (typeName)
                        {
                            case "CodeBinder.cbstring":
                            {
                                builder.Append("string[]");
                                break;
                            }
                            default:
                                throw new NotSupportedException();
                        }
                        break;
                    }
                    default:
                        throw new NotSupportedException();
                }

                isInterface = false;
                if (symbol.IsNullable())
                    builder.Append(" | null");

                return;
            }
            case SymbolKind.TypeParameter:
                // Nothing to do
                break;
            default:
                throw new NotSupportedException();
        }

        string? typeScriptTypeName;
        if (!isKnownTypeScriptType(fullTypeName, isByRef, isTypeArgument, out typeScriptTypeName, out isInterface))
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
                            builder.CommaSeparator(ref first).Append(parameter);
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
                throw new NotSupportedException();
        }

        if (symbol.IsNullable())
            builder.Append(" | null");
    }

    static bool isKnownTypeScriptType(string fullTypeName, bool isByRef, bool isTypeArgument,
        [NotNullWhen(true)]out string? knownTypeScriptType, out bool isInterface)
    {
        if (isKnowSimpleTypeScriptType(fullTypeName, isByRef, isTypeArgument, out knownTypeScriptType))
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
            case "System.Collections.Generic.IEnumerator<out T>":
            {
                knownTypeScriptType = "Iterator";
                isInterface = true;
                return true;
            }
            case "System.Collections.Generic.IEnumerable<out T>":
            {
                knownTypeScriptType = "Iterable";
                isInterface = true;
                return true;
            }
            case "System.Boolean[]":
            {
                knownTypeScriptType = "BooleanArray";
                isInterface = false;
                return true;
            }
            case "System.Byte[]":
            {
                knownTypeScriptType = "Uint8Array";
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
            case "System.IntPtr[]":
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

    static bool isKnowSimpleTypeScriptType(string fullTypeName, bool isByRef, bool isTypeArgument, [NotNullWhen(true)]out string? knownTypeScriptType)
    {
        if (isByRef)
        {
            if (TypeScriptUtils.TryGetRefBoxType(fullTypeName, out knownTypeScriptType))
                return true;
            else
                return false;
        }
        else if (isTypeArgument)
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
                case "System.String":
                    knownTypeScriptType = "string";
                    return true;
                case "CodeBinder.cbbool":
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
