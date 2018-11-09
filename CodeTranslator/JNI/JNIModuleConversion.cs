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
            get { return JNIModuelName + ".h"; }
        }

        public string JNIModuelName
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
                WriteMethod(builder, method);
        }

        private void WriteMethod(CodeBuilder builder, MethodDeclarationSyntax method)
        {
            var methodSignatures = method.GetMethodSignatures(this);

            builder.Append("JNIEXPORT ");
            WriteType(builder, method.ReturnType);
            builder.Append("JNICALL ");
            builder.Append("Java_").Append(Namespace.Replace('.', '_')).Append("_")
                .Append(JNIModuelName)
                .Append("_").Append(method.GetName()).AppendLine("(");
            builder.IncreaseIndent();
            WriteParameters(builder, method.ParameterList);
            builder.AppendLine(");");
            builder.DecreaseIndent();
            builder.AppendLine();
        }

        private void WriteParameters(CodeBuilder builder, ParameterListSyntax parameterList)
        {
            builder.Append("JNIEnv *, jclass");
            foreach (var parameter in parameterList.Parameters)
                WriteParameter(builder, parameter);
        }

        private void WriteParameter(CodeBuilder builder, ParameterSyntax parameter)
        {
            builder.Append(", ");
            bool isRef = parameter.IsRef() || parameter.IsOut();
            WriteType(builder, parameter.Type, isRef);
            builder.Append(parameter.Identifier.Text);
        }

        private void WriteType(CodeBuilder builder, TypeSyntax type, bool isRef = false)
        {
            builder.Append(type.GetJNIType(isRef, this)).Append(" ");
        }

        public override string GeneratedPreamble
        {
            get { return "/* This file was generated. DO NOT EDIT! */"; }
        }
    }
}
