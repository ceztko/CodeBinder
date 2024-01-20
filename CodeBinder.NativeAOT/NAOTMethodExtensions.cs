// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using System.Linq;
using System.Runtime.InteropServices;

namespace CodeBinder.NativeAOT;

public static class NAOTMethodExtensions
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
        string type = getNAOTType(symbol, parameter.GetAttributes(provider),
            isByRef ? DeclarationType.ParamByRef : DeclarationType.Regular);
        builder.Append(type).Space().Append(parameter.Identifier.Text);
        return builder;
    }

    static bool tryGetNAOTBinder(this ITypeSymbol typeSymbol, bool pointerType,
        [NotNullWhen(true)] out string? binderStr)
    {
        if (!tryGetNAOTBinder(typeSymbol, out binderStr))
        {
            binderStr = null;
            return false;
        }

        if (pointerType)
            binderStr = $"{binderStr}*";

        return true;
    }

    public static bool TryGetNAOTBinder(this BaseTypeDeclarationSyntax typeSyntax,
        ICompilationProvider provider, [NotNullWhen(true)] out string? binderStr)
    {
        return TryGetNAOTBinder(typeSyntax, false, provider, out binderStr);
    }

    public static bool TryGetNAOTBinder(this BaseTypeDeclarationSyntax typeSyntax, bool pointerType,
        ICompilationProvider provider, [NotNullWhen(true)] out string? binderStr)
    {
        var symbol = typeSyntax.GetTypeSymbol(provider);
        return tryGetNAOTBinder(symbol, pointerType, out binderStr);
    }

    public static bool TryGetNAOTBinder(this ParameterSyntax parameter,
        ICompilationProvider provider, [NotNullWhen(true)] out string? binderStr)
    {
        return TryGetNAOTBinder(parameter, false, provider, out binderStr);
    }

    public static bool TryGetNAOTBinder(this ParameterSyntax parameter, bool pointerType,
        ICompilationProvider provider, [NotNullWhen(true)] out string? binderStr)
    {
        var symbol = parameter.Type!.GetTypeSymbolThrow(provider);
        if (tryGetNAOTBinder(symbol, pointerType, out binderStr))
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

    static bool tryGetNAOTBinder(ITypeSymbol symbol, [NotNullWhen(true)] out string? binderStr)
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

    public static string GetNAOTDeclaration(this VariableDeclarationSyntax declaration, ICompilationProvider provider)
    {
        var symbol = declaration.Type.GetTypeSymbolThrow(provider);
        var type = getNAOTType(symbol, declaration.Variables[0].GetAttributes(provider),
            DeclarationType.Regular);
        return $"{type} {declaration.Variables[0].Identifier.Text}";
    }

    public static string GetNAOTDeclaration(this ParameterSyntax parameter, ICompilationProvider provider)
    {
        var symbol = parameter.Type!.GetTypeSymbolThrow(provider);
        var type = getNAOTType(symbol, parameter.GetAttributes(provider), DeclarationType.Regular);
        return $"{type} {parameter.Identifier.Text}";
    }

    public static string GetNAOTType(this ParameterSyntax parameter, ICompilationProvider provider)
    {
        var symbol = parameter.Type!.GetTypeSymbolThrow(provider);
        return getNAOTType(symbol, parameter.GetAttributes(provider), DeclarationType.Regular);
    }

    public static string GetNAOTMethodName(this MethodDeclarationSyntax method)
    {
        return method.GetName();
    }

    internal static string GetNAOTReturnType(this MethodDeclarationSyntax method,
        ICompilationProvider provider)
    {
        var symbol = method.GetDeclaredSymbol<IMethodSymbol>(provider);
        return getNAOTReturnType(symbol);
    }

    public static string GetNAOTReturnType(this DelegateDeclarationSyntax dlg, ICompilationProvider provider)
    {
        // TODO: Should be possible to prpare static cpp trampolines also for delegates.
        // Maybe not so easy
        var symbol = dlg.GetDeclaredSymbol<INamedTypeSymbol>(provider);
        return getNAOTReturnType(symbol.DelegateInvokeMethod!);
    }

    static string getNAOTReturnType(IMethodSymbol method)
    {
        string type = getNAOTType(method.ReturnType, method.GetReturnTypeAttributes(),
            DeclarationType.Return);
        return type;
    }

    private static string getNAOTType(ITypeSymbol symbol, IEnumerable<AttributeData> attributes,
        DeclarationType declType)
    {
        string? bindedType;
        switch (symbol.TypeKind)
        {
            // Handle some special types first
            case TypeKind.Enum:
            {
                string? binded;
                if (!tryGetNAOTBinder(symbol, out binded))
                    throw new Exception($"Could not find the binder for the type {symbol}");

                if (declType == DeclarationType.ParamByRef)
                    return $"{binded} *";
                else
                    return binded;
            }
            case TypeKind.Delegate:
            {
                string? binded;
                if (!tryGetNAOTBinder(symbol, out binded))
                    throw new Exception($"Could not find the binder for the type {symbol}");

                return binded;
            }
            case TypeKind.Array:
            {
                if (declType == DeclarationType.Return)
                {
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
                    }
                }

                var arrayType = (IArrayTypeSymbol)symbol;
                if (!tryGetNAOTBinder(arrayType.ElementType, out bindedType))
                {
                    string typeName = arrayType.ElementType.GetFullName();
                    switch (declType)
                    {
                        case DeclarationType.Regular:
                            bindedType = getNAOTPointerType(typeName, attributes);
                            break;
                        case DeclarationType.Return:
                            bindedType = getNAOTPointerType(typeName, attributes);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                break;
            }
            default:
            {
                string typeName = symbol.GetFullName();

                // TODO: Optmize CLR types with ITypeSymbol.SpecialType, handling of constParameter,
                // CHECK-ME evaluate supporting string arrays
                if (typeName == "CodeBinder.cbstring")
                {
                    switch (declType)
                    {
                        case DeclarationType.Regular:
                        {
                            bindedType = "CodeBinder.cbstring";
                            break;
                        }
                        case DeclarationType.Return:
                        {
                            bindedType = "CodeBinder.cbstring";
                            break;
                        }
                        case DeclarationType.ParamByRef:
                        {
                            bindedType = "CodeBinder.cbstring*";
                            break;
                        }
                        default:
                            throw new NotSupportedException();
                    }
                }
                else
                {
                    if (tryGetNAOTBinder(symbol, out bindedType))
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
                                bindedType = getNAOTType(typeName, attributes);
                                break;
                            case DeclarationType.ParamByRef:
                                bindedType = getNAOTPointerType(typeName, attributes);
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

    static string getNAOTType(string typeName, IEnumerable<AttributeData> attributes)
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

    static string getNAOTPointerType(string typeName, IEnumerable<AttributeData> attributes)
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
            case "CodeBinder.cbstring":
                return "cbstring*";
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
