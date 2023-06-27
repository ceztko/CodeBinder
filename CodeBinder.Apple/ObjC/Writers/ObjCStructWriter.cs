// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Shared.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeBinder.Apple;

class ObjCStructWriter : ObjCTypeWriter<StructDeclarationSyntax>
{
    public ObjCStructWriter(StructDeclarationSyntax declaration, PartialDeclarationsTree partialDeclarations,
        ObjCCompilationContext context, ObjCFileType fileType)
        : base(declaration, partialDeclarations, context, fileType) { }

    protected override void WriteTypeParameters()
    {
        Builder.Append(Item.GetTypeParameters(), Context).Space();
    }

    public override int Arity
    {
        get { return Item.Arity; }
    }
}
