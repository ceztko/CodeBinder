using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    class JavaInterfaceConversion : JavaTypeConversion<CSharpInterfaceTypeContext>
    {
        protected override TypeWriter GetTypeWriter()
        {
            return new InterfaceTypeWriter(TypeContext.Node, this);
        }
    }

    class InterfaceTypeWriter : TypeWriter<InterfaceDeclarationSyntax>
    {
        public InterfaceTypeWriter(InterfaceDeclarationSyntax node, ICompilationContextProvider context)
            : base(node, context) { }

        protected override void WriteTypeMembers()
        {
            WriteTypeMembers(Type.Members);
        }
    }
}
