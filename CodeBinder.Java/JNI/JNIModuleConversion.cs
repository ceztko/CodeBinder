// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBinder.JNI
{
    public class JNIModuleConversion : TypeConversion<JNIModuleContext, ConversionCSharpToJNI>
    {
        public JNIModuleConversion(ConversionCSharpToJNI conversion)
            : base(conversion)
        {
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
            builder.AppendLine("#include \"Internal/JNITypes.h\"");
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
                builder.Append(MethodWriter.Create(method, this));
                builder.AppendLine();
            }
        }

        public override string GeneratedPreamble
        {
            get { return ConversionCSharpToJNI.SourcePreamble; }
        }
    }
}
