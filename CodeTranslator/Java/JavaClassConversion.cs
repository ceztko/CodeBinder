using CodeTranslator.Shared.CSharp;
using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    class JavaClassConversion : JavaTypeConversion<CSharpClassTypeContext>
    {
        public override void WriteTypeBody(IndentStringBuilder builder)
        {

        }

        public override string TypeDeclaration
        {
            get { return "class"; }
        }
    }
}
