using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    class MethodInitWriter : ConversionBuilder
    {
        JNICompilationContext _compilation;

        public MethodInitWriter(JNICompilationContext compilation)
        {
            _compilation = compilation;
        }

        public override void Write(CodeBuilder builder)
        {
            builder.AppendLine("#include \"JNITypesPrivate.h\"");
            foreach (var module in _compilation.Modules)
                builder.Append("#include \"JNI").Append(module.Name).AppendLine(".h\"");

            builder.AppendLine();
            builder.AppendLine("static void* funcs[] = {");
            using (builder.Indent())
            {
                foreach (var module in _compilation.Modules)
                {
                    foreach (var method in module.Methods)
                    {
                        var signatures = method.GetMethodSignatures(module);
                        if (signatures.Count == 0)
                        {
                            builder.Append("(void *)").Append(method.GetJNIMethodName(module)).AppendLine(",");
                        }
                        else
                        {
                            foreach (var signature in signatures)
                                builder.Append("(void *)").Append(signature.GetJNIMethodName(method, module)).AppendLine(",");
                        }

                    }
                }
            }

            builder.Append("}").EndOfLine();
        }

        public override string GeneratedPreamble
        {
            get { return CSToJNIConversion.SourcePreamble; }
        }

        public override string FileName
        {
            get { return "MethodInit.cpp"; }
        }
    }
}
