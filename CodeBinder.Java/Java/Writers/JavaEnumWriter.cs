// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeBinder.Util;
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeBinder.Java
{
    class JavaEnumWriter : JavaBaseTypeWriter<EnumDeclarationSyntax>
    {
        bool _isOrdinalEnum;

        public JavaEnumWriter(EnumDeclarationSyntax syntax, JavaCodeConversionContext context)
            : base(syntax, context)
        {
            _isOrdinalEnum = !Item.IsFlag(Context);
            if (_isOrdinalEnum)
            {
                for (int i = 0; i < syntax.Members.Count; i++)
                {
                    var member = syntax.Members[i];
                    long value = member.GetEnumValue(Context);
                    if (value != i)
                        _isOrdinalEnum = false;
                }
            }
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
            foreach (var member in Item.Members)
            {
                Builder.CommaAppendLine(ref first);
                WriteMember(member);
            }

            Builder.EndOfStatement();
        }

        private void WriteMember(EnumMemberDeclarationSyntax member)
        {
            Builder.Append(member.GetName());
            if (!_isOrdinalEnum)
            {
                Builder.Append("(");
                Builder.Append(member.GetEnumValue(Context).ToString());
                Builder.Append(")");
            }
        }

        private void WriteConstructor()
        {
            Builder.Append(TypeName);
            if (_isOrdinalEnum)
                Builder.EmptyParameterList();
            else
                Builder.Parenthesized().Append("int value");

            Builder.AppendLine();
            using (Builder.Block())
            {
                if (_isOrdinalEnum)
                    Builder.Append("value = this.ordinal()").EndOfStatement();
                else
                    Builder.Append("this.value = value").EndOfStatement();
            }
        }

        private void WriteFromValueMethod()
        {
            Builder.Append("public static").Space().Append(TypeName).Space()
                .Append("fromValue(int value)").AppendLine();
            using (Builder.Block())
            {
                if (_isOrdinalEnum)
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
                else
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
            }
        }
    }
}
