// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptInterfaceWriter : TypeScriptTypeWriter<TypeScriptInterfaceContext>
{
    public TypeScriptInterfaceWriter(TypeScriptInterfaceContext iface, PartialDeclarationsTree partialDeclarations)
        : base(iface, partialDeclarations) { }

    public TypeScriptInterfaceWriter(TypeScriptInterfaceContext iface)
        : base(iface, iface.ComputePartialDeclarationsTree()) { }

    protected override void WriteTypeParameters()
    {
        Builder.Append(Item.Node.GetTypeParameters(), Compilation).Space();
    }

    public override int Arity
    {
        get { return Item.Node.Arity; }
    }

    public override string TypeDeclaration => "interface";
}
