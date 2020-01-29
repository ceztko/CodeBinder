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

namespace CodeBinder.CLang
{
    public class CLangModuleConversion : TypeConversion<CLangModuleContext, CLangCompilationContext, ConversionCSharpToCLang>
    {
        public CLangModuleConversion(CLangModuleContext context, ConversionCSharpToCLang conversion)
            : base(context, conversion) { }

        public override string FileName
        {
            get { return ModuleName + ".h"; }
        }

        public string ModuleName
        {
            get { return Context.Name; }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            builder.AppendLine("#include \"libdefs.h\"");
            builder.AppendLine("#include \"Types.h\"");
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
            void writeMethods(bool widechar)
            {
                foreach (var method in Context.Methods)
                {
                    builder.Append(CLangMethodWriter.Create(method, widechar, this));
                    builder.AppendLine();
                }
            }

            //writeMethods(true);
            writeMethods(false);
        }

        public override string GeneratedPreamble
        {
            get { return ConversionCSharpToCLang.SourcePreamble; }
        }

        public override CLangCompilationContext Compilation
        {
            get { return Context.Compilation; }
        }
    }
}
