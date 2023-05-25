// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Utils;
using CodeBinder.Shared;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.CSharp;
using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeBinder.CLang
{
    class CLangTypesHeaderConversion : CLangConversionWriter
    {
        public CLangTypesHeaderConversion(CLangCompilationContext compilation)
            : base(compilation) { }

        protected override string GetFileName() => "Types.h";

        protected override void write(CodeBuilder builder)
        {
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            builder.AppendLine($"#include \"{ConversionCSharpToCLang.BaseTypesHeader}\"");
            builder.AppendLine();
            writeOpaqueTypes(builder);
            builder.AppendLine("#ifdef __cplusplus");
            builder.AppendLine("extern \"C\"");
            builder.AppendLine("{");
            builder.AppendLine("#endif // __cplusplus");
            writeEnums(builder);
            writeFunctionPointerDelegates(builder);
            writeStructTypes(builder);
            builder.AppendLine();
            builder.AppendLine("#ifdef __cplusplus");
            builder.AppendLine("}");
            builder.AppendLine("#endif // __cplusplus");
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
                if (!type.TryGetCLangBinder(Compilation, out typeStr))
                    typeStr = type.Identifier.Text;

                builder.Append("#define").Space().Append(typeStr).Space().Append("void").AppendLine();
            }

            builder.AppendLine();
        }

        private void writeStructTypes(CodeBuilder builder)
        {
            if (Compilation.StructTypes.Count == 0)
                return;

            builder.AppendLine("// Struct types");
            builder.AppendLine();
            foreach (var type in Compilation.StructTypes)
            {
                string? typeStr;
                if (!type.TryGetCLangBinder(Compilation, out typeStr))
                    typeStr = type.Identifier.Text;

                builder.Append("struct").Space().AppendLine(typeStr);
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
                var attributes = enm.GetAttributes(Compilation);
                string enumName = attributes.GetAttribute<NativeBindingAttribute>().GetConstructorArgument<string>(0);
                string stem = attributes.GetAttribute<NativeStemAttribute>().GetConstructorArgument<string>(0);
                (Regex Regex, string Pattern)? substitution = null;
                if (attributes.TryGetAttribute<NativeSubstitutionAttribute>(out var substitutionAttr))
                    substitution = (new Regex(substitutionAttr.GetConstructorArgument<string>(0)), substitutionAttr.GetConstructorArgument<string>(1));

                builder.Append("typedef").Space().Append("enum").AppendLine();
                var splitted = enm.GetName().SplitCamelCase();
                using (builder.TypeBlock(enumName))
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
                            if (substitution == null)
                                splitted = item.GetName().SplitCamelCase();
                            else
                                splitted = new string[] { substitution.Value.Regex.Replace(item.GetName(), substitution.Value.Pattern) };

                            bindedItemName = getCLangSplittedIdentifier(tmpbuilder, stem, splitted);
                        }

                        builder.Append(bindedItemName).Space().Append("=").Space().Append(value.ToString()).Comma().AppendLine();
                    }

                    // Add 32 bit maximum value to enforce enum size
                    // TODO: maximize basing on max enum size, handle negative numbers
                    builder.AppendLine($"__{stem}__ = 0xFFFFFFFF,");
                }

                builder.AppendLine();
            }
        }

        private void writeFunctionPointerDelegates(CodeBuilder builder)
        {
            // Function pointer delegates
            builder.AppendLine("// Function pointer delegates");
            builder.AppendLine();
            foreach (var callback in Compilation.Callbacks)
            {
                string name;
                AttributeData? bidingAttrib;
                if (callback.TryGetAttribute<NativeBindingAttribute>(Compilation, out bidingAttrib))
                    name = bidingAttrib.GetConstructorArgument<string>(0);
                else
                    name = callback.Identifier.Text;

                builder.Append("typedef").Space().Append(callback.GetCLangReturnType(Compilation))
                    .Append("(*").Append(name).Append(")")
                    .Append("(").Append(new CLangParameterListWriter(callback.ParameterList, false, Compilation))
                    .Append(")").EndOfLine();
            }
        }

        private void writeStructMembers(CodeBuilder builder, StructDeclarationSyntax str)
        {
            foreach (var member in str.Members)
            {
                if (member.Kind() != SyntaxKind.FieldDeclaration)
                    continue;

                var field = (member as FieldDeclarationSyntax)!;
                builder.Append(field.Declaration.GetCLangDeclaration(Compilation)).EndOfLine();
            }
        }

        string getCLangSplittedIdentifier(StringBuilder builder, string prefix, string[] splitted)
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
}
