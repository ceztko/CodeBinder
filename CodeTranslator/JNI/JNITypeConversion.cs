// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTranslator.JNI
{
    abstract class JNITypeConversion<TTypeContext> : CSharpTypeConversion<TTypeContext, CSToJNIConversion>
        where TTypeContext : CSharpTypeContext
    {
        public string Namespace { get; private set; }
        string _Basepath;

        protected JNITypeConversion(CSToJNIConversion langConversion)
            : base(langConversion)
        {
            Namespace = langConversion.BaseNamespace;
            _Basepath = string.IsNullOrEmpty(Namespace) ? null : Namespace.Replace('.', Path.DirectorySeparatorChar);
        }

        public override string FileName
        {
            get { throw new NotImplementedException(); }
        }

        public sealed override void Write(CodeBuilder builder)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IConversionBuilder> Builders
        {

            get
            {
                visitMembers();
                yield break;
            }
        }

        private void visitMembers()
        {
            foreach (var member in Members)
            {
                if (member.Kind() != SyntaxKind.MethodDeclaration)
                    continue;

                var method = member as MethodDeclarationSyntax;
            }
        }

        protected abstract SyntaxList<MemberDeclarationSyntax> Members
        {
            get;
        }

    }
}
