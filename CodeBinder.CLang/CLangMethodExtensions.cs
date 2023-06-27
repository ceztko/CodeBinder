// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using CodeBinder.Shared.CSharp;
using CodeBinder.Utils;
using CodeBinder.Shared;
using CodeBinder.Attributes;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace CodeBinder.CLang
{
    public static class CLangMethodExtensions
    {
        enum DeclarationType
        {
            /// <summary>
            /// Parameter, field or local declaration
            /// </summary>
            Regular,
            /// <summary>
            /// Parameter by reference declaration
            /// </summary>
            ParamByRef,
            /// <summary>
            /// Return type declaration
            /// </summary>
            Return,
        }

        [Flags]
        enum DeclarationFlags
        {
            None = 0,
            /// <summary>
            /// Use a C++ declaration
            /// </summary>
            CppMethod = 1,
            /// <summary>
            /// Don't use subscript syntax for array declarations
            /// </summary>
            NoSubscriptSyntax = 2,
        }

        public static CodeBuilder Append(this CodeBuilder builder, ParameterSyntax parameter,
            ICompilationContextProvider provider)
        {
            return Append(builder, parameter, false, provider);
        }

        public static CodeBuilder Append(this CodeBuilder builder, ParameterSyntax parameter,
            bool cppMethod, ICompilationContextProvider provider)
        {
            bool isByRef = parameter.IsRef() || parameter.IsOut();
            var symbol = parameter.Type!.GetTypeSymbol(provider);
            string? suffix;
            string type = getCLangType(symbol, parameter.GetAttributes(provider),
                isByRef ? DeclarationType.ParamByRef : DeclarationType.Regular,
                cppMethod ? DeclarationFlags.CppMethod : DeclarationFlags.None, out suffix);
            builder.Append(type).Space().Append(parameter.Identifier.Text);
            if (suffix != null)
                builder.Append(suffix);

            return builder;
        }

        static bool tryGetCLangBinder(this ITypeSymbol typeSymbol, bool pointerType,
            [NotNullWhen(true)] out string? binderStr)
        {
            if (!tryGetCLangBinder(typeSymbol, out binderStr))
            {
                binderStr = null;
                return false;
            }

            if (pointerType)
                binderStr = $"{binderStr}*";

            return true;
        }

        public static bool TryGetCLangBinder(this BaseTypeDeclarationSyntax typeSyntax,
            ICompilationContextProvider provider, [NotNullWhen(true)] out string? binderStr)
        {
            return TryGetCLangBinder(typeSyntax, false, provider, out binderStr);
        }

        public static bool TryGetCLangBinder(this BaseTypeDeclarationSyntax typeSyntax, bool pointerType,
            ICompilationContextProvider provider, [NotNullWhen(true)] out string? binderStr)
        {
            var symbol = typeSyntax.GetTypeSymbol(provider);
            return tryGetCLangBinder(symbol, pointerType, out binderStr);
        }

        public static bool TryGetCLangBinder(this ParameterSyntax parameter,
            ICompilationContextProvider provider, [NotNullWhen(true)] out string? binderStr)
        {
            return TryGetCLangBinder(parameter, false, provider, out binderStr);
        }

        public static bool TryGetCLangBinder(this ParameterSyntax parameter, bool pointerType,
            ICompilationContextProvider provider, [NotNullWhen(true)] out string? binderStr)
        {
            var symbol = parameter.Type!.GetTypeSymbol(provider);
            if (tryGetCLangBinder(symbol, pointerType, out binderStr))
            {
                return true;
            }
            else
            {
                var binder = parameter.GetAttributes(provider).FirstOrDefault((item) => item.Inherits<NativeTypeBinder>());
                if (binder == null)
                {
                    binderStr = null;
                    return false;
                }

                if (pointerType)
                    binderStr = $"{binder.AttributeClass!.Name}*";
                else
                    binderStr = $"{binder.AttributeClass!.Name}";

                return true;
            }
        }

        static bool tryGetCLangBinder(ITypeSymbol symbol, [NotNullWhen(true)] out string? binderStr)
        {
            var attributes = symbol.GetAttributes();
            if (!attributes.TryGetAttribute<NativeBindingAttribute>(out var attribute))
            {
                binderStr = null;
                return false;
            }

            binderStr = attribute.GetConstructorArgument<string>(0);
            return true;
        }

        public static string GetCLangDeclaration(this VariableDeclarationSyntax declaration, ICompilationContextProvider provider)
        {
            var symbol = declaration.Type.GetTypeSymbol(provider);
            var type = getCLangType(symbol, declaration.Variables[0].GetAttributes(provider),
                DeclarationType.Regular, DeclarationFlags.None, out _);
            return $"{type} {declaration.Variables[0].Identifier.Text}";
        }

        public static string GetCLangDeclaration(this ParameterSyntax parameter, ICompilationContextProvider provider)
        {
            return GetCLangDeclaration(parameter, false, provider);
        }

        public static string GetCLangDeclaration(this ParameterSyntax parameter, bool noFixedArray, ICompilationContextProvider provider)
        {
            var symbol = parameter.Type!.GetTypeSymbol(provider);
            string? suffix;
            var type = getCLangType(symbol, parameter.GetAttributes(provider), DeclarationType.Regular,
                noFixedArray ? DeclarationFlags.NoSubscriptSyntax : DeclarationFlags.None, out suffix);
            if (suffix == null)
                return $"{type} {parameter.Identifier.Text}";
            else
                return $"{type} {parameter.Identifier.Text}{suffix}";
        }

        public static string GetCLangType(this SpecialType type)
        {
            switch(type)
            {
                case SpecialType.System_Boolean:
                    return "cbbool";
                case SpecialType.System_Byte:
                    return "uint8_t";
                case SpecialType.System_SByte:
                    return "int8_t";
                case SpecialType.System_UInt16:
                    return "uint16_t";
                case SpecialType.System_Int16:
                    return "int16_t";
                case SpecialType.System_UInt32:
                    return "uint32_t";
                case SpecialType.System_Int32:
                    return "int32_t";
                case SpecialType.System_UInt64:
                    return "uint64_t";
                case SpecialType.System_Int64:
                    return "int64_t";
                case SpecialType.System_Single:
                    return "float";
                case SpecialType.System_Double:
                    return "double";
                case SpecialType.System_IntPtr:
                    return "void*";
                case SpecialType.System_UIntPtr:
                    return "void*";
                default:
                    throw new Exception($"Unsupported by type {type}");
            }
        }

        public static string GetCLangMethodName(this MethodDeclarationSyntax method)
        {
            return method.GetName();
        }

        internal static string GetCLangReturnType(this MethodDeclarationSyntax method,
            ICompilationContextProvider provider)
        {
            return GetCLangReturnType(method, false, provider);
        }

        internal static string GetCLangReturnType(this MethodDeclarationSyntax method, bool cppMethod, ICompilationContextProvider provider)
        {
            var symbol = method.GetDeclaredSymbol<IMethodSymbol>(provider);
            return GetCLangReturnType(symbol, cppMethod);
        }

        public static string GetCLangReturnType(this DelegateDeclarationSyntax dlg, ICompilationContextProvider provider)
        {
            // TODO: Should be possible to prpare static cpp trampolines also for delegates.
            // Maybe not so easy
            var symbol = dlg.GetDeclaredSymbol<INamedTypeSymbol>(provider);
            return GetCLangReturnType(symbol.DelegateInvokeMethod!, false);
        }

        public static string GetCLangReturnType(this IMethodSymbol method, bool cppMethod = false)
        {
            string? suffix;
            string type = getCLangType(method.ReturnType, method.GetReturnTypeAttributes(),
                DeclarationType.Return, cppMethod ? DeclarationFlags.CppMethod : DeclarationFlags.None, out suffix);
            if (suffix == null)
                return type;
            else
                return $"{type} {suffix}";
        }

        private static string getCLangType(ITypeSymbol symbol, IEnumerable<AttributeData> attributes,
            DeclarationType declType, DeclarationFlags declFlags, out string? suffix)
        {
            bool constParameter = false;
            string? bindedType;
            switch (symbol.TypeKind)
            {
                // Handle some special types first
                case TypeKind.Enum:
                {
                    suffix = null;
                    string? binded;
                    if (!tryGetCLangBinder(symbol, out binded))
                        throw new Exception("Could not find the binder for the parameter");

                    if (declType == DeclarationType.ParamByRef)
                        return $"{binded} *";
                    else
                        return binded;
                }
                case TypeKind.Delegate:
                {
                    suffix = null;
                    string? binded;
                    if (!tryGetCLangBinder(symbol, out binded))
                        throw new Exception("Could not find the binder for the parameter");

                    return binded;
                }
                case TypeKind.Array:
                {
                    if (declType == DeclarationType.Return)
                    {
                        suffix = null;
                        if (attributes.HasAttribute<ConstAttribute>())
                            constParameter = true;
                    }
                    else
                    {
                        if (!attributes.HasAttribute<OutAttribute>())
                            constParameter = true;

                        if (declFlags.HasFlag(DeclarationFlags.NoSubscriptSyntax))
                        {
                            suffix = null;
                        }
                        else
                        {
                            int fixedSizeArray;
                            if (attributes.TryGetAttribute<MarshalAsAttribute>(out var marshalAsAttr) &&
                                marshalAsAttr.TryGetNamedArgument("SizeConst", out fixedSizeArray))
                            {
                                suffix = $"[{fixedSizeArray}]";
                            }
                            else
                            {
                                suffix = "[]";
                            }
                        }
                    }

                    var arrayType = (IArrayTypeSymbol)symbol;
                    if (!tryGetCLangBinder(arrayType.ElementType, out bindedType))
                    {
                        string typeName = arrayType.ElementType.GetFullName();
                        switch (declType)
                        {
                            case DeclarationType.Regular:
                                if (declFlags.HasFlag(DeclarationFlags.NoSubscriptSyntax))
                                    bindedType = getCLangPointerType(typeName, attributes);
                                else
                                    bindedType = getCLangType(typeName, attributes);
                                break;
                            case DeclarationType.Return:
                                bindedType = getCLangPointerType(typeName, attributes);
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                    }

                    break;
                }
                default:
                {
                    suffix = null;
                    string typeName = symbol.GetFullName();

                    // TODO: Optmize CLR types with ITypeSymbol.SpecialType, handling of constParameter,
                    // CHECK-ME evaluate supporting string arrays
                    if (typeName == "CodeBinder.cbstring")
                    {
                        switch (declType)
                        {
                            case DeclarationType.Regular:
                            {
                                if (declFlags.HasFlag(DeclarationFlags.CppMethod))
                                {
                                    constParameter |= true;
                                    bindedType = "cbstringp&";
                                }
                                else
                                {
                                    bindedType = "cbstring";
                                }

                                break;
                            }
                            case DeclarationType.Return:
                            {
                                if (declFlags.HasFlag(DeclarationFlags.CppMethod))
                                    bindedType = "cbstringr";
                                else
                                    bindedType = "cbstring";
                                break;
                            }
                            case DeclarationType.ParamByRef:
                            {
                                if (declFlags.HasFlag(DeclarationFlags.CppMethod))
                                    bindedType = "cbstringpr&";
                                else
                                    bindedType = "cbstring*";
                                break;
                            }
                            default:
                                throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        if (tryGetCLangBinder(symbol, out bindedType))
                        {
                            if (declType == DeclarationType.ParamByRef)
                                bindedType = $"{bindedType}*";
                        }
                        else
                        {
                            switch (declType)
                            {
                                case DeclarationType.Regular:
                                case DeclarationType.Return:
                                    bindedType = getCLangType(typeName, attributes);
                                    break;
                                case DeclarationType.ParamByRef:
                                    bindedType = getCLangPointerType(typeName, attributes);
                                    break;
                                default:
                                    throw new NotSupportedException();
                            }
                        }
                    }
                    break;
                }
            }

            if (constParameter)
                return $"const {bindedType}";
            else
                return bindedType;
        }

        static string getCLangType(string typeName, IEnumerable<AttributeData> attributes)
        {
            switch (typeName)
            {
                case "System.Void":
                    return "void";
                case "System.Runtime.InteropServices.HandleRef":
                case "System.IntPtr":
                {
                    var binder = attributes.FirstOrDefault((item) => item.Inherits<NativeTypeBinder>());
                    if (binder != null)
                        return $"{binder.AttributeClass!.Name}*";

                    return "void*";
                }
                case "CodeBinder.cbstring":
                    return "cbstring";
                case "System.Boolean":
                    // TODO: Check this has the attribute [MarshalAs(UnmanageType.I1)]
                    return "cbbool";
                case "System.Byte":
                    return "uint8_t";
                case "System.SByte":
                    return "int8_t";
                case "System.Int16":
                    return "int16_t";
                case "System.UInt16":
                    return "uint16_t";
                case "System.Int32":
                    // TODO: Add CodeBinder.Attributes attribute to specify explicitly sized 32 bit signed integer
                    return "int";
                case "System.UInt32":
                    return "uint32_t";
                case "System.Int64":
                    return "int64_t";
                case "System.UInt64":
                    return "uint64_t";
                case "System.Single":
                    return "float";
                case "System.Double":
                    return "double";
                default:
                    throw new Exception($"Unsupported by type {typeName}");
            }
        }

        static string getCLangPointerType(string typeName, IEnumerable<AttributeData> attributes)
        {
            switch (typeName)
            {
                case "System.Runtime.InteropServices.HandleRef":
                case "System.IntPtr":
                {
                    var binder = attributes.FirstOrDefault((item) => item.Inherits<NativeTypeBinder>());
                    if (binder != null)
                        return $"{binder.AttributeClass!.Name}**";

                    return "void**";
                }
                case "System.Boolean":
                    // TODO: Check this has the attribute [MarshalAs(UnmanageType.I1)]
                    return "cbbool*";
                case "System.Byte":
                    return "uint8_t*";
                case "System.SByte":
                    return "int8_t*";
                case "System.Int16":
                    return "int16_t*";
                case "System.UInt16":
                    return "uint16_t*";
                case "System.Int32":
                    // TODO: Add CodeBinder.Attributes attribute to specify explicitly sized 32 bit signed integer
                    return "int*";
                case "System.UInt32":
                    return "uint32_t*";
                case "System.Int64":
                    return "int64_t*";
                case "System.UInt64":
                    return "uint64_t*";
                case "System.Single":
                    return "float*";
                case "System.Double":
                    return "double*";
                default:
                    throw new Exception($"Unsupported by type {typeName}");
            }
        }
    }
}
