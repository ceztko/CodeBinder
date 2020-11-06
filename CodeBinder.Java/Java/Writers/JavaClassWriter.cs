// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared.CSharp;
using CodeBinder.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    class JavaClassWriter : JavaTypeWriter<ClassDeclarationSyntax>
    {
        public JavaClassWriter(ClassDeclarationSyntax declaration, PartialDeclarationsTree partialDeclarations,
                JavaCodeConversionContext context) : base(declaration, partialDeclarations, context) { }

        protected override void WriteTypeParameters()
        {
            Builder.Append(Item.GetTypeParameters(), Context).Space();
        }

        public override int Arity
        {
            get { return Item.Arity; }
        }

        public override bool NeedStaticKeyword
        {
            get { return true; }
        }
    }
}
