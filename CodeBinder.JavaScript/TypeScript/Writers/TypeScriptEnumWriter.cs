// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptEnumWriter : TypeScriptBaseTypeWriter<TypeScriptEnumContext>
{
    public TypeScriptEnumWriter(TypeScriptEnumContext enm)
        : base(enm)
    {
    }

    protected override void WriteMembers()
    {
        foreach (var member in Item.Node.Members)
        {
            long value = member.GetEnumValue(Compilation);
            Builder.Append(member.Identifier.Text).Space().Append("=").Space().Append(value.ToString()).Comma().AppendLine();
        }
    }

    public override string TypeDeclaration => "const enum";
}
