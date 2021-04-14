// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    class JNIMethodInitConversion : ConversionWriter
    {
        JNICompilationContext _compilation;

        public JNIMethodInitConversion(JNICompilationContext compilation)
        {
            _compilation = compilation;
        }

        protected override void write(CodeBuilder builder)
        {
            foreach (var module in _compilation.Modules)
                builder.Append("#include \"JNI").Append(module.Name).AppendLine(".h\"");

            builder.AppendLine();
            builder.AppendLine("static void* funcs[] = {");
            using (builder.Indent())
            {
                foreach (var module in _compilation.Modules)
                {
                    foreach (var method in module.Methods)
                        builder.Append("(void *)").Append(method.GetJNIMethodName(module)).AppendLine(",");
                }
            }

            builder.Append("}").EndOfLine();
        }

        protected override string GetGeneratedPreamble() => ConversionCSharpToJNI.SourcePreamble;

        protected override string GetFileName() => "MethodInit.cpp";
    }
}
