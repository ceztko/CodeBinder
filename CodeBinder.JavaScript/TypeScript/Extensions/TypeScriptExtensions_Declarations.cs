// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using System.Linq;

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
                return new[] { new ConstructorWriter((ConstructorDeclarationSyntax)member, context) };
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

    static IEnumerable<CodeWriter> getMethodWriters(MethodDeclarationSyntax method, TypeScriptCompilationContext context)
    {
        var methodSymbol = method.GetDeclaredSymbol<IMethodSymbol>(context);
        if (methodSymbol.IsPartialDefinition || methodSymbol.IsExtern)
            yield break;

        yield return new MethodWriter(method, context);
    }
}
