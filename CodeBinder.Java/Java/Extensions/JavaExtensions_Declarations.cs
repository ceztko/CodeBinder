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
                    return new[] { new ConstructorWriter(member as ConstructorDeclarationSyntax, context) };
                case SyntaxKind.DestructorDeclaration:
                    return new[] { new DestructorWriter(member as DestructorDeclarationSyntax, context) };
                case SyntaxKind.MethodDeclaration:
                    return getMethodWriters(member as MethodDeclarationSyntax, context);
                case SyntaxKind.PropertyDeclaration:
                    return new[] { new PropertyWriter(member as PropertyDeclarationSyntax, context) };
                case SyntaxKind.IndexerDeclaration:
                    return new[] { new IndexerWriter(member as IndexerDeclarationSyntax, context) };
                case SyntaxKind.FieldDeclaration:
                    return new[] { new FieldWriter(member as FieldDeclarationSyntax, context) };
                case SyntaxKind.InterfaceDeclaration:
                    var iface = member as InterfaceDeclarationSyntax;
                    return new[] { new InterfaceTypeWriter(iface, partialDeclarations.MemberPartialDeclarations[iface], context) };
                case SyntaxKind.ClassDeclaration:
                    var cls = member as ClassDeclarationSyntax;
                    return new[] { new ClassTypeWriter(cls, partialDeclarations.MemberPartialDeclarations[cls], context) };
                case SyntaxKind.StructKeyword:
                    var structure = member as StructDeclarationSyntax;
                    return new[] { new StructTypeWriter(structure, partialDeclarations.MemberPartialDeclarations[structure], context) };
                case SyntaxKind.EnumDeclaration:
                    return new[] { new EnumTypeWriter(member as EnumDeclarationSyntax, context) };
                default:
                    throw new Exception();
            }
        }

        static IEnumerable<CodeWriter> getMethodWriters(MethodDeclarationSyntax method, JavaCodeConversionContext context)
        {
            var signatures = method.GetMethodSignatures(context);
            if (signatures.Count == 0)
            {
                yield return new MethodWriter(method, context);
            }
            else
            {
                foreach (var signature in signatures)
                    yield return new SignatureMethodWriter(signature, method, context);
            }
        }
    }
}
