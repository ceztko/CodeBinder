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
        protected override TypeWriter GetTypeWriter()
        {
            return new EnumTypeWriter(TypeContext.Node, this);
        }
    }

    class EnumTypeWriter : TypeWriter<EnumDeclarationSyntax>
    {
        bool _isFlag;

        public EnumTypeWriter(EnumDeclarationSyntax node, ISemanticModelProvider context)
            : base(node, context)
        {
            _isFlag = Type.IsFlag(this);
        }

        protected override void WriteTypeMembers()
        {
            WriteEnumMembers();
            Builder.AppendLine();
            Builder.AppendLine("public final int value");
            Builder.AppendLine();
            WriteConstructor();
            Builder.AppendLine();
            WriteFromValueMethod();
        }

        private void WriteEnumMembers()
        {
            bool first = true;
            foreach (var member in Type.Members)
            {
                if (first)
                    first = false;
                else
                    Builder.AppendLine(",");

                WriteMember(member);
            }

            Builder.AppendLine(";");
        }

        private void WriteMember(EnumMemberDeclarationSyntax member)
        {
            Builder.Append(member.GetName());
            if (_isFlag)
            {
                Builder.Append("(");
                Builder.Append(member.GetEnumValue(this).ToString());
                Builder.Append(")");
            }
        }

        private void WriteConstructor()
        {
            Builder.Append(TypeName);
            if (_isFlag)
            {
                Builder.AppendLine("(int value) {");
                Builder.IncreaseIndent();
                Builder.AppendLine("this.value = value;");
            }
            else
            {
                Builder.AppendLine("() {");
                Builder.IncreaseIndent();
                Builder.AppendLine("value = this.ordinal();");
            }

            Builder.DecreaseIndent();
            Builder.AppendLine("}");
        }

        private void WriteFromValueMethod()
        {
            Builder.Append("public static ");
            Builder.Append(TypeName);
            Builder.AppendLine(" fromValue(int value) {");
            using (Builder.Indent())
            {
                if (_isFlag)
                {
                    Builder.Append(TypeName);
                    Builder.Append("[] values = ");
                    Builder.Append(TypeName);
                    Builder.AppendLine(".values();");
                    Builder.AppendLine("for (int i = 0; i < values.length; i++) {");
                    using (Builder.Indent())
                    {
                        Builder.Append(TypeName);
                        Builder.AppendLine(" envalue = values[i];");
                        Builder.AppendLine("if (envalue.value == value)");
                        Builder.Indented().AppendLine("return envalue;");
                    }
                    Builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum ");
                    Builder.Append(TypeName);
                    Builder.AppendLine("\");");
                }
                else
                {
                    Builder.AppendLine("try {");
                    using (Builder.Indent())
                    {
                        Builder.Append("return ");
                        Builder.Append(TypeName);
                        Builder.AppendLine(".values()[value];");
                    }
                    Builder.AppendLine("} catch (Exception e) {");
                    using (Builder.Indent())
                    {
                        Builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum ");
                        Builder.Append(TypeName);
                        Builder.AppendLine("\");");
                    }
                    Builder.AppendLine("}");
                }
            }
            Builder.AppendLine("}");
        }
    }
}
