// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using System.Linq;

namespace CodeBinder.Apple;

class ObjCTypesHeaderConversion : ObjCHeaderConversionWriter
{
    public bool IsInternalHeader { get; private set; }

    public ObjCTypesHeaderConversion(ObjCCompilationContext compilation, bool isInternalHeader)
        : base(compilation)
    {
        IsInternalHeader = isInternalHeader;
    }

    protected override string GetFileName() => ConversionCSharpToObjC.TypesHeader;

    protected override string? GetBasePath() => IsInternalHeader ? ConversionCSharpToObjC.InternalBasePath : null;

    protected override string? GetGeneratedPreamble() => ConversionCSharpToObjC.SourcePreamble;

    protected override void write(CodeBuilder builder)
    {
        BeginHeaderGuard(builder);
        builder.AppendLine();
        if (IsInternalHeader)
        {
            builder.AppendLine($"#include <stdexcept>");
            builder.AppendLine($"#import \"../{ConversionCSharpToObjC.TypesHeader}\"");
            // TODO: CBOCBinderUtils.h should be included in a currently missing
            // internal only header that only has tools for code generation
            builder.AppendLine($"#import \"{nameof(ObjCResources.CBOCBinderUtils_h).ToObjCHeaderFilename()}\"");
        }
        else
        {
            builder.AppendLine($"#import \"{BaseTypesHeader}\"");
        }

        builder.AppendLine();
        if (IsInternalHeader)
        {
            writeEnumToStringMethods(builder);
            builder.AppendLine();
        }
        else
        {
            writeOpaqueTypes(builder);
            builder.AppendLine();
            writeEnums(builder);
            builder.AppendLine();
            writeCallbacks(builder);
            builder.AppendLine();
        }

        EndHeaderGuard(builder);
    }

    void writeOpaqueTypes(CodeBuilder builder)
    {
        // Forward declared classes
        builder.AppendLine("// Forward declared classes");
        builder.AppendLine();
        foreach (var type in Compilation.StorageTypes.Declarations)
        {
            builder.Append("@class").Space().Append(type.GetObjCName(Compilation)).EndOfStatement();
        }

        builder.AppendLine();

        // Forward declared interfaces
        builder.AppendLine("// Forward declared interfaces");
        builder.AppendLine();
        foreach (var iface in Compilation.Interfaces.Declarations)
        {
            builder.Append("@protocol").Space().Append(iface.GetObjCName(Compilation)).EndOfStatement();
        }
    }

    private void writeEnums(CodeBuilder builder)
    {
        // Enums
        builder.AppendLine("// Enums");
        builder.AppendLine();
        foreach (var enm in Compilation.Enums.Declarations)
        {
            string enumName = enm.GetObjCName(Compilation);
            var symbol = enm.GetDeclaredSymbol<INamedTypeSymbol>(Compilation);
            bool isflag = symbol.HasAttribute<FlagsAttribute>();
            string underlyingType = symbol.EnumUnderlyingType!.GetObjCPrimitiveType();
            builder.Append("typedef").Space().Append(isflag ? "NS_OPTIONS" : "NS_ENUM").Parenthesized()
                .Append(underlyingType).CommaSeparator().Append(enumName).Close().AppendLine();

            var members = getEnumMembers(enm);
            using (builder.TypeBlock())
            {
                foreach (var member in members)
                    builder.Append(member.Name).Space().Append("=").Space().Append(member.Value.ToString()).Comma().AppendLine();
            }

            builder.AppendLine();
        }
    }

    private void writeEnumToStringMethods(CodeBuilder builder)
    {
        builder.AppendLine("// CBToString for enums");
        foreach (var enm in Compilation.Enums.Declarations)
        {
            string enumName = enm.GetObjCName(Compilation);
            var members = getEnumMembers(enm, true);
            writeCBToStringMethod(builder, enumName, enm.IsFlag(Compilation), members);
            builder.AppendLine();
        }
    }

    // Write a CBToString() method for this enum
    private void writeCBToStringMethod(CodeBuilder builder, string enumName, bool isFlag, List<EnumMember> members)
    {
        builder.Append("inline").Space().Append("NSString *").Space().Append("CBToString").Parenthesized().Append(isFlag ? $"{enumName}_Options" : enumName).Space().Append("value").Close().AppendLine();
        using (builder.Block())
        {
            builder.Append("switch (value)").AppendLine();
            using (builder.Block())
            {
                foreach (var member in members)
                {
                    builder.Append("case").Space().Append(member.Name).Colon().AppendLine();
                    builder.IndentChild().Append("return").Append($"@\"{member.Name}\"").EndOfStatement();
                }

                builder.Append("default").Colon().AppendLine();
                builder.IndentChild().Append("throw std::runtime_error(\"Unsupported\")").EndOfStatement();
            }
        }
    }

    List<EnumMember> getEnumMembers(EnumDeclarationSyntax enm, bool skipAliases = false)
    {
        var enums = new List<EnumMember>(enm.Members.Count);
        foreach (var item in enm.Members)
        {
            long value = item.GetEnumValue(Compilation);
            if (skipAliases && enums.Any((enm) => enm.Value == value))
                continue;

            string name = item.GetObjCName(Compilation);
            enums.Add(new EnumMember() { Name = name, Value = value });
        }

        return enums;
    }

    private void writeCallbacks(CodeBuilder builder)
    {
        // Callbacks
        builder.AppendLine("// Callbacks");
        builder.AppendLine();
        foreach (var callback in Compilation.Delegates.Declarations)
        {
            string name;
            AttributeData? bidingAttrib;
            if (callback.TryGetAttribute<NativeBindingAttribute>(Compilation, out bidingAttrib))
                name = bidingAttrib.GetConstructorArgument<string>(0);
            else
                name = callback.Identifier.Text;

            throw new NotImplementedException();
            /* TODO
            builder.Append("typedef").Space().Append(callback.GetCLangReturnType(Compilation))
                .Append("(*").Append(name).Append(")")
                .Append("(").Append(new CLangParameterListWriter(callback.ParameterList, Compilation)).Append(")")
                .EndOfLine();
            */
        }
    }

    bool ShouldSkipType(BaseTypeDeclarationSyntax type)
    {
        bool publicType = type.HasAccessibility(Accessibility.Public, Compilation);
        if (IsInternalHeader)
        {
            if (publicType)
                return true;
        }
        else
        {
            if (!publicType)
                return true;
        }

        return false;
    }

    protected override string HeaderGuardStem => IsInternalHeader ? "INTERNAL_TYPES" : "TYPES";

    public string BaseTypesHeader => IsInternalHeader
        ? $"../{ConversionCSharpToObjC.BaseTypesHeader}"
        : ConversionCSharpToObjC.BaseTypesHeader;


    struct EnumMember
    {
        public string Name;
        public long Value;
    }
}
