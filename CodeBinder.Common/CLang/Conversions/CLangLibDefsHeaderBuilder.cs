using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    class CLangLibDefsHeaderBuilder : CLangCompilationContextBuilder
    {
        public CLangLibDefsHeaderBuilder(CLangCompilationContext compilation)
            : base(compilation)
        {
        }

        public override string FileName => "libdefs.h";

        public override void write(CodeBuilder builder)
        {
            string libnameUpper = Compilation.LibraryName.ToUpper();
            string LIBRARY_STATIC = $"{libnameUpper}_STATIC";
            string LIBRARY_SHARED = $"{libnameUpper}_SHARED";
            string LIBRARY_SHARED_API = $"{libnameUpper}_SHARED_API";
            string LIBRARY_IMPORT = $"{libnameUpper}_IMPORT";
            string LIBRARY_EXPORT = $"{libnameUpper}_EXPORT";
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            builder.Append("#if").Space().Append("defined(").Append(LIBRARY_SHARED).Append(") || !defined(").Append(LIBRARY_STATIC).AppendLine(")");
            builder.AppendLine();
            builder.Append("#ifdef").Space().AppendLine(LIBRARY_EXPORT);
            using(builder.Indent())
            {
                builder.AppendLine("#ifdef _MSC_VER");
                builder.IndentChild().Append("#define").Space().Append(LIBRARY_SHARED_API).Space().AppendLine("__declspec(dllexport)").Close();
                builder.AppendLine("#else // Non MVSC");
                builder.IndentChild().Append("#define").Space().Append(LIBRARY_SHARED_API).Space().AppendLine("__attribute__ ((visibility (\"default\")))").Close();
                builder.AppendLine("#endif");
            }
            builder.AppendLine("#else");
            using (builder.Indent())
            {
                builder.Append("#define").Space().AppendLine(LIBRARY_IMPORT);
                builder.AppendLine("#ifdef _MSC_VER");
                builder.IndentChild().Append("#define").Space().Append(LIBRARY_SHARED_API).Space().AppendLine("__declspec(dllimport)").Close();
                builder.AppendLine("#else // Non MVSC");
                builder.IndentChild().Append("#define").Space().Append(LIBRARY_SHARED_API).Space().AppendLine("__attribute__ ((visibility (\"default\")))").Close();
                builder.AppendLine("#endif");
            }
            builder.AppendLine("#endif");
            builder.AppendLine();
            builder.AppendLine("#else");
            using (builder.Indent())
            {
                builder.Append("#define").Space().AppendLine(LIBRARY_SHARED_API);
                builder.Append("#ifndef").Space().AppendLine(LIBRARY_EXPORT);
                builder.IndentChild().Append("#define").Space().AppendLine(LIBRARY_IMPORT);
                builder.AppendLine("#endif");
            }

            builder.AppendLine("#endif");
        }
    }
}
