// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using CodeBinder.Java;
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
        public ConversionType ConversionType { get; private set; }

        public JNIModuleConversion(JNIModuleContext module, ConversionType conversionType, ConversionCSharpToJNI conversion)
            : base(module, conversion)
        {
            ConversionType = conversionType;
        }

        public string JNIModuleName => $"JNI{Context.Name}";

        protected override string GetFileName() => $"{JNIModuleName}.{FileExtension}";

        protected override string GetGeneratedPreamble() => ConversionCSharpToJNI.SourcePreamble;

        protected sealed override void write(CodeBuilder builder)
        {
            switch (ConversionType)
            {
                case ConversionType.Header:
                    writeHeader(builder);
                    break;
                case ConversionType.Implementation:
                    writeImplementation(builder);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        void writeHeader(CodeBuilder builder)
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
            WriteMethods(builder, ConversionType.Header);
            builder.AppendLine("#ifdef __cplusplus");
            builder.AppendLine("}");
            builder.AppendLine("#endif");
        }

        void writeImplementation(CodeBuilder builder)
        {
            builder.AppendLine("#include \"Internal/JNICommon.h\"");
            builder.AppendLine($"#include \"{JNIModuleName}.h\"");

            foreach (var include in Context.Includes)
            {
                if (string.IsNullOrEmpty(include.Condition))
                {
                    builder.Append("#include").Space().AppendLine(include.Name);
                }
                else
                {
                    builder.Append("#ifdef").Space().AppendLine(include.Condition);
                    builder.Append("#include").Space().AppendLine(include.Name);
                    builder.Append("#endif //").Space().AppendLine(include.Condition);
                }
            }

            builder.AppendLine($"#include <{Context.Name}.h>");

            builder.AppendLine();
            WriteMethods(builder, ConversionType.Implementation);
        }

        private void WriteMethods(CodeBuilder builder, ConversionType conversionType)
        {
            foreach (var method in Context.Methods)
            {
                string? condition = null;
                if (method.TryGetAttribute<ConditionAttribute>(this, out var attr))
                {
                    condition = attr.GetConstructorArgument<string>(0);
                    builder.Append("#ifdef").Space().Append(condition).AppendLine();
                }

                if (method.TryGetAttribute<VerbatimConversionAttribute>(Context, out var attribute)
                    && (attribute.ConstructorArguments.Length == 1 ||
                        attribute.GetConstructorArgument<ConversionType>(0) == conversionType))
                {
                    // Use the verbatim conversion instead
                    string verbatimStr = attribute.ConstructorArguments.Length == 1
                        ? attribute.GetConstructorArgument<string>(0)
                        : attribute.GetConstructorArgument<string>(1);
                    builder.AppendLine(verbatimStr);
                }
                else
                {
                    builder.Append(new JNITrampolineMethodWriter(method, this, conversionType));
                }

                if (condition != null)
                {
                    builder.AppendLine();
                    builder.Append("#endif //").Space().Append(condition).AppendLine();
                }

                builder.AppendLine();
            }
        }

        public override JNICompilationContext Compilation
        {
            get { return Context.Compilation; }
        }

        string FileExtension
        {
            get
            {
                return ConversionType switch
                {
                    ConversionType.Header => "h",
                    ConversionType.Implementation => "cpp",
                    _ => throw new NotSupportedException()
                };
            }
        }
    }
}
