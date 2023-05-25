// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptNodeVisitor : CSharpNodeVisitor<TypeScriptCompilationContext, CSharpMemberTypeContext, ConversionCSharpToTypeScript>
{
    public TypeScriptNodeVisitor(TypeScriptCompilationContext context) : base(context)
    {
    }

    protected override string GetMethodBaseName(IMethodSymbol symbol)
    {
        if (symbol.MethodKind == MethodKind.Constructor)
            return "constructor";
        else
            return base.GetMethodBaseName(symbol);
    }
}
