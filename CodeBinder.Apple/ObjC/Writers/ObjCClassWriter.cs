// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Apple;

class ObjCClassWriter : ObjCTypeWriter<ClassDeclarationSyntax>
{
    public ObjCClassWriter(ClassDeclarationSyntax declaration, PartialDeclarationsTree partialDeclarations,
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
