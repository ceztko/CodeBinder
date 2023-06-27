// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Java;

class JavaInterfaceWriter : JavaTypeWriter<InterfaceDeclarationSyntax>
{
    public JavaInterfaceWriter(InterfaceDeclarationSyntax declaration, PartialDeclarationsTree partialDeclarations,
            JavaCodeConversionContext context) : base(declaration, partialDeclarations, context) { }

    protected override void WriteTypeParameters()
    {
        Builder.Append(Item.GetTypeParameters(), Context).Space();
    }

    public override int Arity
    {
        get { return Item.Arity; }
    }
}
