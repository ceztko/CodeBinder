// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBinder.CLang;

public class CLangModuleConversion : TypeConversion<CLangModuleContext, CLangCompilationContext, ConversionCSharpToCLang>
{
    public ModuleConversionType ConversionType { get; private set; }

    public CLangModuleConversion(CLangModuleContext context, ModuleConversionType conversionType, ConversionCSharpToCLang conversion)
        : base(context, conversion)
    {
        ConversionType = conversionType;
    }

    protected override string GetFileName() => $"{ModuleName}.{FileExtension}";

    public string ModuleName
    {
        get { return Context.Name; }
    }

    protected sealed override void write(CodeBuilder builder)
    {
        switch (ConversionType)
        {
            case ModuleConversionType.CHeader:
                WriteCHeader(builder);
                break;
            case ModuleConversionType.CppTrampoline:
                WriteCppTrampoline(builder);
                break;
            default:
                throw new NotSupportedException();
        }
    }

    void WriteCHeader(CodeBuilder builder)
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
        WriteClangMethods(builder, false);
        builder.AppendLine("#ifdef __cplusplus");
        builder.AppendLine("}");
        builder.AppendLine("#endif");
    }

    void WriteCppTrampoline(CodeBuilder builder)
    {
        builder.AppendLine($"#include \"{ModuleName}.h\"");
        builder.AppendLine("#include \"CBInterop.hpp\"");
        builder.AppendLine();
        builder.AppendLine("// Cpp implementations declarations");
        builder.AppendLine($"namespace {Context.Compilation.LibraryName.ToLower()}");
        builder.AppendLine("{");
        builder.AppendLine();
        WriteCppMethodDeclarations(builder);
        builder.AppendLine("}");
        builder.AppendLine("// C trampolines");
        builder.AppendLine("extern \"C\"");
        builder.AppendLine("{");
        builder.AppendLine();
        WriteClangMethods(builder, true);
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine();
    }

    private void WriteClangMethods(CodeBuilder builder, bool writeBody)
    {
        foreach (var method in Context.Methods)
        {
            if (writeBody)
                builder.Append(new CLangMethodTrampolineWriter(method, this));
            else
                builder.Append(new CLangMethodDeclarationWriter(method, false, this));

            builder.AppendLine();
        }
    }

    private void WriteCppMethodDeclarations(CodeBuilder builder)
    {
        foreach (var method in Context.Methods)
        {
            builder.Append(new CLangMethodDeclarationWriter(method, true, this));
            builder.AppendLine();
        }
    }

    protected override string GetGeneratedPreamble() => ConversionCSharpToCLang.SourcePreamble;

    public override CLangCompilationContext Compilation
    {
        get { return Context.Compilation; }
    }

    string FileExtension
    {
        get
        {
            switch(ConversionType)
            {
                case ModuleConversionType.CHeader:
                    return "h";
                case ModuleConversionType.CppTrampoline:
                    return "ipp";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

public enum ModuleConversionType
{
    /// <summary>
    /// Regular header with C signatures
    /// </summary>
    CHeader,

    /// <summary>
    /// Cpp method trampolines, that cares about special string types
    /// </summary>
    CppTrampoline
}
