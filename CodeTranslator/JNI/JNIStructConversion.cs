using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.JNI
{
    class JNIStructConversion : JNITypeConversion<CSharpStructTypeContext>
    {
        public JNIStructConversion(CSToJNIConversion conversion)
            : base(conversion) { }

        protected override SyntaxList<MemberDeclarationSyntax> Members
        {
            get { return TypeContext.Node.Members; }
        }
    }
}
