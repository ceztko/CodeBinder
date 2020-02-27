﻿using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    class CLangLibraryHeaderConversion : CLangConversionWriter
    {
        public CLangLibraryHeaderConversion(CLangCompilationContext compilation)
            : base(compilation) { }

        protected override void write(CodeBuilder builder)
        {
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            foreach (var module in Compilation.Modules)
                builder.Append("#include \"").Append(module.Name).AppendLine(".h\"");
        }

        protected override string GetGeneratedPreamble() => ConversionCSharpToCLang.SourcePreamble;

        protected override string GetFileName() => $"{Compilation.LibraryName}.h";
    }
}