// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Apple;

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

    protected override void write(CodeBuilder builder)
    {
        string libnameUpper = Compilation.ObjCLibraryName.ToUpper();
        string LIBRARY_STATIC = $"{libnameUpper}_STATIC";
        string LIBRARY_SHARED = $"{libnameUpper}_SHARED";
        string LIBRARY_API = $"{libnameUpper}_API";
        builder.AppendLine("#pragma once");
        builder.AppendLine();
        builder.Append("#if").Space().Append("defined(").Append(LIBRARY_SHARED).Append(") || !defined(").Append(LIBRARY_STATIC).AppendLine(")");
        using(builder.Indent())
        {
            builder.Append("#define").Space().Append(LIBRARY_API).Space().AppendLine("__attribute__ ((visibility (\"default\")))");
        }
        builder.AppendLine("#else");
        using (builder.Indent())
        {
            builder.Append("#define").Space().AppendLine(LIBRARY_API);
        }

        builder.AppendLine("#endif");
    }
}
