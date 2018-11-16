using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    class JavaStructConversion : JavaTypeConversion<CSharpStructTypeContext>
    {
        public JavaStructConversion(CSToJavaConversion conversion)
            : base(conversion) { }

        protected override CodeWriter GetTypeWriter()
        {
            return new StructTypeWriter(TypeContext.Node, this);
        }
    }

    class StructTypeWriter : TypeWriter<StructDeclarationSyntax>
    {
        public StructTypeWriter(StructDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

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
