using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    class CLangMethodInitWriter : ConversionBuilder
    {
        CLangCompilationContext _compilation;

        public CLangMethodInitWriter(CLangCompilationContext compilation)
        {
            _compilation = compilation;
        }

        public override void Write(CodeBuilder builder)
        {
            foreach (var module in _compilation.Modules)
                builder.Append("#include \"C").Append(module.Name).AppendLine(".h\"");

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

        public override string GeneratedPreamble
        {
            get { return ConversionCSharpToCLang.SourcePreamble; }
        }

        public override string FileName
        {
            get { return "MethodInit.cpp"; }
        }
    }
}
