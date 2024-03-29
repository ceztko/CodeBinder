﻿// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Java;

class JavaStructWriter : JavaTypeWriter<StructDeclarationSyntax>
{
    public JavaStructWriter(StructDeclarationSyntax declaration, PartialDeclarationsTree partialDeclarations,
            JavaCodeConversionContext context) : base(declaration, partialDeclarations, context) { }

    protected override void WriteTypeMembers()
    {
        // Add public default constructor
        Builder.Append("public").Space().Append(TypeName).EmptyParameterList().Space().EmptyBody().AppendLine();
        Builder.AppendLine();
        base.WriteTypeMembers();
    }

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
