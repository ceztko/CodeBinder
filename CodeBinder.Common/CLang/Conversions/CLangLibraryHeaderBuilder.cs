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

        public override void write(CodeBuilder builder)
        {
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            foreach (var module in Compilation.Modules)
                builder.Append("#include \"").Append(module.Name).AppendLine(".h\"");
        }

        protected override string GetGeneratedPreamble() => ConversionCSharpToCLang.SourcePreamble;

        public override string FileName
        {
            get { return $"{Compilation.LibraryName}.h"; }
        }
    }
}
