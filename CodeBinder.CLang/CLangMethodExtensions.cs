// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using CodeBinder.Shared;
using CodeBinder.Attributes;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CodeBinder.CLang
{
    static class CLangMethodExtensions
    {
        enum ParameterType
        {
            Regular,
            ByRef,
            Return
        }

        public static CodeBuilder Append(this CodeBuilder builder, ParameterSyntax parameter, ICompilationContextProvider provider)
        {
            bool isByRef = parameter.IsRef() || parameter.IsOut();
            var symbol = parameter.Type!.GetTypeSymbol(provider);
            string? suffix;
            string type = getCLangType(symbol, parameter.GetAttributes(provider),
                isByRef ? ParameterType.ByRef : ParameterType.Regular, out suffix);
            builder.Append(type).Space().Append(parameter.Identifier.Text);
            if (suffix != null)
                builder.Append(suffix);

            return builder;
        }

        public static string GetCLangMethodName(this MethodDeclarationSyntax method, bool widechar)
        {
            if (widechar)
                return $"{method.GetName()}W";
            else
                return method.GetName();
        }

        public static string GetCLangReturnType(this MethodDeclarationSyntax method, ICompilationContextProvider provider)
        {
            var symbol = method.GetDeclaredSymbol<IMethodSymbol>(provider);
            return getCLangReturnType(symbol);
        }

        public static string GetCLangReturnType(this DelegateDeclarationSyntax dlg, ICompilationContextProvider provider)
        {
            var symbol = dlg.GetDeclaredSymbol<INamedTypeSymbol>(provider);
            return getCLangReturnType(symbol.DelegateInvokeMethod!);
        }

        private static string getCLangReturnType(IMethodSymbol method)
        {
            string? suffix;
            string type = getCLangType(method.ReturnType, method.GetReturnTypeAttributes(), ParameterType.Return, out suffix);
            if (suffix == null)
                return type;
            else
                return $"{type} {suffix}";
        }

        private static string getCLangType(ITypeSymbol symbol, IEnumerable<AttributeData> attributes, ParameterType type, out string? suffix)
        {
            switch(symbol.TypeKind)
            {
                // Handle some special types first
                case TypeKind.Enum:
                {
                    suffix = null;
                    var bindingAttr = symbol.GetAttribute<NativeBindingAttribute>();
                    string binded = bindingAttr.GetConstructorArgument<string>(0);
                    if (type == ParameterType.ByRef)
                        return $"{binded} *";
                    else
                        return binded;
                }
                case TypeKind.Delegate:
                {
                    suffix = null;
                    var bindingAttr = symbol.GetAttribute<NativeBindingAttribute>();
                    return bindingAttr.GetConstructorArgument<string>(0);
                }
            }

            string typeName;
            bool constParameter = false;
            if (symbol.TypeKind == TypeKind.Array)
            {
                var arrayType = (IArrayTypeSymbol)symbol;
                typeName = arrayType.ElementType.GetFullName();

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
            }
            else
            {
                suffix = null;
                typeName = symbol.GetFullName();
            }

            string bindedType;
            switch (type)
            {
                case ParameterType.Regular:
                    bindedType = getCLangType(typeName, attributes, false, ref constParameter);
                    break;
                case ParameterType.Return:
                    if (symbol.TypeKind == TypeKind.Array)
                        bindedType = getCLangPointerType(typeName, attributes);
                    else
                        bindedType = getCLangType(typeName, attributes, true, ref constParameter);

                    break;
                case ParameterType.ByRef:
                    bindedType = getCLangPointerType(typeName, attributes);
                    break;
                default:
                    throw new Exception();
            }

            if (constParameter)
                return $"const {bindedType}";
            else
                return bindedType;
        }

        static string getCLangType(string typeName, IEnumerable<AttributeData> attributes, bool returnType, ref bool constParameter)
        {
            switch (typeName)
            {
                case "System.Void":
                    return "void";
                case "System.String":
                    if (!returnType && !attributes.HasAttribute<MutableAttribute>())
                        constParameter |= true;

                    return "cbstring_t";
                case "System.Runtime.InteropServices.HandleRef":
                case "System.IntPtr":
                    var binder = attributes.FirstOrDefault((item) => item.Inherits<NativeTypeBinder>());
                    if (binder != null)
                        return $"{binder.AttributeClass.Name} *";

                    return "void *";
                case "System.Boolean":
                    // TODO: Check this has the attribute [MarshalAs(UnmanageType.I1)]
                    return "BBool";
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
                case "System.String":
                    return "cbstring_t *";
                case "System.Runtime.InteropServices.HandleRef":
                case "System.IntPtr":
                    var binder = attributes.FirstOrDefault((item) => item.Inherits<NativeTypeBinder>());
                    if (binder != null)
                        return $"{binder.AttributeClass.Name} **";

                    return "void **";
                case "System.Boolean":
                    // TODO: Check this has the attribute [MarshalAs(UnmanageType.I1)]
                    return "BBool *";
                case "System.Char":
                    return "cbchar_t *";
                case "System.Byte":
                    return "uint8_t *";
                case "System.SByte":
                    return "int8_t *";
                case "System.Int16":
                    return "int16_t *";
                case "System.UInt16":
                    return "uint16_t *";
                case "System.Int32":
                    // TODO: Add CodeBinder.Attributes attribute to specify explicitly sized 32 bit signed integer
                    return "int *";
                case "System.UInt32":
                    return "uint32_t *";
                case "System.Int64":
                    return "int64_t *";
                case "System.UInt64":
                    return "uint64_t *";
                case "System.Single":
                    return "float *";
                case "System.Double":
                    return "double *";
                default:
                    throw new Exception($"Unsupported by type {typeName}");
            }
        }

        public static CodeBuilder EndOfLine(this CodeBuilder builder)
        {
            return builder.AppendLine(";");
        }

        public static CodeBuilder Space(this CodeBuilder builder)
        {
            return builder.Append(" ");
        }

        public static CodeBuilder Comma(this CodeBuilder builder)
        {
            return builder.Append(",");
        }

        public static CodeBuilder CommaSeparator(this CodeBuilder builder)
        {
            return builder.Append(", ");
        }

        public static CodeBuilder TypeBlock(this CodeBuilder builder, bool appendLine = true)
        {
            builder.AppendLine("{");
            return builder.Indent("}", appendLine);
        }
    }
}
