// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
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
            if (!string.IsNullOrEmpty(defaultLiteral))
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
                    return "NULL";
                case "System.Boolean":
                    return "NO";
                case "System.Char":
                    return "unichar()";
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
                case SyntaxKind.CharKeyword:
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
                    throw new Exception();
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
                case SyntaxKind.CharKeyword:
                    return "unichar";
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
                    throw new Exception();
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
            writeTypeSymbol(builder, typeSymbol.GetFullName(), typeSymbol, ObjCTypeUsageKind.Normal, context, out objcTypeKind);
            return builder.ToString();
        }

        public static ObjCTypeInfo GetObjCTypeInfo(this TypeSyntax type, ObjCCompilationContext context)
        {
            var builder = new CodeBuilder();
            var typeSymbol = type.GetTypeSymbol(context);
            string fullName = typeSymbol.GetFullName();
            var ret = new ObjCTypeInfo();
            writeTypeSymbol(builder, typeSymbol.GetFullName(), typeSymbol, ObjCTypeUsageKind.Normal, context, out ret.Kind);
            ret.Reachability = GetReachability(typeSymbol, context);
            ret.TypeName = builder.ToString();
            return ret;
        }

        public static string GetObjCType(this TypeSyntax type, ObjCTypeUsageKind displayKind, ObjCCompilationContext context)
        {
            var builder = new CodeBuilder();
            ObjCTypeKind objcTypeKind;
            var typeSymbol = type.GetTypeSymbol(context);
            writeTypeSymbol(builder, typeSymbol.GetFullName(), typeSymbol, displayKind, context, out objcTypeKind);
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
                    writeTypeSymbol(builder, typeSymbol.GetFullName(), typeSymbol, usageKind, context, out objcTypeKind);
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
                case SymbolKind.Field:
                {
                    writeObjCIdentifier(builder, syntax, symbol, context);
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

        static void writeObjCMethodIdentifier(CodeBuilder builder, TypeSyntax syntax, IMethodSymbol method,
            ObjCCompilationContext context)
        {
            SymbolReplacement? replacement;
            string objCMethodName;
            if (method.HasObjCReplacement(out replacement))
            {
                objCMethodName = replacement.Name;
            }
            else
            {
                if (method.IsNative())
                {
                    objCMethodName = method.Name;
                }
                else
                {
                    objCMethodName = context.Conversion.MethodsLowerCase ? method.Name.ToObjCCase() : method.Name;
                }
            }

            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.IdentifierName:
                {
                    builder.Append(objCMethodName);
                    break;
                }
                case SyntaxKind.GenericName:
                {
                    // NOTE: Don't append generic parameters here: ObjectiveC does not support
                    // generic methods
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

        static void writeObjCParameterIdentifier(CodeBuilder builder, TypeSyntax syntax, IParameterSymbol parameter,
            ObjCCompilationContext context)
        {
            void writeBoxValueAccess()
            {
                writeObjCIdentifier(builder, syntax, parameter, context);
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

            writeObjCIdentifier(builder, syntax, parameter, context);
        }

        static void writeObjCIdentifier(CodeBuilder builder, TypeSyntax syntax, ISymbol symbol,
            ObjCCompilationContext context)
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

        static void writeTypeSymbol(CodeBuilder builder, string fullTypeName, ITypeSymbol symbol,
            ObjCTypeUsageKind usage, ObjCCompilationContext context, out ObjCTypeKind objcTypeKind)
        {
            // Try to adjust the typename, looking for know types
            string? objCTypeName;
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
                            TryAdaptRefType(ref objCTypeName, usage, objcTypeKind);
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

                    if (namedType.IsValueType && usage == ObjCTypeUsageKind.DeclarationByRef)
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
                    break;
                default:
                    throw new Exception();
            }

            ObjCTypeKind? tempTypeKind;
            if (IsKnownObjCType(fullTypeName, symbol.Kind, usage, out objCTypeName, out tempTypeKind))
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
                            TryAdaptRefType(ref boxTypeName, usage, objcTypeKind);
                            builder.Append(boxTypeName);
                            return;
                        }
                        else
                        {
                            objcTypeKind = tempTypeKind.Value;
                        }

                        // NOTE: We don't append generic parameters here: ObjectiveC has only the so
                        // called "lightweight generics", that are useful only for swift interop
                        builder.Append(objCTypeName);
                        break;
                    }
                    case SymbolKind.TypeParameter:
                    case SymbolKind.ArrayType:
                    {
                        objcTypeKind = tempTypeKind.Value;
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
                        TryAdaptRefType(ref objCTypeName, usage, objcTypeKind);
                        builder.Append(objCTypeName);
                        break;
                    }
                    case SymbolKind.TypeParameter:
                    {
                        objCTypeName = symbol.GetObjCName(context);
                        TryAdaptRefType(ref objCTypeName, usage, objcTypeKind);
                        builder.Append(objCTypeName);
                        break;
                    }
                    case SymbolKind.ArrayType:
                    {
                        throw new NotSupportedException("Array type with non primitive types are not supported");
                    }
                    default:
                        throw new Exception();
                }
            }
        }

        static CodeBuilder Append(this CodeBuilder builder, ITypeSymbol symbol, ObjCCompilationContext context)
        {
            writeTypeSymbol(builder, symbol.GetFullName(), symbol, ObjCTypeUsageKind.Normal, context, out var objcTypeKind);
            return builder;
        }

        static bool IsKnownObjCType(string fullTypeName, SymbolKind typeKind, ObjCTypeUsageKind usage,
            [NotNullWhen(true)]out string? knownObjCType, [NotNullWhen(true)]out ObjCTypeKind? objcTypeKind)
        {
            if (IsKnowSimpleObjCType(fullTypeName, typeKind, usage, out knownObjCType))
            {
                // Primitives value types + void
                objcTypeKind = ObjCTypeKind.Value;
                return true;
            }

            // Known reference types
            switch (fullTypeName)
            {
                case "System.Runtime.InteropServices.HandleRef":
                {
                    knownObjCType = "CBHandleRef";
                    objcTypeKind = ObjCTypeKind.Class;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.Object":
                {
                    knownObjCType = "NSObject";
                    objcTypeKind = ObjCTypeKind.Class;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.String":
                {
                    knownObjCType = "NSString";
                    objcTypeKind = ObjCTypeKind.Class;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.Exception":
                {
                    knownObjCType = "CBException";
                    objcTypeKind = ObjCTypeKind.Class;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.NotImplementedException":
                {
                    knownObjCType = "CBException";
                    objcTypeKind = ObjCTypeKind.Class;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.IDisposable":
                {
                    knownObjCType = "CBIDisposable";
                    objcTypeKind = ObjCTypeKind.Protocol;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.Collections.Generic.IReadOnlyList<out T>":
                {
                    knownObjCType = "CBIReadOnlyList";
                    objcTypeKind = ObjCTypeKind.Protocol;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.Collections.Generic.IEqualityComparer<in T>":
                {
                    knownObjCType = "CBIEqualityCompararer";
                    objcTypeKind = ObjCTypeKind.Protocol;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.Collections.Generic.IEnumerable<out T>":
                {
                    knownObjCType = "NSFastEnumeration";
                    objcTypeKind = ObjCTypeKind.Protocol;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.Collections.Generic.List<T>":
                {
                    knownObjCType = "NSMutableArray";
                    objcTypeKind = ObjCTypeKind.Class;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
                    return true;
                }
                case "System.Collections.Generic.KeyValuePair<TKey, TValue>":
                {
                    knownObjCType = "CBKeyValuePair";
                    objcTypeKind = ObjCTypeKind.Class;
                    TryAdaptRefType(ref knownObjCType, usage, objcTypeKind.Value);
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
        /// Simple types are primitives value types + void
        /// </summary>
        static bool IsKnowSimpleObjCType(string fullTypeName, SymbolKind typekind, ObjCTypeUsageKind usage,
            [NotNullWhen(true)]out string? knownObjCType)
        {
            switch (typekind)
            {
                case SymbolKind.ArrayType:
                {
                    if (ObjCUtils.TryGeArrayBoxType(fullTypeName, out knownObjCType))
                    {
                        TryAdaptRefType(ref knownObjCType, usage, ObjCTypeKind.Class);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                case SymbolKind.Parameter:
                {
                    // Type parameter usage must not be a declaration
                    Debug.Assert(usage == ObjCTypeUsageKind.Normal);
                    return TryGetSimpleGenericType(fullTypeName, out knownObjCType);
                }
                default:
                {
                    return ObjCUtils.TryGetSimpleType(fullTypeName, out knownObjCType);
                }
            }
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
                    Debug.Assert(!symbol.IsCLRPrimitiveType());
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
        static bool TryGetSimpleGenericType(string fullTypeName, [NotNullWhen(true)]out string? typeName)
        {
            switch (fullTypeName)
            {
                // Boxed types
                case "System.UIntPtr":
                case "System.IntPtr":
                case "System.Boolean":
                case "System.Char":
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
        static void TryAdaptRefType(ref string type, ObjCTypeUsageKind usage, ObjCTypeKind typeKind)
        {
            if (typeKind == ObjCTypeKind.Value)
            {
                // This methods adapts only ref types
                return;
            }

            switch (usage)
            {
                case ObjCTypeUsageKind.Declaration:
                {
                    // Handle special declaration for protocols
                    if (typeKind == ObjCTypeKind.Protocol)
                        type = $"id<{type}>";
                    else
                        type = $"{type} *";
                    return;
                }
                case ObjCTypeUsageKind.DeclarationByRef:
                {
                    // Handle special declaration for protocols
                    if (typeKind == ObjCTypeKind.Protocol)
                        type = $"id<{type}> *";
                    else
                        type = $"{type} **";
                    return;
                }
                case ObjCTypeUsageKind.Normal:
                    return;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
