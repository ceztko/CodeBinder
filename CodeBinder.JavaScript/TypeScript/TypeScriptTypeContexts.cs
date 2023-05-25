// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

public class TypeScriptClassContext : CSharpClassTypeContext<TypeScriptCompilationContext>
{
    public TypeScriptClassContext(ClassDeclarationSyntax node, TypeScriptCompilationContext compilation)
        : base(node, compilation)
    {
    }
}

public class TypeScriptStructContext : CSharpStructTypeContext<TypeScriptCompilationContext>
{
    public TypeScriptStructContext(StructDeclarationSyntax node, TypeScriptCompilationContext compilation)
        : base(node, compilation)
    {
    }
}

public class TypeScriptInterfaceContext : CSharpInterfaceTypeContext<TypeScriptCompilationContext>
{
    public TypeScriptInterfaceContext(InterfaceDeclarationSyntax node, TypeScriptCompilationContext compilation)
        : base(node, compilation)
    {
    }
}

public class TypeScriptEnumContext : CSharpEnumTypeContext<TypeScriptCompilationContext>
{
    public TypeScriptEnumContext(EnumDeclarationSyntax node, TypeScriptCompilationContext compilation)
        : base(node, compilation)
    {
    }
}

public class TypeScriptDelegateContext : CSharpDelegateTypeContext<TypeScriptCompilationContext>
{
    public TypeScriptDelegateContext(DelegateDeclarationSyntax node, TypeScriptCompilationContext compilation)
        : base(node, compilation)
    {
    }
}
