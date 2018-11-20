using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    class StructTypeWriter : TypeWriter<StructDeclarationSyntax>
    {
        public StructTypeWriter(StructDeclarationSyntax declaration,
                ICompilationContextProvider context)
            : base(declaration, context) { }

        public StructTypeWriter(IReadOnlyList<StructDeclarationSyntax> partialDeclarations,
                ICompilationContextProvider context)
            : base(partialDeclarations, context) { }

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
