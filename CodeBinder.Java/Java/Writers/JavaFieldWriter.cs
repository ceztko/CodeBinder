// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    class JavaFieldWriter : JavaCodeWriter<FieldDeclarationSyntax>
    {
        public JavaFieldWriter(FieldDeclarationSyntax syntax, JavaCodeConversionContext context)
            : base(syntax, context) { }

        protected override void Write()
        {
            string modifiers = Item.GetJavaModifiersString();
            if (!modifiers.IsNullOrEmpty())
                Builder.Append(modifiers).Space();

            Builder.Append(Item.Declaration, Context).EndOfStatement();
        }
    }
}
