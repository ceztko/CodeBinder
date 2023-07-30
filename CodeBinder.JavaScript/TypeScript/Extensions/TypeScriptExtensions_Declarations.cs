// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

static partial class TypeScriptExtensions
{
    public static IEnumerable<CodeWriter> GetWriters(this MemberDeclarationSyntax member,
        PartialDeclarationsTree partialDeclarations, TypeScriptCompilationContext context)
    {
        var kind = member.Kind();
        switch (kind)
        {
            case SyntaxKind.ConstructorDeclaration:
                return getConstructorWriters((ConstructorDeclarationSyntax)member, context);
            case SyntaxKind.DestructorDeclaration:
                return new[] { new DestructorWriter((DestructorDeclarationSyntax)member, context) };
            case SyntaxKind.MethodDeclaration:
                return getMethodWriters((MethodDeclarationSyntax)member, context);
            case SyntaxKind.PropertyDeclaration:
                return new[] { new TypeScriptPropertyWriter((PropertyDeclarationSyntax)member, context) };
            case SyntaxKind.IndexerDeclaration:
                return new[] { new JavaIndexerWriter((IndexerDeclarationSyntax)member, context) };
            case SyntaxKind.FieldDeclaration:
                return new[] { new TypeScriptFieldWriter((FieldDeclarationSyntax)member, context) };
            case SyntaxKind.InterfaceDeclaration:
                var iface = (InterfaceDeclarationSyntax)member;
                return new[] { new TypeScriptInterfaceWriter(context.Interfaces[iface], partialDeclarations.ChildrenPartialDeclarations[iface]) };
            case SyntaxKind.ClassDeclaration:
                var cls = (ClassDeclarationSyntax)member;
                return new[] { new TypeScriptClassWriter(context.Classes[cls], partialDeclarations.ChildrenPartialDeclarations[cls]) };
            case SyntaxKind.StructDeclaration:
                var structure = (StructDeclarationSyntax)member;
                return new[] { new TypeScriptStructWriter(context.Structs[structure], partialDeclarations.ChildrenPartialDeclarations[structure]) };
            case SyntaxKind.EnumDeclaration:
                return new[] { new TypeScriptEnumWriter(context.Enums[(EnumDeclarationSyntax)member]) };
            default:
                throw new NotSupportedException();
        }
    }

    static IEnumerable<CodeWriter> getConstructorWriters(ConstructorDeclarationSyntax constructor, TypeScriptCompilationContext context)
    {
        yield return new ConstructorWriter(constructor, context);
        var methodSymbol = constructor.GetDeclaredSymbol<IMethodSymbol>(context);
        if (methodSymbol.ContainingType.TypeKind == TypeKind.Struct && methodSymbol.Parameters.Length != 0)
            yield return new StructConstructorWrapperWriter(constructor, context);
    }

    static IEnumerable<CodeWriter> getMethodWriters(MethodDeclarationSyntax method, TypeScriptCompilationContext context)
    {
        var methodSymbol = method.GetDeclaredSymbol<IMethodSymbol>(context);
        if (methodSymbol.IsPartialDefinition || methodSymbol.IsNative())
            yield break;

        IMethodSymbol symbol;
        if (method.Identifier.Text == "FreeHandle")
        {
            symbol = method.GetDeclaredSymbol<IMethodSymbol>(context);
            if (symbol.OverriddenMethod?.ContainingType.GetFullName() == "CodeBinder.HandledObjectBase")
            {
                yield return new CreateFinalizerMethodWriter(symbol.ContainingType);
                yield break;
            }
        }

        yield return new MethodWriter(method, context);
    }

    class CreateFinalizerMethodWriter : CodeWriter
    {
        ITypeSymbol _finalizableType;

        public CreateFinalizerMethodWriter(ITypeSymbol finalizableType)
        {
            _finalizableType = finalizableType;
        }

        protected override void Write()
        {
            Builder.AppendLine($$"""
protected override createFinalizer(): HandledObjectFinalizer | null
{
    return new {{_finalizableType.Name}}Finalizer();
}
""");
        }
    }
}
