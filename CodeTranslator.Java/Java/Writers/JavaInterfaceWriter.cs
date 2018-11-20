using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    class InterfaceTypeWriter : TypeWriter<InterfaceDeclarationSyntax>
    {
        public InterfaceTypeWriter(InterfaceDeclarationSyntax declaration,
                ICompilationContextProvider context)
            : base(declaration, context) { }

        public InterfaceTypeWriter(IReadOnlyList<InterfaceDeclarationSyntax> childDeclarations,
                ICompilationContextProvider context)
            : base(childDeclarations, context) { }

        protected override void WriteTypeParameters()
        {
            Builder.Append(Context.GetTypeParameters(), this).Space();
        }

        public override int Arity
        {
            get { return Context.Arity; }
        }
    }
}
