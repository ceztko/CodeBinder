using CodeTranslator.Shared.CSharp;
using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.JNI
{
    class JNIClassConversion : JNITypeConversion<CSharpClassTypeContext>
    {
        public JNIClassConversion(CSToJNIConversion conversion)
            : base(conversion) { }

        protected override SyntaxList<MemberDeclarationSyntax> Members
        {
            get { return TypeContext.Node.Members; }
        }
    }
}
