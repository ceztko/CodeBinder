// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptFieldWriter : CodeWriter<FieldDeclarationSyntax, TypeScriptCompilationContext>
{
    public TypeScriptFieldWriter(FieldDeclarationSyntax syntax, TypeScriptCompilationContext context)
        : base(syntax, context) { }

    protected override void Write()
    {
        string modifiers = Item.GetModifiersString(Context);
        if (!modifiers.IsNullOrEmpty())
            Builder.Append(modifiers).Space();

        Builder.Append(Item.Declaration, Context).EndOfStatement();
    }
}
