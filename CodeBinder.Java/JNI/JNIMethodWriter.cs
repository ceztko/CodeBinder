// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeBinder.CLang;
using System;
using System.Runtime.InteropServices;

namespace CodeBinder.JNI
{
    class JNITrampolineMethodWriter : CodeWriter<MethodDeclarationSyntax, JNIModuleConversion>
    {
        public ConversionType ConversionType { get; private set; }

        public JNITrampolineMethodWriter(MethodDeclarationSyntax method, JNIModuleConversion module, ConversionType conversionType)
            : base(method, module)
        {
            ConversionType = conversionType;
        }

        protected override void Write()
        {
            if (ConversionType == ConversionType.Implementation)
                Builder.Append("extern \"C\"").Space();

            Builder.Append("JNIEXPORT").Space();
            Builder.Append(ReturnType).Space();
            Builder.Append("JNICALL").Space();
            Builder.Append(MethodName).AppendLine("(");
            using (Builder.Indent())
            {
                Builder.Append("JNIEnv *jenv, jclass jcls");
                WriteParameters();
                Builder.Append(")");
            }

            if (ConversionType == ConversionType.Implementation)
            {
                using (Builder.AppendLine().Block())
                {
                    WriteBody();
                }
            }
            else
            {
                Builder.EndOfLine();
            }
        }

        void WriteBody()
        {
            Builder.AppendLine("(void)jenv;");
            Builder.AppendLine("(void)jcls;");

            bool closeBuilder = false;
            var returnTypeSym = Item.ReturnType.GetTypeSymbol(Context);
            if (returnTypeSym.SpecialType != SpecialType.System_Void)
            {
                Builder.Append("return").Space();
                switch (returnTypeSym.SpecialType)
                {
                    case SpecialType.System_Boolean:
                    case SpecialType.System_IntPtr:
                    case SpecialType.System_UIntPtr:
                    {
                        // These types requires cast
                        Builder.Append($"({Item.GetJNIReturnType(Context)})");
                        break;
                    }
                    default:
                    {
                        if (returnTypeSym.TypeKind == TypeKind.Enum)
                        {
                            // Enum types requires cast to jint
                            Builder.Append($"({Item.GetJNIReturnType(Context)})");
                        }
                        else if (returnTypeSym.GetFullName() == "CodeBinder.cbstring")
                        {
                            // e.g. return SN2J(ENTextFieldGetText(field), jenv);
                            Builder.Append("SN2J").Parenthesized(false).Append("jenv").CommaSeparator();
                            closeBuilder = true;
                        }

                        break;
                    }
                }
            }

            Builder.Append(Item.GetCLangMethodName());
            using (Builder.ParameterList())
            {
                bool first = true;
                foreach (var param in Item.ParameterList.Parameters)
                {
                    Builder.CommaSeparator(ref first);
                    var symbol = param.GetDeclaredSymbol<IParameterSymbol>(Context);
                    switch (symbol.Type.TypeKind)
                    {
                        case TypeKind.Array:
                        {
                            bool commit = symbol.HasAttribute<OutAttribute>();
                            Builder.Append("AJ2N").Parenthesized().Append("jenv")
                                .CommaSeparator().Append(param.Identifier.Text)
                                .CommaSeparator().Append(commit ? "true" : "false")
                                .Close();
                            break;
                        }
                        case TypeKind.Enum:
                        {
                            string? binder;
                            if (!param.TryGetCLangBinder(Context, out binder))
                                throw new NotSupportedException($"Missing binder for enum {param.Identifier.Text}");

                            if (symbol.IsRefLike())
                            {
                                Builder.Append("BJ2N")
                                    .AngleBracketed().Append(binder).Close()
                                    .Parenthesized().Append("jenv").CommaSeparator().Append(param.Identifier.Text).Close();
                            }
                            else
                            {
                                Builder.Parenthesized().Append(binder).Close().Append(param.Identifier.Text);
                            }

                            break;
                        }
                        default:
                        {
                            var fullTypeName = symbol.Type.GetFullName();
                            switch (fullTypeName)
                            {
                                case "System.IntPtr":
                                case "System.UIntPtr":
                                {
                                    if (symbol.IsRefLike())
                                    {
                                        WriteBoxParameter(param, symbol);
                                    }
                                    else
                                    {
                                        string? binder;
                                        if (param.TryGetCLangBinder(true, Context, out binder))
                                            Builder.Parenthesized().Append(binder).Close().Append(param.Identifier.Text);
                                        else
                                            Builder.Parenthesized().Append("void *").Close().Append(param.Identifier.Text);
                                    }

                                    break;
                                }
                                case "System.Runtime.InteropServices.HandleRef":
                                {
                                    string? binder;
                                    if (param.TryGetCLangBinder(Context, out binder))
                                    {
                                        // e.g. field->GetHandle<ENPdfTextField>(jenv)
                                        Builder.Append(param.Identifier.Text).Append("->GetHandle")
                                            .AngleBracketed().Append(binder).Close().
                                            Parenthesized().Append("jenv").Close();
                                    }
                                    else
                                    {
                                        throw new NotImplementedException();
                                    }

                                    break;
                                }
                                case "CodeBinder.cbstring":
                                {
                                    if (symbol.IsRefLike())
                                    {
                                        // e.g. BJ2N(jenv, name)
                                        Builder.Append("BJ2N")
                                            .Parenthesized().Append("jenv").CommaSeparator().Append(param.Identifier.Text).Close();
                                    }
                                    else
                                    {
                                        // e.g. SJ2N(jenv, value)
                                        Builder.Append("SJ2N").Parenthesized().Append("jenv").CommaSeparator().Append(param.Identifier.Text).Close();
                                    }

                                    break;
                                }
                                case "System.Boolean":
                                {
                                    if (symbol.IsRefLike())
                                        WriteBoxParameter(param, symbol);
                                    else
                                        Builder.Append("(cbbool)").Append(param.Identifier.Text);
                                    break;
                                }
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
                                {
                                    if (symbol.IsRefLike())
                                        WriteBoxParameter(param, symbol);
                                    else
                                        Builder.Append(param.Identifier.Text);
                                    break;
                                }
                                default:
                                {
                                    Builder.Append(param.Identifier.Text);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            if (closeBuilder)
                Builder.CloseUsing();

            Builder.EndOfLine();
        }

        private void WriteBoxParameter(ParameterSyntax param, IParameterSymbol symbol)
        {
            // e.g. BJ2N<uint32_t>(jenv, objNum)
            Builder.Append("BJ2N")
                .AngleBracketed().Append(symbol.Type.SpecialType.GetCLangType()).Close()
                .Parenthesized().Append("jenv").CommaSeparator().Append(param.Identifier.Text).Close();
        }

        void WriteTrampolineCall()
        {

        }

        void WriteParameters()
        {
            foreach (var parameter in Item.ParameterList.Parameters)
                WriteParameter(parameter);
        }

        void WriteParameter(ParameterSyntax parameter)
        {
            Builder.CommaSeparator();
            Builder.Append(parameter.GetJNIType(Context)).Space();
            Builder.Append(parameter.Identifier.Text);
        }

        public string ReturnType
        {
            get { return Item.GetJNIReturnType(Context); }
        }

        public string MethodName
        {
            get { return Item.GetJNIMethodName(Context.Context); }
        }
    }
}
