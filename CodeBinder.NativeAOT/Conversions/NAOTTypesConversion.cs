// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
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
        var tmpbuilder = new StringBuilder();
        foreach (var enm in Compilation.Enums)
        {
            var symbol = enm.GetDeclaredSymbol<ITypeSymbol>(Compilation);
            var attributes = symbol.GetAttributes();
            string enumName = attributes.GetAttribute<NativeBindingAttribute>().GetConstructorArgument<string>(0);
            string stem = attributes.GetAttribute<NativeStemAttribute>().GetConstructorArgument<string>(0);
            (Regex Regex, string Pattern)? substitution = null;
            if (attributes.TryGetAttribute<NativeSubstitutionAttribute>(out var substitutionAttr))
                substitution = (new Regex(substitutionAttr.GetConstructorArgument<string>(0)), substitutionAttr.GetConstructorArgument<string>(1));

            if (attributes.HasAttribute<FlagsAttribute>())
                builder.AppendLine("[Flags]");

            builder.Append("enum ").AppendLine(enumName);
            using (builder.Block())
            {
                foreach (var item in enm.Members)
                {
                    if (item.ShouldDiscard(Compilation))
                        continue;

                    long value = item.GetEnumValue(Compilation);
                    string bindedItemName;
                    AttributeData? attr;
                    if (item.TryGetAttribute<NativeBindingAttribute>(Compilation, out attr))
                    {
                        bindedItemName = attr.GetConstructorArgument<string>(0);
                    }
                    else
                    {
                        string[] splitted;
                        if (substitution == null)
                            splitted = item.GetName().SplitCamelCase();
                        else
                            splitted = new string[] { substitution.Value.Regex.Replace(item.GetName(), substitution.Value.Pattern) };

                        bindedItemName = getNAOTSplittedIdentifier(tmpbuilder, stem, splitted);
                    }

                    builder.Append(bindedItemName).Space().Append("=").Space().Append(value.ToString()).Comma().AppendLine();
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

    string getNAOTSplittedIdentifier(StringBuilder builder, string prefix, string[] splitted)
    {
        builder.Clear();
        if (prefix != null)
        {
            builder.Append(prefix);
            builder.Append('_');
        }

        bool first = true;
        foreach (var str in splitted)
        {
            if (first)
                first = false;
            else
                builder.Append('_');
            builder.Append(str.ToUpperInvariant());
        }

        return builder.ToString();
    }
}
