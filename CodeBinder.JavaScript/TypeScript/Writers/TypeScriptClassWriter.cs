// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptClassWriter : TypeScriptTypeWriter<TypeScriptClassContext>
{
    public TypeScriptClassWriter(TypeScriptClassContext cls, PartialDeclarationsTree partialDeclarations)
        : base(cls, partialDeclarations) { }

    public TypeScriptClassWriter(TypeScriptClassContext cls)
        : base(cls, cls.ComputePartialDeclarationsTree()) { }

    protected override void WriteTypeParameters()
    {
        Builder.Append(Item.Node.GetTypeParameters(), Compilation).Space();
    }

    public override int Arity
    {
        get { return Item.Node.Arity; }
    }

    public override bool ClassType => true;

    public override string TypeDeclaration => "class";

    public override bool WriteAbstractModifier => Item.Symbol.IsAbstract;
}
