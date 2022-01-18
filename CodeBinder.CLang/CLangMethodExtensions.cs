// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using CodeBinder.Shared;
using CodeBinder.Attributes;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace CodeBinder.CLang
{
    public static class CLangMethodExtensions
    {
        enum ParameterType
        {
            Regular,
            ByRef,
            Return
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
                isByRef ? ParameterType.ByRef : ParameterType.Regular, cppMethod, out suffix);
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

        public static string GetCLangType(this VariableDeclarationSyntax declaration, ICompilationContextProvider provider)
        {
            var symbol = declaration.Type.GetTypeSymbol(provider);
            return getCLangType(symbol, declaration.Variables[0].GetAttributes(provider), ParameterType.Regular, false, out _);
        }

        public static string GetCLangType(this ParameterSyntax parameter, ICompilationContextProvider provider)
        {
            var symbol = parameter.Type!.GetTypeSymbol(provider);
            return getCLangType(symbol, parameter.GetAttributes(provider), ParameterType.Regular, false, out _);
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
            return getCLangReturnType(cppMethod, symbol);
        }

        public static string GetCLangReturnType(this DelegateDeclarationSyntax dlg, ICompilationContextProvider provider)
        {
            // TODO: Should be possible to prpare static cpp trampolines also for delegates.
            // Maybe not so easy
            var symbol = dlg.GetDeclaredSymbol<INamedTypeSymbol>(provider);
            return getCLangReturnType(false, symbol.DelegateInvokeMethod!);
        }

        private static string getCLangReturnType(bool cppMethod, IMethodSymbol method)
        {
            string? suffix;
            string type = getCLangType(method.ReturnType, method.GetReturnTypeAttributes(),
                ParameterType.Return, cppMethod, out suffix);
            if (suffix == null)
                return type;
            else
                return $"{type} {suffix}";
        }

        private static string getCLangType(ITypeSymbol symbol, IEnumerable<AttributeData> attributes,
            ParameterType type, bool cppMethod, out string? suffix)
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

                    if (type == ParameterType.ByRef)
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
                    if (type == ParameterType.Return)
                    {
                        suffix = null;
                        if (attributes.HasAttribute<ConstAttribute>())
                            constParameter = true;
                    }
                    else
                    {
                        if (!attributes.HasAttribute<OutAttribute>())
                            constParameter = true;

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

                    var arrayType = (IArrayTypeSymbol)symbol;
                    if (!tryGetCLangBinder(arrayType.ElementType, out bindedType))
                    {
                        string typeName = arrayType.ElementType.GetFullName();
                        switch (type)
                        {
                            case ParameterType.Regular:
                                bindedType = getCLangType(typeName, attributes);
                                break;
                            case ParameterType.Return:
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
                        switch (type)
                        {
                            case ParameterType.Regular:
                            {
                                if (cppMethod)
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
                            case ParameterType.Return:
                            {
                                if (cppMethod)
                                    bindedType = "cbstringr";
                                else
                                    bindedType = "cbstring";
                                break;
                            }
                            case ParameterType.ByRef:
                            {
                                if (cppMethod)
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
                            if (type == ParameterType.ByRef)
                                bindedType = $"{bindedType}*";
                        }
                        else
                        {
                            switch (type)
                            {
                                case ParameterType.Regular:
                                    bindedType = getCLangType(typeName, attributes);
                                    break;
                                case ParameterType.Return:
                                    bindedType = getCLangType(typeName, attributes);
                                    break;
                                case ParameterType.ByRef:
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
                case "System.Boolean":
                    // TODO: Check this has the attribute [MarshalAs(UnmanageType.I1)]
                    return "cbbool";
                case "System.Char":
                    return "cbchar_t";
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
                case "System.Char":
                    return "cbchar_t*";
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
