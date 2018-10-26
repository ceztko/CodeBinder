// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeTranslator.Util;
using CodeTranslator.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeTranslator.Java
{
    class JavaEnumConversion : JavaTypeConversion
    {
        CSharpEnumTypeContext _context;
        bool _isFlag;

        public JavaEnumConversion(CSharpEnumTypeContext node)
        {
            _context = node;
            _isFlag = node.IsFlag();
        }

        public override string TypeName
        {
            get { return _context.Node.GetName(); }
        }

        public override void WriteDeclaration(IndentStringBuilder builder)
        {
            var modifiers = _context.Node.GetJavaModifiersString();
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
            foreach (var member in _context.Node.Members)
                WriteMember(builder, member);
        }

        private void WriteMember(IndentStringBuilder builder, EnumMemberDeclarationSyntax member)
        {
            builder.Append(member.GetName());
            if (_isFlag)
            {

            }

            builder.AppendLine(",");
        }
    }
}
