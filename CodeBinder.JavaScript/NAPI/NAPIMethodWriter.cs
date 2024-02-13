// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.NAPI;

class NAPITrampolineMethodWriter : CodeWriter<MethodDeclarationSyntax, NAPIModuleConversion>
{
    public ConversionType ConversionType { get; private set; }

    public NAPITrampolineMethodWriter(MethodDeclarationSyntax method, NAPIModuleConversion module, ConversionType conversionType)
        : base(method, module)
    {
        ConversionType = conversionType;
    }

    protected override void Write()
    {
        Builder.Append("extern \"C\"").Space().Append("napi_value").Space();
        Builder.Append(MethodName).AppendLine("(");
        using (Builder.Indent())
            Builder.Append("napi_env env, napi_callback_info info").Append(")");

        if (ConversionType == ConversionType.Implementation)
        {
            using (Builder.AppendLine().Block())
            {
                writeBody();
            }
        }
        else
        {
            Builder.EndOfStatement();
        }
    }

    void writeBody()
    {
        Builder.Append("napi_status napistatus_").EndOfStatement();
        Builder.AppendLine();
        Builder.Append("(void)env").EndOfStatement();
        Builder.Append("(void)info").EndOfStatement();
        Builder.Append("(void)napistatus_").EndOfStatement();
        Builder.AppendLine();

        bindParameters();

        var methodSymbol = Item.GetDeclaredSymbol<IMethodSymbol>(Context);
        if (!methodSymbol.ReturnsVoid)
        {
            Builder.Append(methodSymbol.GetCLangReturnType()).Space().Append("cret_").Space().Append("=").Space();
        }

        Builder.Append(Item.GetCLangMethodName());
        using (Builder.ParameterList())
        {
            bool first = true;
            int i = 0;
            foreach (var param in Item.ParameterList.Parameters)
            {
                Builder.CommaSeparator(ref first);
                var symbol = param.GetDeclaredSymbol<IParameterSymbol>(Context);
                switch (symbol.Type.TypeKind)
                {
                    case TypeKind.Array:
                    {
                        var arrayType = (IArrayTypeSymbol)symbol.Type;
                        bool commit = symbol.HasAttribute<OutAttribute>();
                        Builder.Append("AJS2N").AngleBracketed().Append($"{(commit ? "" : "const ")}{arrayType.ElementType.GetCLangType()}").Close()
                            .Parenthesized().Append("env")
                            .CommaSeparator().Append(param.Identifier.Text)
                            .CommaSeparator().Append(commit ? "true" : "false")
                            .Close();
                        break;
                    }
                    case TypeKind.Enum:
                    {
                        if (symbol.IsRefLike())
                        {
                            string? binder;
                            if (!param.TryGetCLangBinder(Context, out binder))
                                throw new Exception("Unable to find binder");

                            // e.g. BJS2N<ENPdfVersion, int32_t>(env, box)
                            Builder.Append($"BJS2N<{binder}, int32_t>").Parenthesized()
                                .Append("env").CommaSeparator().Append(param.Identifier.Text).Close();
                        }
                        else
                        {
                            Builder.Append(param.Identifier.Text);
                        }
                        break;
                    }
                    default:
                    {
                        var fullTypeName = symbol.Type.GetFullName();
                        if (symbol.IsRefLike())
                        {
                            switch (fullTypeName)
                            {
                                case "CodeBinder.cbstring":
                                {
                                    // e.g. BJS2N<cbstring>(env, box)
                                    Builder.Append("BJS2N<cbstring>").Parenthesized().
                                        Append("env").CommaSeparator().Append(param.Identifier.Text).Close();
                                    break;
                                } 
                                case "System.IntPtr":
                                case "System.UIntPtr":
                                {
                                    string? binder;
                                    if (!param.TryGetCLangBinder(Context, out binder))
                                        throw new Exception("Unable to find binder");

                                    // e.g. BJS2N<ENPdfXObject*, int64_t>(env, box)
                                    Builder.Append($"BJS2N<{binder}*, int64_t>").Parenthesized()
                                        .Append("env").CommaSeparator().Append(param.Identifier.Text).Close();
                                    break;
                                }
                                case "CodeBinder.cbbool":
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
                                {
                                    writeBoxParameter(param, symbol);
                                    break;
                                }
                                default:
                                {
                                    throw new NotSupportedException();
                                }
                            }
                        }
                        else
                        {
                            Builder.Append(param.Identifier.Text);
                        }

                        break;
                    }
                }

                i++;
            }
        }

        Builder.EndOfStatement();
        Builder.AppendLine();

        if (methodSymbol.ReturnsVoid)
        {
            // NOTE: void returning function needs nullptr
            Builder.Append("return nullptr").EndOfStatement();
        }
        else
        {
            Builder.Append("return").Space();
            switch (methodSymbol.ReturnType.TypeKind)
            {
                case TypeKind.Enum:
                {
                    // Enum types requires cast to jint
                    Builder.Append("CreateNapiValue(env, (int32_t)cret_)").EndOfStatement();
                    break;
                }
                default:
                {
                    switch (methodSymbol.ReturnType.GetFullName())
                    {
                        case "System.IntPtr":
                        case "System.UIntPtr":
                        case "System.Boolean":
                        case "System.Byte":
                        case "System.SByte":
                        case "System.UInt16":
                        case "System.Int16":
                        case "System.UInt32":
                        case "System.Int32":
                        case "System.UInt64":
                        case "System.Int64":
                        case "System.Single":
                        case "System.Double":
                        case "CodeBinder.cbstring":
                        case "CodeBinder.cbbool":
                        {
                            Builder.Append("CreateNapiValue(env, cret_)").EndOfStatement();
                            break;
                        }
                        default:
                        {
                            throw new NotSupportedException();
                        }
                    }
                    break;
                }
            }
        }
    }

    void bindParameters()
    {
        int parameterCount = Item.ParameterList.Parameters.Count;
        if (parameterCount == 0)
            return;

        Builder.AppendLine($"size_t argc = {parameterCount};");
        Builder.AppendLine($"napi_value args[{parameterCount}];");
        Builder.AppendLine("napi_get_cb_info(env, info, &argc, args, nullptr, nullptr);");
        Builder.AppendLine($"assert(argc == {parameterCount});");

        Builder.AppendLine();
        for (int i = 0; i < Item.ParameterList.Parameters.Count; i++)
            bindParameter(Item.ParameterList.Parameters[i], i);
    }

    void writeBoxParameter(ParameterSyntax param, IParameterSymbol symbol)
    {
        // e.g. BJS2N<uint32_t>(env, box)
        Builder.Append("BJS2N")
            .AngleBracketed().Append(symbol.Type.GetCLangType()).Close()
            .Parenthesized().Append("env").CommaSeparator().Append(param.Identifier.Text).Close();
    }

    void bindParameter(ParameterSyntax param, int index)
    {
        Builder.Append("auto").Space().Append(param.Identifier.Text).Space().Append("=").Space();
        var symbol = param.GetDeclaredSymbol<IParameterSymbol>(Context);
        switch (symbol.Type.TypeKind)
        {
            case TypeKind.Array:
            {
                if (symbol.IsRefLike())
                    throw new NotSupportedException("ref like array parameter is unsupported");

                Builder.Append($"args[{index}]");
                break;
            }
            case TypeKind.Enum:
            {
                if (symbol.IsRefLike())
                {
                    Builder.Append($"args[{index}]");
                }
                else
                {
                    string? binder;
                    if (!param.TryGetCLangBinder(Context, out binder))
                        throw new Exception("Unable to find binder");

                    Builder.Parenthesized().Append(binder).Close().Append($"GetInt32FromNapiValue(env, args[{index}])");
                }

                break;
            }
            default:
            {
                var fullTypeName = symbol.Type.GetFullName();

                if (symbol.IsRefLike())
                {
                    Builder.Append($"args[{index}]");
                }
                else
                {
                    switch (fullTypeName)
                    {
                        case "System.IntPtr":
                        case "System.UIntPtr":
                        {
                            string? binder;
                            if (param.TryGetCLangBinder(true, Context, out binder))
                                Builder.Parenthesized().Append(binder).Close().Append($"GetPtrFromNapiValue(env, args[{index}])");
                            else
                                Builder.Append($"GetPtrFromNapiValue(env, args[{index}])");

                            break;
                        }
                        case "System.Runtime.InteropServices.HandleRef":
                        {
                            string? binder;
                            if (!param.TryGetCLangBinder(true, Context, out binder))
                                throw new NotImplementedException();

                            Builder.Parenthesized().Append(binder).Close()
                                .Append($"GetHandleRefPtrFromNapiValue(env, args[{index}])");

                            break;
                        }

                        case "CodeBinder.cbstring":
                        {
                            Builder.Append($"CreateCBStringFromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "CodeBinder.cbbool":
                        {
                            Builder.Append($"GetBoolFromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "CodeBinder.cboptbool":
                        {
                            Builder.Append($"GetOptBoolFromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.SByte":
                        {
                            Builder.Append($"GetInt8FromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.Byte":
                        {
                            Builder.Append($"GetUInt8FromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.Int16":
                        {
                            Builder.Append($"GetInt16FromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.UInt16":
                        {
                            Builder.Append($"GetUInt16FromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.Int32":
                        {
                            Builder.Append($"GetInt32FromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.UInt32":
                        {
                            Builder.Append($"GetUInt32FromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.Int64":
                        {
                            Builder.Append($"GetInt64FromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.UInt64":
                        {
                            Builder.Append($"GetUInt64FromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.Single":
                        {
                            Builder.Append($"GetFloatFromNapiValue(env, args[{index}])");
                            break;
                        }
                        case "System.Double":
                        {
                            Builder.Append($"GetDoubleFromNapiValue(env, args[{index}])");
                            break;
                        }
                        default:
                            throw new NotSupportedException($"Unsupported type {fullTypeName}");
                    }
                }

                break;
            }
        }

        Builder.EndOfStatement();
    }

    public string MethodName
    {
        get { return Item.GetNAPIMethodName(); }
    }
}
