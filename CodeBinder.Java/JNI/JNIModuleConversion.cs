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
    public class JNIModuleConversion : TypeConversion<JNIModuleContext, JNICompilationContext, ConversionCSharpToJNI>
    {
        public JNIModuleConversion(JNIModuleContext module, ConversionCSharpToJNI conversion)
            : base(module, conversion)
        {
        }

        public string JNIModuleName => $"JNI{Context.Name}";

        protected override string GetFileName() => $"{JNIModuleName}.h";

        protected override string GetGeneratedPreamble() => ConversionCSharpToJNI.SourcePreamble;

        protected sealed override void write(CodeBuilder builder)
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
            foreach (var method in Context.Methods)
            {
                builder.Append(MethodWriter.Create(method, this));
                builder.AppendLine();
            }
        }

        public override JNICompilationContext Compilation
        {
            get { return Context.Compilation; }
        }
    }
}
