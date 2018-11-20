// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTranslator.JNI
{
    public class JNIModuleConversion : TypeConversion<JNIModuleContext, CSToJNIConversion>
    {
        public string Namespace { get; private set; }
        string _Basepath;

        public JNIModuleConversion(CSToJNIConversion conversion)
            : base(conversion)
        {
            Namespace = conversion.BaseNamespace;
            _Basepath = string.IsNullOrEmpty(Namespace) ? null : Namespace.Replace('.', Path.DirectorySeparatorChar);
        }

        public override string FileName
        {
            get { return JNIModuleName + ".h"; }
        }

        public string JNIModuleName
        {
            get { return "JNI" + TypeContext.Name; }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            builder.AppendLine("#include \"JNITypes.h\"");
            builder.AppendLine();
            builder.AppendLine("#ifdef __cplusplus");
            builder.AppendLine("extern \"C\"");
            builder.AppendLine("{");
            builder.AppendLine("#endif");
            builder.AppendLine();
            WriteMethods(builder);
            builder.AppendLine("#ifdef __cplusplus");
            builder.AppendLine("}");
            builder.AppendLine("#endif");
        }

        private void WriteMethods(CodeBuilder builder)
        {
            foreach (var method in TypeContext.Methods)
            {
                var signatures = method.GetMethodSignatures(this);
                if (signatures.Count == 0)
                {
                    builder.Append(MethodWriter.Create(method, this));
                    builder.AppendLine();
                }
                else
                {
                    foreach (var signature in signatures)
                    {
                        builder.Append(MethodWriter.Create(signature, method, this));
                        builder.AppendLine();
                    }
                }
            }
        }

        public override string GeneratedPreamble
        {
            get { return CSToJNIConversion.SourcePreamble; }
        }
    }
}
