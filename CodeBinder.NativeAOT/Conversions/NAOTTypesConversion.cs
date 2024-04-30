// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using CodeBinder.CLang;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeBinder.NativeAOT;

class NAOTTypesConversion : NAOTConversionWriter
{
    public bool IsTemplate {  get; set; }

    public NAOTTypesConversion(NAOTCompilationContext compilation, bool template)
        : base(compilation)
    {
        IsTemplate = template;
    }

    protected override string GetFileName() => "types.cs";

    protected override void write(CodeBuilder builder)
    {
        if (IsTemplate)
        {
            writeOpaqueTypes(builder);
        }
        else
        {
            writeEnums(builder);
            writeDefinedTypes(builder);
        }
    }

    void writeOpaqueTypes(CodeBuilder builder)
    {
        if (Compilation.OpaqueTypes.Count == 0)
            return;

        builder.AppendLine("// Opaque types");
        builder.AppendLine();
        foreach (var type in Compilation.OpaqueTypes)
        {
            string? typeStr;
            if (!type.TryGetNAOTBinder(Compilation, out typeStr))
                typeStr = type.Identifier.Text;

            builder.Append("global using ").Append(typeStr).Append(" = System.IntPtr").EndOfStatement();
        }

        builder.AppendLine();
    }

    private void writeDefinedTypes(CodeBuilder builder)
    {
        if (Compilation.StructTypes.Count == 0)
            return;

        builder.AppendLine("// Defined types types");
        builder.AppendLine();
        foreach (var type in Compilation.StructTypes)
        {
            string? typeStr;
            if (!type.TryGetNAOTBinder(Compilation, out typeStr))
                typeStr = type.Identifier.Text;

            builder.Append("struct ").AppendLine(typeStr);
            using (builder.TypeBlock())
            {
                writeStructMembers(builder, type);
            }

            builder.AppendLine();
        }
    }

    private void writeEnums(CodeBuilder builder)
    {
        // Enums
        builder.AppendLine("// Enums");
        builder.AppendLine();
        foreach (var enm in Compilation.Enums)
        {
            var symbol = enm.GetDeclaredSymbol<ITypeSymbol>(Compilation);
            var attributes = symbol.GetAttributes();

            if (attributes.HasAttribute<FlagsAttribute>())
                builder.AppendLine("[Flags]");

            builder.Append("enum ").AppendLine(enm.GetName());
            using (builder.Block())
            {
                foreach (var item in enm.Members)
                {
                    if (item.ShouldDiscard(Compilation))
                        continue;

                    long value = item.GetEnumValue(Compilation);
                    builder.Append(item.GetName()).Space().Append("=").Space().Append(value.ToString()).Comma().AppendLine();
                }
            }

            builder.AppendLine();
        }
    }

    private void writeStructMembers(CodeBuilder builder, StructDeclarationSyntax str)
    {
        foreach (var member in str.Members)
        {
            if (member.Kind() != SyntaxKind.FieldDeclaration)
                continue;

            var field = (member as FieldDeclarationSyntax)!;
            builder.Append(field.Declaration.GetNAOTDeclaration(Compilation)).EndOfStatement();
        }
    }
}
