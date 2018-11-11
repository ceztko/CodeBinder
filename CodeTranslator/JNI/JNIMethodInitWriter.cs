using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.JNI
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
            foreach (var module in _conversion.RootTypes)
                builder.Append("#include \"JNI").Append(module.Name).AppendLine(".h\"");

            builder.AppendLine();
            builder.AppendLine("static void* funcs[] = {");
            using (builder.Indent())
            {
                foreach (var module in _conversion.RootTypes)
                {
                    foreach (var method in module.Methods)
                    {
                        var signatures = method.GetMethodSignatures(module);
                        if (signatures.Length == 0)
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
