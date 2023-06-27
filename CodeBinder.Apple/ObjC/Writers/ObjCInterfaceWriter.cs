// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Apple;

class ObjCInterfaceWriter : ObjCTypeWriter<InterfaceDeclarationSyntax>
{
    public ObjCInterfaceWriter(InterfaceDeclarationSyntax declaration, PartialDeclarationsTree partialDeclarations,
            ObjCCompilationContext context, ObjCFileType fileType)
        : base(declaration, partialDeclarations, context, fileType) { }


    protected override void WriteBaseTypes()
    {
        /* Do nothing */
        // TODO: Support base interfaces
        if (Item.BaseList != null)
            throw new NotImplementedException("Missing base interfaces support");
    }

    protected override void WriteTypeParameters()
    {
        Builder.Append(Item.GetTypeParameters(), Context).Space();
    }

    public override int Arity
    {
        get { return Item.Arity; }
    }
}
