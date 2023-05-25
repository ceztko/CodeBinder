// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.JavaScript.NAPI;

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptStructWriter : TypeScriptTypeWriter<TypeScriptStructContext>
{
    public TypeScriptStructWriter(TypeScriptStructContext strct, PartialDeclarationsTree partialDeclarations)
        : base(strct, partialDeclarations) { }

    public TypeScriptStructWriter(TypeScriptStructContext strct)
        : base(strct, strct.ComputePartialDeclarationsTree()) { }

    protected override void WriteMembers()
    {
        // Add public default constructor
        Builder.Append("constructor").EmptyParameterList().AppendLine();
        using (Builder.Block())
        {
            Builder.Append("super()").EndOfStatement();
        }
        Builder.AppendLine();
        base.WriteMembers();
    }

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
}
