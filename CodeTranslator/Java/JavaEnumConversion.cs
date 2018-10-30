// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeTranslator.Util;
using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeTranslator.Java
{
    class JavaEnumConversion : JavaTypeConversion<CSharpEnumTypeContext>
    {
        bool _isFlag;

        public override void InitWrite()
        {
            _isFlag = TypeContext.Node.IsFlag(this);
        }

        protected override void WriteTypeMembers(IndentStringBuilder builder, BaseTypeDeclarationSyntax type)
        {
            WriteMembers(builder);
            builder.AppendLine();
            builder.AppendLine("public final int value");
            builder.AppendLine();
            WriteConstructor(builder);
            builder.AppendLine();
            WriteFromValueMethod(builder);
        }

        private void WriteMembers(IndentStringBuilder builder)
        {
            bool first = true;
            foreach (var member in TypeContext.Node.Members)
            {
                if (first)
                    first = false;
                else
                    builder.AppendLine(",");

                WriteMember(builder, member);
            }

            builder.AppendLine(";");
        }

        private void WriteMember(IndentStringBuilder builder, EnumMemberDeclarationSyntax member)
        {
            builder.Append(member.GetName());
            if (_isFlag)
            {
                builder.Append("(");
                builder.Append(member.GetEnumValue(this).ToString());
                builder.Append(")");
            }
        }

        private void WriteConstructor(IndentStringBuilder builder)
        {
            builder.Append(TypeName);
            if (_isFlag)
            {
                builder.AppendLine("(int value) {");
                builder.IncreaseIndent();
                builder.AppendLine("this.value = value;");
            }
            else
            {
                builder.AppendLine("() {");
                builder.IncreaseIndent();
                builder.AppendLine("value = this.ordinal();");
            }

            builder.DecreaseIndent();
            builder.AppendLine("}");
        }

        private void WriteFromValueMethod(IndentStringBuilder builder)
        {
            builder.Append("public static ");
            builder.Append(TypeName);
            builder.AppendLine(" fromValue(int value) {");
            using (builder = builder.Indent())
            {
                if (_isFlag)
                {
                    builder.Append(TypeName);
                    builder.Append("[] values = ");
                    builder.Append(TypeName);
                    builder.AppendLine(".values();");
                    builder.AppendLine("for (int i = 0; i < values.length; i++) {");
                    using (builder = builder.Indent())
                    {
                        builder.Append(TypeName);
                        builder.AppendLine(" envalue = values[i];");
                        builder.AppendLine("if (envalue.value == value)");
                        builder.Indented().AppendLine("return envalue;");
                    }
                    builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum ");
                    builder.Append(TypeName);
                    builder.AppendLine("\");");
                }
                else
                {
                    builder.AppendLine("try {");
                    using (builder = builder.Indent())
                    {
                        builder.Append("return ");
                        builder.Append(TypeName);
                        builder.AppendLine(".values()[value];");
                    }
                    builder.AppendLine("} catch (Exception e) {");
                    using (builder = builder.Indent())
                    {
                        builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum ");
                        builder.Append(TypeName);
                        builder.AppendLine("\");");
                    }
                    builder.AppendLine("}");
                }
            }
            builder.AppendLine("}");
        }
    }
}
