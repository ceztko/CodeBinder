using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    class CLangLibraryHeaderBuilder : CLangCompilationContextBuilder
    {
        public CLangLibraryHeaderBuilder(CLangCompilationContext compilation)
            : base(compilation) { }

        public override void Write(CodeBuilder builder)
        {
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            foreach (var module in Compilation.Modules)
                builder.Append("#include \"").Append(module.Name).AppendLine(".h\"");
        }

        public override string GeneratedPreamble
        {
            get { return ConversionCSharpToCLang.SourcePreamble; }
        }

        public override string FileName
        {
            get { return $"{Compilation.LibraryName}.h"; }
        }
    }
}
