using CodeBinder.Util;
using CodeBinder.Shared;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.CSharp;
using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CodeBinder.CLang
{
    class CLangTypesHeaderBuilder : CLangCompilationContextBuilder
    {
        public CLangTypesHeaderBuilder(CLangCompilationContext compilation)
            : base(compilation) { }

        protected override string GetFileName() => "Types.h";

        protected override void write(CodeBuilder builder)
        {
            builder.AppendLine("#pragma once");
            builder.AppendLine();
            builder.AppendLine("#include \"Internal/BaseTypes.h\"");
            builder.AppendLine();
            writeOpaqueTypes(builder);
            builder.AppendLine("#ifdef __cplusplus");
            builder.AppendLine("extern \"C\"");
            builder.AppendLine("{");
            builder.AppendLine("#endif // __cplusplus");
            writeEnums(builder);
            writeCallbacks(builder);
            builder.AppendLine();
            builder.AppendLine("#ifdef __cplusplus");
            builder.AppendLine("}");
            builder.AppendLine("#endif // __cplusplus");
        }

        void writeOpaqueTypes(CodeBuilder builder)
        {
            // Opaque types
            builder.AppendLine("// Opaque types");
            builder.AppendLine();
            foreach (var type in Compilation.Types)
            {
                builder.Append("#define").Space().Append(type.Identifier.Text).Space().Append("void").AppendLine();
            }

            builder.AppendLine();
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
                Regex? substitutionRegex = null;
                string? substitutionPattern = null;
                if (attributes.TryGetAttribute<NativeSubstitutionAttribute>(out var substitutionAttr))
                {
                    substitutionRegex = new Regex(substitutionAttr.GetConstructorArgument<string>(0));
                    substitutionPattern = substitutionAttr.GetConstructorArgument<string>(1);
                }

                builder.Append("enum").Space().AppendLine(enumName);
                var splitted = enm.GetName().SplitCamelCase();
                using (builder.TypeBlock())
                {
                    foreach (var item in enm.Members)
                    {
                        var itemattribs = item.GetAttributes(Compilation);
                        if (itemattribs.HasAttribute<NativeIgnoreAttribute>())
                            continue;

                        long value = item.GetEnumValue(Compilation);
                        string bindedItemName;
                        AttributeData? attr;
                        if (itemattribs.TryGetAttribute<NativeBindingAttribute>(out attr))
                        {
                            bindedItemName = attr.GetConstructorArgument<string>(0);
                        }
                        else
                        {
                            if (substitutionRegex == null)
                                splitted = item.GetName().SplitCamelCase();
                            else
                                splitted = new string[] { substitutionRegex.Replace(item.GetName(), substitutionPattern) };

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

        private void writeCallbacks(CodeBuilder builder)
        {
            // Callbacks
            builder.AppendLine("// Callbacks");
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
                    .Append("(").Append(new CLangParameterListWriter(callback.ParameterList, Compilation)).Append(")")
                    .EndOfLine();
            }
        }

        string getCLangSplittedIdentifier(StringBuilder builder, string prefix, string[] splitted)
        {
            builder.Clear();
            if (prefix != null)
            {
                builder.Append(prefix);
                builder.Append("_");
            }

            bool first = true;
            foreach (var str in splitted)
            {
                if (first)
                    first = false;
                else
                    builder.Append("_");
                builder.Append(str.ToUpperInvariant());
            }

            return builder.ToString();
        }
    }
}
