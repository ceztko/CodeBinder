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

        protected override ISyntaxWriter GetTypeWriter()
        {
            return new EnumTypeWriter(TypeContext.Node, this);
        }
    }

    class EnumTypeWriter : TypeWriter<EnumDeclarationSyntax>
    {
        bool _isFlag;

        public EnumTypeWriter(EnumDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context)
        {
            _isFlag = Syntax.IsFlag(this);
        }

        protected override void WriteTypeMembers()
        {
            WriteEnumMembers();
            Builder.AppendLine();
            Builder.Append("public final int value").EndOfLine();
            Builder.AppendLine();
            WriteConstructor();
            Builder.AppendLine();
            WriteFromValueMethod();
        }

        private void WriteEnumMembers()
        {
            bool first = true;
            foreach (var member in Syntax.Members)
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
                Builder.Append("(int value)").Space();
            else
                Builder.Append("()").Space();

            using (Builder.BeginBlock())
            {
                if (_isFlag)
                    Builder.Append("this.value = value").EndOfLine();
                else
                    Builder.Append("value = this.ordinal()").EndOfLine();
            }
        }

        private void WriteFromValueMethod()
        {
            Builder.Append("public static").Space();
            Builder.Append(TypeName);
            Builder.Append(" fromValue(int value)").Space();
            using (Builder.BeginBlock())
            {
                if (_isFlag)
                {
                    Builder.Append(TypeName);
                    Builder.Append("[] values =").Space();
                    Builder.Append(TypeName);
                    Builder.Append(".values()").EndOfLine();
                    Builder.Append("for (int i = 0; i < values.length; i++)").Space();
                    using (Builder.BeginBlock())
                    {
                        Builder.Append(TypeName).Space();
                        Builder.Append("envalue = values[i];").EndOfLine();
                        Builder.Append("if (envalue.value == value)");
                        Builder.Indented().Append("return envalue").EndOfLine();
                    }
                    Builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum").Space()
                        .Append(TypeName).Append("\")").EndOfLine();
                }
                else
                {
                    Builder.Append("try").Space();
                    using (Builder.BeginBlock(false))
                    {
                        Builder.Append("return").Space();
                        Builder.Append(TypeName);
                        Builder.Append(".values()[value]").EndOfLine();
                    }
                    Builder.Space().Append("catch (Exception e)").Space();
                    using (Builder.BeginBlock())
                    {
                        Builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum").Space()
                            .Append(TypeName).Append("\")").EndOfLine();
                    }
                }
            }
        }
    }
}
