// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using System.Linq;
using System.Runtime.InteropServices;

namespace CodeBinder.NativeAOT;

public static class NativeAOTMethodExtensions
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

    public static CodeBuilder Append(this CodeBuilder builder, ParameterSyntax parameter, ICompilationProvider provider)
    {
        bool isByRef = parameter.IsRef() || parameter.IsOut();
        var symbol = parameter.Type!.GetTypeSymbolThrow(provider);
        string? suffix;
        string type = getCLangType(symbol, parameter.GetAttributes(provider),
            isByRef ? DeclarationType.ParamByRef : DeclarationType.Regular, out suffix);
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
        ICompilationProvider provider, [NotNullWhen(true)] out string? binderStr)
    {
        return TryGetCLangBinder(typeSyntax, false, provider, out binderStr);
    }

    public static bool TryGetCLangBinder(this BaseTypeDeclarationSyntax typeSyntax, bool pointerType,
        ICompilationProvider provider, [NotNullWhen(true)] out string? binderStr)
    {
        var symbol = typeSyntax.GetTypeSymbol(provider);
        return tryGetCLangBinder(symbol, pointerType, out binderStr);
    }

    public static bool TryGetCLangBinder(this ParameterSyntax parameter,
        ICompilationProvider provider, [NotNullWhen(true)] out string? binderStr)
    {
        return TryGetCLangBinder(parameter, false, provider, out binderStr);
    }

    public static bool TryGetCLangBinder(this ParameterSyntax parameter, bool pointerType,
        ICompilationProvider provider, [NotNullWhen(true)] out string? binderStr)
    {
        var symbol = parameter.Type!.GetTypeSymbolThrow(provider);
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

    public static string GetCLangDeclaration(this VariableDeclarationSyntax declaration, ICompilationProvider provider)
    {
        var symbol = declaration.Type.GetTypeSymbolThrow(provider);
        var type = getCLangType(symbol, declaration.Variables[0].GetAttributes(provider),
            DeclarationType.Regular, out _);
        return $"{type} {declaration.Variables[0].Identifier.Text}";
    }

    public static string GetCLangDeclaration(this ParameterSyntax parameter, ICompilationProvider provider)
    {
        var symbol = parameter.Type!.GetTypeSymbolThrow(provider);
        string? suffix;
        var type = getCLangType(symbol, parameter.GetAttributes(provider), DeclarationType.Regular, out suffix);
        if (suffix == null)
            return $"{type} {parameter.Identifier.Text}";
        else
            return $"{type} {parameter.Identifier.Text}{suffix}";
    }

    public static string GetCLangMethodName(this MethodDeclarationSyntax method)
    {
        return method.GetName();
    }

    internal static string GetCLangReturnType(this MethodDeclarationSyntax method,
        ICompilationProvider provider)
    {
        var symbol = method.GetDeclaredSymbol<IMethodSymbol>(provider);
        return GetCLangReturnType(symbol);
    }

    public static string GetCLangReturnType(this DelegateDeclarationSyntax dlg, ICompilationProvider provider)
    {
        // TODO: Should be possible to prpare static cpp trampolines also for delegates.
        // Maybe not so easy
        var symbol = dlg.GetDeclaredSymbol<INamedTypeSymbol>(provider);
        return GetCLangReturnType(symbol.DelegateInvokeMethod!);
    }

    public static string GetCLangReturnType(this IMethodSymbol method)
    {
        string? suffix;
        string type = getCLangType(method.ReturnType, method.GetReturnTypeAttributes(),
            DeclarationType.Return, out suffix);
        if (suffix == null)
            return type;
        else
            return $"{type} {suffix}";
    }

    private static string getCLangType(ITypeSymbol symbol, IEnumerable<AttributeData> attributes,
        DeclarationType declType, out string? suffix)
    {
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
                        ;
                }
                else
                {
                    if (!attributes.HasAttribute<OutAttribute>())
                        ;

                    int fixedSizeArray;
                    if (attributes.TryGetAttribute<MarshalAsAttribute>(out var marshalAsAttr) &&
                        marshalAsAttr.TryGetNamedArgument("SizeConst", out fixedSizeArray))
                    {
                        suffix = $"[{fixedSizeArray}]";
                    }
                    else
                    {
                        suffix = null;
                    }
                }

                var arrayType = (IArrayTypeSymbol)symbol;
                if (!tryGetCLangBinder(arrayType.ElementType, out bindedType))
                {
                    string typeName = arrayType.ElementType.GetFullName();
                    switch (declType)
                    {
                        case DeclarationType.Regular:
                            bindedType = getCLangPointerType(typeName, attributes);
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
                            bindedType = "cbstring";
                            break;
                        }
                        case DeclarationType.Return:
                        {
                            bindedType = "cbstring";
                            break;
                        }
                        case DeclarationType.ParamByRef:
                        {
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
            case "CodeBinder.cbbool":
                return "cbbool";
            case "System.Byte":
                return "byte";
            case "System.SByte":
                return "sbyte";
            case "System.Int16":
                return "short";
            case "System.UInt16":
                return "ushort";
            case "System.Int32":
                return "int";
            case "System.UInt32":
                return "uint";
            case "System.Int64":
                return "long";
            case "System.UInt64":
                return "ulong";
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
            case "CodeBinder.cbbool":
                return "cbbool*";
            case "System.Byte":
                return "byte*";
            case "System.SByte":
                return "sbyte*";
            case "System.Int16":
                return "short*";
            case "System.UInt16":
                return "ushort*";
            case "System.Int32":
                return "int*";
            case "System.UInt32":
                return "uint*";
            case "System.Int64":
                return "long*";
            case "System.UInt64":
                return "ulong*";
            case "System.Single":
                return "float*";
            case "System.Double":
                return "double*";
            default:
                throw new Exception($"Unsupported by type {typeName}");
        }
    }
}
