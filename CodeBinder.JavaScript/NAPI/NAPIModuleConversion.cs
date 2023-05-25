// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Attributes;

namespace CodeBinder.JavaScript.NAPI;

public class NAPIModuleConversion : TypeConversion<NAPIModuleContext, NAPICompilationContext, ConversionCSharpToNAPI>
{
    public ConversionType ConversionType { get; private set; }

    public NAPIModuleConversion(NAPIModuleContext module, ConversionType conversionType, ConversionCSharpToNAPI conversion)
        : base(module, conversion)
    {
        ConversionType = conversionType;
    }

    public string NAPIModuleName => $"NAPI{Context.Name}";

    protected override string GetFileName() => $"{NAPIModuleName}.{FileExtension}";

    protected override string GetGeneratedPreamble() => ConversionCSharpToNAPI.SourcePreamble;

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
        builder.AppendLine("#include \"Internal/JSNAPI.h\"");
        builder.AppendLine();
        builder.AppendLine("namespace js");
        builder.AppendLine("{");
        builder.AppendLine();
        WriteMethods(builder, ConversionType.Header);
        builder.AppendLine("}");
    }

    void writeImplementation(CodeBuilder builder)
    {
        builder.AppendLine($"#include \"{NAPIModuleName}.h\"");
        builder.AppendLine("#include \"Internal/JSInterop.h\"");

        foreach (var include in Context.Includes)
        {
            if (include.Condition.IsNullOrEmpty())
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
        builder.AppendLine("namespace js");
        builder.AppendLine("{");
        WriteMethods(builder, ConversionType.Implementation);
        builder.AppendLine("}");
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
                builder.Append(new NAPITrampolineMethodWriter(method, this, conversionType));
            }

            if (condition != null)
            {
                builder.AppendLine();
                builder.Append("#endif //").Space().Append(condition).AppendLine();
            }

            builder.AppendLine();
        }
    }

    public override NAPICompilationContext Compilation
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
