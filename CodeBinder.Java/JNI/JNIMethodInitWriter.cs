using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.JNI
{
    class MethodInitWriter : ConversionBuilder
    {
        CSToJNIConversion _conversion;

        public MethodInitWriter(CSToJNIConversion conversion)
        {
            _conversion = conversion;
        }

        public override void Write(CodeBuilder builder)
        {
            builder.AppendLine("#include \"JNITypesPrivate.h\"");
            foreach (var module in _conversion.Modules)
                builder.Append("#include \"JNI").Append(module.Name).AppendLine(".h\"");

            builder.AppendLine();
            builder.AppendLine("static void* funcs[] = {");
            using (builder.Indent())
            {
                foreach (var module in _conversion.Modules)
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
