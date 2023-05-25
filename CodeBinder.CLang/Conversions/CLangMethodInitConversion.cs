// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared.CSharp;
using CodeBinder.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    class CLangMethodInitConversion : CLangConversionWriter
    {
        public CLangMethodInitConversion(CLangCompilationContext compilation)
            : base(compilation) { }

        protected override void write(CodeBuilder builder)
        {
            foreach (var module in Compilation.Modules)
                builder.Append("#include \"").Append(module.Name).AppendLine(".h\"");

            builder.AppendLine();
            builder.AppendLine("static void* funcs[] = {");
            using (builder.Indent())
            {
                foreach (var module in Compilation.Modules)
                {
                    foreach (var method in module.Methods)
                        builder.Append("(void *)").Append(method.GetCLangMethodName()).AppendLine(",");
                }
            }

            builder.Append("}").EndOfLine();
        }

        protected override string GetGeneratedPreamble() => ConversionCSharpToCLang.SourcePreamble;

        protected override string GetFileName() => "MethodInit.cpp";
    }
}
