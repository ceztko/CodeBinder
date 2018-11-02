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
        public JavaEnumConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override TypeWriter GetTypeWriter()
        {
            return new EnumTypeWriter(TypeContext.Node, this);
        }
    }

    class EnumTypeWriter : TypeWriter<EnumDeclarationSyntax>
    {
        bool _isFlag;

        public EnumTypeWriter(EnumDeclarationSyntax node, ICompilationContextProvider context)
            : base(node, context)
        {
            _isFlag = Type.IsFlag(this);
        }

        protected override void WriteTypeMembers()
        {
            WriteEnumMembers();
            Builder.AppendLine();
            Builder.AppendLine("public final int value;");
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
                Builder.Append("(int value) ");
            else
                Builder.Append("() ");

            using (Builder.BeginBlock())
            {
                if (_isFlag)
                    Builder.AppendLine("this.value = value;");
                else
                    Builder.AppendLine("value = this.ordinal();");
            }
        }

        private void WriteFromValueMethod()
        {
            Builder.Append("public static ");
            Builder.Append(TypeName);
            Builder.Append(" fromValue(int value) ");
            using (Builder.BeginBlock())
            {
                if (_isFlag)
                {
                    Builder.Append(TypeName);
                    Builder.Append("[] values = ");
                    Builder.Append(TypeName);
                    Builder.AppendLine(".values();");
                    Builder.Append("for (int i = 0; i < values.length; i++) ");
                    using (Builder.BeginBlock())
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
                    Builder.Append("try ");
                    using (Builder.BeginBlock(false))
                    {
                        Builder.Append("return ");
                        Builder.Append(TypeName);
                        Builder.AppendLine(".values()[value];");
                    }
                    Builder.Append(" catch (Exception e) ");
                    using (Builder.BeginBlock())
                    {
                        Builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum ");
                        Builder.Append(TypeName);
                        Builder.AppendLine("\");");
                    }
                }
            }
        }
    }
}
