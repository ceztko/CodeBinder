using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    class CLangMethodInitBuilder : CLangCompilationContextBuilder
    {
        public CLangMethodInitBuilder(CLangCompilationContext compilation)
            : base(compilation) { }

        protected override void write(CodeBuilder builder)
        {
            foreach (var module in Compilation.Modules)
                builder.Append("#include \"").Append(module.Name).AppendLine(".h\"");

            builder.AppendLine();
            builder.AppendLine("static void* funcs[] = {");
            using (builder.Indent())
            {
                void writeMethods(bool widechar)
                {
                    foreach (var module in Compilation.Modules)
                    {
                        foreach (var method in module.Methods)
                            builder.Append("(void *)").Append(method.GetCLangMethodName(widechar, module)).AppendLine(",");
                    }
                }

                //writeMethods(true);
                writeMethods(false);
            }

            builder.Append("}").EndOfLine();
        }

        protected override string GetGeneratedPreamble() => ConversionCSharpToCLang.SourcePreamble;

        protected override string GetFileName() => "MethodInit.cpp";
    }
}
