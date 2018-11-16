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
        public JavaInterfaceConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override ContextWriter GetTypeWriter()
        {
            return new InterfaceTypeWriter(TypeContext.Node, this);
        }
    }

    class InterfaceTypeWriter : TypeWriter<InterfaceDeclarationSyntax>
    {
        public InterfaceTypeWriter(InterfaceDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        public override bool IsInterface
        {
            get { return true; }
        }

        protected override void WriteTypeMembers()
        {
            WriteTypeMembers(Context.Members);
        }

        protected override void WriteTypeParameters()
        {
            Builder.Append(Context.TypeParameterList, Context.ConstraintClauses, this);
        }

        public override int Arity
        {
            get { return Context.Arity; }
        }
    }
}
