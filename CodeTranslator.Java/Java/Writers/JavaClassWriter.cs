using CodeTranslator.Shared.CSharp;
using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    class ClassTypeWriter : TypeWriter<ClassDeclarationSyntax>
    {
        public ClassTypeWriter(ClassDeclarationSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void WriteTypeMembers()
        {
            WriteTypeMembers(Context.Members);
        }

        protected override void WriteTypeParameters()
        {
            Builder.Append(Context.GetTypeParameters(), this).Space();
        }

        public override int Arity
        {
            get { return Context.Arity; }
        }

        public override bool NeedStaticKeyword
        {
            get { return true; }
        }
    }
}
