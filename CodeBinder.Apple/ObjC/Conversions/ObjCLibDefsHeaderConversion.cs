// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Attributes;
using CodeBinder.Shared;
using CodeBinder.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Apple
{
    class ObjCLibDefsHeaderConversion : ObjCHeaderConversionWriter
    {
        public ObjCLibDefsHeaderConversion(ObjCCompilationContext compilation)
            : base(compilation)
        {
        }

        protected override string HeaderGuardStem => $"{Compilation.LibraryName.ToUpper()}_LIBDEFS";

        protected override string GetFileName() => HeaderFileName;

        public const string HeaderFileName = "objclibdefs.h";

        public static string GetLibraryApiMacro(ObjCCompilationContext compilation)
        {
            string libnameUpper = compilation.ObjCLibraryName.ToUpper();
            return $"{libnameUpper}_API";
        }

        public static string GetLibraryExportMacro(ObjCCompilationContext compilation)
        {
            string libnameUpper = compilation.ObjCLibraryName.ToUpper();
            return $"{libnameUpper}_EXPORT";
        }

        protected override void write(CodeBuilder builder)
        {
            string libnameUpper = Compilation.ObjCLibraryName.ToUpper();
            string LIBRARY_STATIC = $"{libnameUpper}_STATIC";
            string LIBRARY_SHARED = $"{libnameUpper}_SHARED";
            string LIBRARY_API = $"{libnameUpper}_API";
            string LIBRARY_IMPORT = $"{libnameUpper}_IMPORT";
            string LIBRARY_EXPORT = $"{libnameUpper}_EXPORT";
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            builder.Append("#if").Space().Append("defined(").Append(LIBRARY_SHARED).Append(") || !defined(").Append(LIBRARY_STATIC).AppendLine(")");
            builder.AppendLine();
            builder.Append("#ifdef").Space().AppendLine(LIBRARY_EXPORT);
            using(builder.Indent())
            {
                builder.Append("#define").Space().Append(LIBRARY_API).Space().AppendLine("__attribute__ ((visibility (\"default\")))");
            }
            builder.AppendLine("#else");
            using (builder.Indent())
            {
                builder.Append("#define").Space().AppendLine(LIBRARY_IMPORT);
                builder.Append("#define").Space().AppendLine(LIBRARY_API);
            }
            builder.AppendLine("#endif");
            builder.AppendLine();
            builder.AppendLine("#else");
            using (builder.Indent())
            {
                builder.Append("#define").Space().AppendLine(LIBRARY_API);
                builder.Append("#ifndef").Space().AppendLine(LIBRARY_EXPORT);
                builder.IndentChild().Append("#define").Space().AppendLine(LIBRARY_IMPORT);
                builder.AppendLine("#endif");
            }

            builder.AppendLine("#endif");
        }
    }
}
