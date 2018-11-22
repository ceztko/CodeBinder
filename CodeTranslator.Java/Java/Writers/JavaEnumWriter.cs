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
    class EnumTypeWriter : BaseTypeWriter<EnumDeclarationSyntax>
    {
        bool _isFlag;

        public EnumTypeWriter(EnumDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context)
        {
            _isFlag = Context.IsFlag(this);
        }

        protected override void WriteTypeMembers()
        {
            WriteEnumMembers();
            Builder.AppendLine();
            Builder.Append("public final int value").EndOfStatement();
            Builder.AppendLine();
            WriteConstructor();
            Builder.AppendLine();
            WriteFromValueMethod();
        }

        private void WriteEnumMembers()
        {
            bool first = true;
            foreach (var member in Context.Members)
            {
                if (first)
                    first = false;
                else
                    Builder.Comma().AppendLine();

                WriteMember(member);
            }

            Builder.EndOfStatement();
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
                Builder.Parenthesized().Append("int value");
            else
                Builder.EmptyParameterList();

            Builder.AppendLine();
            using (Builder.Block())
            {
                if (_isFlag)
                    Builder.Append("this.value = value").EndOfStatement();
                else
                    Builder.Append("value = this.ordinal()").EndOfStatement();
            }
        }

        private void WriteFromValueMethod()
        {
            Builder.Append("public static").Space().Append(TypeName).Space()
                .Append("fromValue(int value)").AppendLine();
            using (Builder.Block())
            {
                if (_isFlag)
                {
                    Builder.Append(TypeName);
                    Builder.Append("[] values =").Space();
                    Builder.Append(TypeName);
                    Builder.Append(".values()").EndOfStatement();
                    Builder.Append("for (int i = 0; i < values.length; i++)").AppendLine();
                    using (Builder.Block())
                    {
                        Builder.Append(TypeName).Space();
                        Builder.Append("envalue = values[i]").EndOfStatement();
                        Builder.Append("if (envalue.value == value)").AppendLine();
                        Builder.IndentChild().Append("return envalue").EndOfStatement();
                    }
                    Builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum").Space()
                        .Append(TypeName).Append("\")").EndOfStatement();
                }
                else
                {
                    Builder.Append("try").AppendLine();
                    using (Builder.Block())
                    {
                        Builder.Append("return").Space();
                        Builder.Append(TypeName);
                        Builder.Append(".values()[value]").EndOfStatement();
                    }
                    Builder.Append("catch (Exception e)").AppendLine();
                    using (Builder.Block())
                    {
                        Builder.Append("throw new RuntimeException(\"Invalid value \" + value + \" for enum").Space()
                            .Append(TypeName).Append("\")").EndOfStatement();
                    }
                }
            }
        }
    }
}
