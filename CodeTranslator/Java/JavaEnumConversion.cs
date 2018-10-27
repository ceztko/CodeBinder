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

        public override string TypeName
        {
            get { return TypeContext.Node.GetName(); }
        }

        public override void InitWrite()
        {
            _isFlag = TypeContext.Node.IsFlag(TypeContext);
        }

        public override void WriteDeclaration(IndentStringBuilder builder)
        {
            var modifiers = TypeContext.Node.GetJavaModifiersString();
            if (modifiers != string.Empty)
            {
                builder.Append(modifiers);
                builder.Append(" ");
            }

            builder.Append("enum ");
            builder.Append(TypeName);
            builder.AppendLine(" {");
            builder.IncreaseIndent();
            WriteMembers(builder);
            builder.DecreaseIndent();
            builder.AppendLine("}");
        }

        private void WriteMembers(IndentStringBuilder builder)
        {
            foreach (var member in TypeContext.Node.Members)
                WriteMember(builder, member);
        }

        private void WriteMember(IndentStringBuilder builder, EnumMemberDeclarationSyntax member)
        {
            builder.Append(member.GetName());
            if (_isFlag)
            {
                builder.Append("(");
                builder.Append(member.GetEnumValue(TypeContext).ToString());
                builder.Append(")");
            }

            builder.AppendLine(",");
        }
    }
}
