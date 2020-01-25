using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CodeBinder.Shared.Java;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Java
{
    static partial class JavaExtensions
    {
        public static IEnumerable<CodeWriter> GetWriters(this MemberDeclarationSyntax member,
            PartialDeclarationsTree partialDeclarations, JavaCodeConversionContext context)
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
                    return new[] { new PropertyWriter((PropertyDeclarationSyntax)member, context) };
                case SyntaxKind.IndexerDeclaration:
                    return new[] { new IndexerWriter((IndexerDeclarationSyntax)member, context) };
                case SyntaxKind.FieldDeclaration:
                    return new[] { new FieldWriter((FieldDeclarationSyntax)member, context) };
                case SyntaxKind.InterfaceDeclaration:
                    var iface = (InterfaceDeclarationSyntax)member;
                    return new[] { new InterfaceTypeWriter(iface, partialDeclarations.MemberPartialDeclarations[iface], context) };
                case SyntaxKind.ClassDeclaration:
                    var cls = (ClassDeclarationSyntax)member;
                    return new[] { new ClassTypeWriter(cls, partialDeclarations.MemberPartialDeclarations[cls], context) };
                case SyntaxKind.StructKeyword:
                    var structure = (StructDeclarationSyntax)member;
                    return new[] { new StructTypeWriter(structure, partialDeclarations.MemberPartialDeclarations[structure], context) };
                case SyntaxKind.EnumDeclaration:
                    return new[] { new EnumTypeWriter((EnumDeclarationSyntax)member, context) };
                default:
                    throw new Exception();
            }
        }

        static IEnumerable<CodeWriter> getMethodWriters(MethodDeclarationSyntax method, JavaCodeConversionContext context)
        {
            if (method.GetCSharpModifiers().Contains(SyntaxKind.PartialKeyword) && method.Body == null)
                yield break;

            for (int i = method.ParameterList.Parameters.Count - 1; i >= 0; i--)
            {
                var parameter = method.ParameterList.Parameters[i];
                if (parameter.Default == null)
                    break;

                yield return new MethodWriter(method, i, context);
            }

            yield return new MethodWriter(method, -1, context);
        }
    }
}
