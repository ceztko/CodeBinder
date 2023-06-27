// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using CodeBinder.Shared.CSharp;
using CodeBinder.Utils;
using CodeBinder.Shared;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CodeBinder.Attributes;
using System.Linq;

namespace CodeBinder.Apple
{
    static partial class ObjCExtensions
    {
        public static string GetObjCDefaultReturnStatement(this TypeSyntax type,
            ObjCCompilationContext context)
        {
            var builder = new CodeBuilder();
            string? defaultLiteral = type.GetObjCDefaultLiteral(context);
            builder.Append("return");
            if (!defaultLiteral.IsNullOrEmpty())
                builder.Space().Append(defaultLiteral);

            return builder.ToString();
        }

        public static string? GetObjCDefaultLiteral(this TypeSyntax type,
            ObjCCompilationContext context)
        {
            var symbol = type.GetTypeSymbol(context);
            if (symbol.TypeKind == TypeKind.Enum)
            {
                // Return a default 0 value for the enum
                return $"({symbol.GetObjCName(context)})0";
            }

            var fullName = symbol.GetFullName();
            switch (fullName)
            {
                case "System.Void":
                    return null;
                case "System.UIntPtr":
                case "System.IntPtr":
                    return "nullptr";
                case "System.Boolean":
                    return "NO";
                case "CodeBinder.Apple.NSUInteger":
                case "CodeBinder.Apple.NSInteger":
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                    return "0";
                default:
                    return "nil";
            }
        }

        public static string GetObjCBoxType(this PredefinedTypeSyntax syntax)
        {
            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.BoolKeyword:
                case SyntaxKind.SByteKeyword:
                case SyntaxKind.ByteKeyword:
                case SyntaxKind.ShortKeyword:
                case SyntaxKind.UShortKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.UIntKeyword:
                case SyntaxKind.LongKeyword:
                case SyntaxKind.ULongKeyword:
                case SyntaxKind.FloatKeyword:
                case SyntaxKind.DoubleKeyword:
                    return "NSNumber";
                default:
                    throw new NotSupportedException();
            }
        }

        public static string GetObjCType(this PredefinedTypeSyntax syntax)
        {
            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.VoidKeyword:
                    return "void";
                case SyntaxKind.ObjectKeyword:
                    return "NSObject";
                case SyntaxKind.StringKeyword:
                    return "NSString";
                case SyntaxKind.BoolKeyword:
                    return "BOOL";
                case SyntaxKind.SByteKeyword:
                    return "int8_t";
                case SyntaxKind.ByteKeyword:
                    return "uint8_t";
                case SyntaxKind.ShortKeyword:
                    return "int16_t";
                case SyntaxKind.UShortKeyword:
                    return "uint16_t";
                case SyntaxKind.IntKeyword:
                    return "int32_t";
                case SyntaxKind.UIntKeyword:
                    return "uint32_t";
                case SyntaxKind.LongKeyword:
                    return "int64_t";
                case SyntaxKind.ULongKeyword:
                    return "uint64_t";
                case SyntaxKind.FloatKeyword:
                    return "float";
                case SyntaxKind.DoubleKeyword:
                    return "double";
                default:
                    throw new NotSupportedException();
            }
        }

        public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax syntax,
            IPropertySymbol symbol, ObjCCompilationContext context)
        {
            writeObjCPropertyIdentifier(builder, syntax, symbol, context);
            return builder;
        }

        // TODO: This method should just handle whole replacements, also member access, example IntPtr.Zero -> 0
        public static bool TryToReplace(this CodeBuilder builder, SyntaxNode syntax, ObjCCompilationContext context)
        {
            var symbol = syntax.GetSymbol(context);
            if (symbol == null)
                return false;

            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                {
                    var field = (IFieldSymbol)symbol;
                    if (field.HasObjCReplacement(out var replacement))
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

        public static string GetObjCType(this TypeSyntax type, ObjCCompilationContext context)
        {
            var builder = new CodeBuilder();
            var typeSymbol = type.GetTypeSymbol(context);
            ObjCTypeKind objcTypeKind;
            writeTypeSymbol(builder, typeSymbol, ObjCTypeUsageKind.Normal, context, out objcTypeKind);
            return builder.ToString();
        }

        public static ObjCTypeInfo GetObjCTypeInfo(this TypeSyntax type, ObjCCompilationContext context)
        {
            var builder = new CodeBuilder();
            var typeSymbol = type.GetTypeSymbol(context);
            string fullName = typeSymbol.GetFullName();
            var ret = new ObjCTypeInfo();
            writeTypeSymbol(builder, typeSymbol, ObjCTypeUsageKind.Normal, context, out ret.Kind);
            ret.Reachability = GetReachability(typeSymbol, context);
            ret.TypeName = builder.ToString();
            return ret;
        }

        public static string GetObjCType(this TypeSyntax type, ObjCTypeUsageKind displayKind, ObjCCompilationContext context)
        {
            var builder = new CodeBuilder();
            var typeSymbol = type.GetTypeSymbol(context);
            writeTypeSymbol(builder, typeSymbol, displayKind, context, out var objcTypeKind);
            return builder.ToString();
        }

        public static string GetObjCType(this ITypeSymbol typeSymbol, ObjCTypeUsageKind displayKind, ObjCCompilationContext context)
        {
            var builder = new CodeBuilder();
            writeTypeSymbol(builder, typeSymbol, displayKind, context, out var objcTypeKind);
            return builder.ToString();
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeSyntax syntax, ObjCCompilationContext context)
        {
            return Append(builder, syntax, ObjCTypeUsageKind.Normal, context);
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeSyntax syntax, ObjCTypeUsageKind usageKind, ObjCCompilationContext context)
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
                    ObjCTypeKind objcTypeKind;
                    var typeSymbol = (ITypeSymbol)symbol;
                    writeTypeSymbol(builder, typeSymbol, usageKind, context, out objcTypeKind);
                    return builder;
                }
                case SymbolKind.Method:
                {
                    writeObjCMethodIdentifier(builder, syntax, (IMethodSymbol)symbol, context);
                    break;
                }
                case SymbolKind.Property:
                {
                    writeObjCPropertyIdentifier(builder, syntax, (IPropertySymbol)symbol, context);
                    break;
                }
                case SymbolKind.Parameter:
                {
                    writeObjCParameterIdentifier(builder, syntax, (IParameterSymbol)symbol, context);
                    break;
                }
                case SymbolKind.Local:
                {
                    writeObjCLocalIdentifier(builder, syntax, (ILocalSymbol)symbol, context);
                    break;
                }
                case SymbolKind.Field:
                {
                    writeObjCFieldIdentifier(builder, syntax, (IFieldSymbol)symbol, context);
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

        static CodeBuilder Append(this CodeBuilder builder, ITypeSymbol typeSymbol, ObjCTypeUsageKind displayKind, ObjCCompilationContext context)
        {
            ObjCTypeKind objcTypeKind;
            writeTypeSymbol(builder, typeSymbol, displayKind, context, out objcTypeKind);
            return builder;
        }

        static void writeObjCMethodIdentifier(CodeBuilder builder, TypeSyntax syntax, IMethodSymbol method,
            ObjCCompilationContext context)
        {
            string objCMethodName = method.GetObjCName(context);
            var kind = syntax.Kind();
            switch (kind)
            {
                // NOTE: Don't append generic parameters here: ObjectiveC
                // does not support generic methods
                case SyntaxKind.GenericName:
                case SyntaxKind.IdentifierName:
                {
                    builder.Append(objCMethodName);
                    break;
                }
                default:
                    throw new Exception();
            }
        }

        static void writeObjCPropertyIdentifier(CodeBuilder builder, SyntaxNode syntax, IPropertySymbol property, ObjCCompilationContext context)
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

            void appendProperty()
            {
                // TODO: Better handle symbol replacements need/not need of parameter list
                SymbolReplacement? propertyReplacement;
                if (property.HasObjCReplacement(out propertyReplacement))
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
                            builder.Append("set");
                        else
                            builder.Append("get");
                    }
                    else
                    {
                        builder.Append(property.GetObjCName(context));
                    }
                }
            }

            if (property.IsStatic)
            {
                // Static property https://stackoverflow.com/a/15811719/213871
                if (property.IsIndexer)
                {
                    appendProperty();
                }
                else
                {
                    builder.Append(property.ContainingType, ObjCTypeUsageKind.Normal, context).Dot();
                    appendProperty();
                }

                // CHECK-ME This has not been verified
                throw new NotSupportedException();
            }
            else
            {
                if (!property.IsIndexer &&
                    (!syntax.Parent!.IsExpression(ExpressionKind.MemberAccess)
                    || (syntax.Parent as MemberAccessExpressionSyntax)!.Expression == syntax))
                {
                    builder.Append("self").Dot();
                }

                appendProperty();
            }
        }

        static void writeObjCParameterIdentifier(CodeBuilder builder, TypeSyntax syntax, IParameterSymbol parameter,
            ObjCCompilationContext context)
        {
            handleRefLikeIndentifierSyntax(builder, syntax, parameter.RefKind != RefKind.None, parameter.Type, context);
            builder.Append(parameter.Name);
        }

        private static void writeObjCFieldIdentifier(CodeBuilder builder, TypeSyntax syntax, IFieldSymbol fieldSymbol, ObjCCompilationContext context)
        {
            handleRefLikeIndentifierSyntax(builder, syntax, false, fieldSymbol.Type, context);
            builder.Append(fieldSymbol.Name);
        }

        private static void writeObjCLocalIdentifier(CodeBuilder builder, TypeSyntax syntax, ILocalSymbol localSymbol, ObjCCompilationContext context)
        {
            handleRefLikeIndentifierSyntax(builder, syntax, false, localSymbol.Type, context);
            builder.Append(localSymbol.Name);
        }

        static void handleRefLikeIndentifierSyntax(CodeBuilder builder, TypeSyntax syntax, bool passByRef, ITypeSymbol typeSymbol, ObjCCompilationContext context)
        {
            if (typeSymbol.GetFullName() != "System.String") // String is handled in invocation
            {
                if (passByRef && syntax.Parent!.IsExpression(ExpressionKind.Assignment) && (syntax.Parent as AssignmentExpressionSyntax)!.Left == syntax)
                {
                    builder.Append("*");
                }
                else if (syntax.Parent.IsKind(SyntaxKind.Argument) && (syntax.Parent as ArgumentSyntax)!.IsRefLike()
                    && (typeSymbol.TypeKind != TypeKind.Struct || typeSymbol.IsObjCPrimitiveType()))
                {
                    builder.Append("&");
                }
            }
        }

        static void writeTypeSymbol(CodeBuilder builder, ITypeSymbol symbol,
            ObjCTypeUsageKind usage, ObjCCompilationContext context, out ObjCTypeKind objcTypeKind)
        {
            // Try to adjust the typename, looking for know types
            string? objCTypeName;
            string fullTypeName;
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                {
                    var namedType = (INamedTypeSymbol)symbol;
                    if (namedType.IsNative())
                    {
                        // Handle CLang types, for example
                        if (namedType.TryGetAttribute<NativeBindingAttribute>(out var binding))
                        {
                            objCTypeName = binding.GetConstructorArgument<string>(0);
                            objcTypeKind = GetUnkwownTypeKind(symbol);
                            TryAdaptType(ref objCTypeName, usage, objcTypeKind);
                            builder.Append(objCTypeName);
                            return;
                        }
                    }

                    if (namedType.IsGenericType)
                    {
                        if (namedType.IsNullable(out var underlyingType))
                            fullTypeName = underlyingType.GetFullName();
                        else
                            fullTypeName = namedType.ConstructedFrom.GetFullName();
                    }
                    else
                    {
                        fullTypeName = symbol.GetFullName();
                    }

                    if (namedType.IsValueType && namedType.TypeKind != TypeKind.Enum
                        && usage == ObjCTypeUsageKind.DeclarationByRef)
                    {
                        // We don't support value types, yet
                        usage = ObjCTypeUsageKind.Declaration;
                    }

                    break;
                }
                case SymbolKind.ArrayType:
                {
                    var arrayType = (IArrayTypeSymbol)symbol;
                    fullTypeName = arrayType.ElementType.GetFullName();
                    break;
                }
                case SymbolKind.TypeParameter:
                    // Nothing to do
                    var parameter = (ITypeParameterSymbol)symbol;
                    fullTypeName = parameter.Name;
                    break;
                default:
                    throw new Exception();
            }

            ObjCTypeKind? tempTypeKind;
            if (IsKnownObjCType(fullTypeName, symbol.Kind, out objCTypeName, out tempTypeKind))
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.NamedType:
                    {
                        var namedType = (INamedTypeSymbol)symbol;
                        if (namedType.IsNullable())
                        {
                            string? boxTypeName;
                            if (!ObjCUtils.TryGetBoxType(fullTypeName, out boxTypeName))
                                throw new NotSupportedException("Unsupported underlying nullable types");

                            objcTypeKind = ObjCTypeKind.Class;
                            TryAdaptType(ref boxTypeName, usage, objcTypeKind);
                            builder.Append(boxTypeName);
                            return;
                        }
                        else
                        {
                            objcTypeKind = tempTypeKind.Value;
                        }

                        // NOTE: We don't append generic parameters here: ObjectiveC has only the so
                        // called "lightweight generics", that are useful only for swift interop
                        TryAdaptType(ref objCTypeName, usage, objcTypeKind);
                        builder.Append(objCTypeName);
                        break;
                    }
                    case SymbolKind.TypeParameter:
                    case SymbolKind.ArrayType:
                    {
                        objcTypeKind = tempTypeKind.Value;
                        TryAdaptType(ref objCTypeName, usage, objcTypeKind);
                        builder.Append(objCTypeName);
                        break;
                    }
                    default:
                        throw new Exception();
                }
            }
            else
            {
                objcTypeKind = GetUnkwownTypeKind(symbol);
                switch (symbol.Kind)
                {
                    case SymbolKind.NamedType:
                    {
                        var namedType = (INamedTypeSymbol)symbol;
                        if (namedType.IsNullable(out var underlyingType))
                            objCTypeName = underlyingType.GetObjCName(context);
                        else
                            objCTypeName = symbol.GetObjCName(context);

                        // NOTE: in case of named types, we don't append generic parameters here:
                        // ObjectiveC has only the so called "lightweight generics", that are useful
                        // only for swift interop
                        TryAdaptType(ref objCTypeName, usage, objcTypeKind);
                        builder.Append(objCTypeName);
                        break;
                    }
                    case SymbolKind.TypeParameter:
                    {
                        objCTypeName = symbol.GetObjCName(context);
                        TryAdaptType(ref objCTypeName, usage, objcTypeKind);
                        builder.Append(objCTypeName);
                        break;
                    }
                    case SymbolKind.ArrayType:
                    {
                        var arrayType = (IArrayTypeSymbol)symbol;
                        if (arrayType.ElementType.IsValueType && arrayType.ElementType.ShouldDiscard(context.Conversion))
                        {
                            // TODO: We can partially support array of value types for ignored types

                        }
                        // TODO: the long term is just support value types as struct
                        throw new NotSupportedException("Array type with non primitive types are not supported");
                    }
                    default:
                        throw new Exception();
                }
            }
        }

        static bool IsKnownObjCType(string fullTypeName, SymbolKind typeKind,
            [NotNullWhen(true)]out string? knownObjCType, [NotNullWhen(true)]out ObjCTypeKind? objcTypeKind)
        {
            if (IsKnowSimpleObjCType(fullTypeName, typeKind, out knownObjCType, out objcTypeKind))
                return true;

            // Known reference types
            switch (fullTypeName)
            {
                case "System.Runtime.InteropServices.HandleRef":
                {
                    knownObjCType = "CBHandleRef";
                    objcTypeKind = ObjCTypeKind.Class;
                    return true;
                }
                case "System.Object":
                {
                    knownObjCType = "NSObject";
                    objcTypeKind = ObjCTypeKind.Class;
                    return true;
                }
                case "System.String":
                {
                    knownObjCType = "NSString";
                    objcTypeKind = ObjCTypeKind.Class;
                    return true;
                }
                case "System.Exception":
                {
                    knownObjCType = "CBException";
                    objcTypeKind = ObjCTypeKind.Class;
                    return true;
                }
                case "System.NotImplementedException":
                {
                    knownObjCType = "CBException";
                    objcTypeKind = ObjCTypeKind.Class;
                    return true;
                }
                case "CodeBinder.IObjectFinalizer":
                {
                    objcTypeKind = ObjCTypeKind.Protocol;
                    knownObjCType = "CBIObjectFinalizer";
                    return true;
                }
                case "CodeBinder.HandledObjectFinalizer":
                {
                    objcTypeKind = ObjCTypeKind.Class;
                    knownObjCType = "CBHandledObjectFinalizer";
                    return true;
                }
                case "CodeBinder.FinalizableObject":
                {
                    objcTypeKind = ObjCTypeKind.Class;
                    knownObjCType = "CBFinalizableObject";
                    return true;
                }
                case "CodeBinder.HandledObjectBase":
                {
                    objcTypeKind = ObjCTypeKind.Class;
                    knownObjCType = "CBHandledObjectBase";
                    return true;
                }
                case "CodeBinder.HandledObject<BaseT>":
                {
                    objcTypeKind = ObjCTypeKind.Class;
                    knownObjCType = "CBHandledObject";
                    return true;
                }
                case "System.IDisposable":
                {
                    knownObjCType = "CBIDisposable";
                    objcTypeKind = ObjCTypeKind.Protocol;
                    return true;
                }
                case "System.Collections.Generic.IReadOnlyList<out T>":
                {
                    knownObjCType = "CBIReadOnlyList";
                    objcTypeKind = ObjCTypeKind.Protocol;
                    return true;
                }
                case "System.Collections.Generic.IEqualityComparer<in T>":
                {
                    knownObjCType = "CBIEqualityCompararer";
                    objcTypeKind = ObjCTypeKind.Protocol;
                    return true;
                }
                case "System.Collections.Generic.IEnumerable<out T>":
                {
                    knownObjCType = "NSFastEnumeration";
                    objcTypeKind = ObjCTypeKind.Protocol;
                    return true;
                }
                case "System.Collections.Generic.List<T>":
                {
                    knownObjCType = "NSMutableArray";
                    objcTypeKind = ObjCTypeKind.Class;
                    return true;
                }
                case "System.Collections.Generic.KeyValuePair<TKey, TValue>":
                {
                    knownObjCType = "CBKeyValuePair";
                    objcTypeKind = ObjCTypeKind.Class;
                    return true;
                }
                default:
                {
                    knownObjCType = null;
                    objcTypeKind = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Simple types are primitives value types, array types + void
        /// </summary>
        static bool IsKnowSimpleObjCType(string fullTypeName, SymbolKind typekind,
            [NotNullWhen(true)]out string? knownObjCType, [NotNullWhen(true)]out ObjCTypeKind? objcTypeKind)
        {
            switch (fullTypeName)
            {
                case "System.Void":
                {
                    objcTypeKind = ObjCTypeKind.Void;
                    knownObjCType = "void";
                    return true;

                }
            }

            switch (typekind)
            {
                case SymbolKind.ArrayType:
                {
                    if (ObjCUtils.TryGetArrayBoxType(fullTypeName, out knownObjCType))
                    {
                        objcTypeKind = ObjCTypeKind.Class;
                        return true;
                    }
                    break;
                }
                case SymbolKind.Parameter:
                {
                    if (TryGetPrimitiveParameterType(fullTypeName, out knownObjCType))
                    {
                        objcTypeKind = ObjCTypeKind.Class;
                        return true;
                    }
                    break;
                }
                default:
                {
                    if (ObjCUtils.TryGetObjCPrimitiveType(fullTypeName, out knownObjCType))
                    {
                        objcTypeKind = ObjCTypeKind.Value;
                        return true;
                    }
                    break;
                }
            }

            objcTypeKind = null;
            return false;
        }

        // Get Type kind 
        static ObjCTypeKind GetUnkwownTypeKind(ITypeSymbol symbol)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Interface:
                    return ObjCTypeKind.Protocol;
                case TypeKind.Class:
                    return ObjCTypeKind.Class;
                case TypeKind.Struct:
                {
                    Debug.Assert(!symbol.IsObjCPrimitiveType());
                    return ObjCTypeKind.Class;
                }
                case TypeKind.TypeParameter:
                {
                    var typeparam = (ITypeParameterSymbol)symbol;
                    Debug.Assert(typeparam.ConstraintTypes.Length == 1);
                    return GetUnkwownTypeKind(typeparam.ConstraintTypes[0]);
                }
                default:
                    return ObjCTypeKind.Value;
            }
        }

        static ObjCTypeReachability GetReachability(ITypeSymbol symbol, ObjCCompilationContext context)
        {
            if (symbol.HasAttribute<IgnoreAttribute>())
                return ObjCTypeReachability.External;

            // FIX-ME: This is unrealiable, there should be better check to ensure
            // the symbol is defined in this Compilation
            if (!context.Namespaces.Contains(symbol.GetContainingNamespace()))
                return ObjCTypeReachability.External;

            if (symbol.HasAccessibility(Accessibility.Public))
                return ObjCTypeReachability.Public;
            else
                return ObjCTypeReachability.Internal;
        }

        /// <summary>
        /// Returs parameter type for simple value type 
        /// </summary>
        /// <param name="fullTypeName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        static bool TryGetPrimitiveParameterType(string fullTypeName, [NotNullWhen(true)]out string? typeName)
        {
            switch (fullTypeName)
            {
                // Boxed types
                case "System.UIntPtr":
                case "System.IntPtr":
                case "System.Boolean":
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int64":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                    typeName = "NSNumber *";
                    return true;
                default:
                    typeName = null;
                    return false;
            }
        }

        /// <summary>
        /// Try fix the ref type based on the usage
        /// </summary>
        static void TryAdaptType(ref string type, ObjCTypeUsageKind usage, ObjCTypeKind typeKind)
        {
            switch (typeKind)
            {
                case ObjCTypeKind.Void:
                {
                    if (usage != ObjCTypeUsageKind.Declaration)
                        throw new NotSupportedException($"Unsupported usage {usage} for type void");

                    break;
                }
                case ObjCTypeKind.Value:
                {
                    switch (usage)
                    {
                        case ObjCTypeUsageKind.Normal:
                        case ObjCTypeUsageKind.Declaration:
                            break;
                        case ObjCTypeUsageKind.DeclarationByRef:
                        case ObjCTypeUsageKind.Pointer:
                            type = $"{type} *";
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    break;
                }
                case ObjCTypeKind.Class:
                case ObjCTypeKind.Protocol:
                {
                    switch (usage)
                    {
                        case ObjCTypeUsageKind.Declaration:
                        case ObjCTypeUsageKind.Pointer:
                        {
                            // Handle special declaration for protocols
                            if (typeKind == ObjCTypeKind.Protocol)
                                type = $"id<{type}>";
                            else
                                type = $"{type} *";
                            break;
                        }
                        case ObjCTypeUsageKind.DeclarationByRef:
                        {
                            // Handle special declaration for protocols
                            if (typeKind == ObjCTypeKind.Protocol)
                                type = $"id<{type}> *";
                            else
                                // Ensure arc is working https://stackoverflow.com/a/39053139
                                type = $"{type} * __strong *";
                            break;
                        }
                        case ObjCTypeUsageKind.Normal:
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    break;
                }
            }
        }
    }
}
