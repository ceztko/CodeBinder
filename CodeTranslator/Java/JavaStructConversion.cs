using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    class JavaStructConversion : JavaTypeConversion<CSharpStructTypeContext>
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
