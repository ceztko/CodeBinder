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
        protected override TypeWriter GetTypeWriter()
        {
            return new ClassTypeWriter(TypeContext.Node, this);
        }
    }

    class ClassTypeWriter : TypeWriter<ClassDeclarationSyntax>
    {
        public ClassTypeWriter(ClassDeclarationSyntax node, ISemanticModelProvider context)
            : base(node, context) { }

        protected override void WriteTypeMembers()
        {
            WriteTypeMembers(Type.Members);
        }
    }
}
