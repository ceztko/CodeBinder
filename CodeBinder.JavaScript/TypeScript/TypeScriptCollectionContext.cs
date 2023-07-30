// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptCollectionContext : CSharpCollectionContext<TypeScriptCompilationContext>
{
    public TypeScriptCollectionContext(TypeScriptCompilationContext context) : base(context)
    {
    }


}
