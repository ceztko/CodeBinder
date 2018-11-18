using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared.Java;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Java
{
    static partial class JavaWriterExtension
    {
        public static CodeBuilder Append(this CodeBuilder builder, ConstructorDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new ConstructorWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, DestructorDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new DestructorWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, MethodDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new MethodWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, PropertyDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new PropertyWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, IndexerDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new IndexerWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, FieldDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new FieldWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, InterfaceDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new InterfaceTypeWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ClassDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new ClassTypeWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, StructDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new StructTypeWriter(member, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, EnumDeclarationSyntax member, ICompilationContextProvider context)
        {
            return builder.Append(new EnumTypeWriter(member, context));
        }

        public static IEnumerable<CodeWriter> GetWriters(this MemberDeclarationSyntax member, ICompilationContextProvider context)
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
                    return new[] { new InterfaceTypeWriter(member as InterfaceDeclarationSyntax, context) };
                case SyntaxKind.ClassDeclaration:
                    return new[] { new ClassTypeWriter(member as ClassDeclarationSyntax, context) };
                case SyntaxKind.StructKeyword:
                    return new[] { new StructTypeWriter(member as StructDeclarationSyntax, context) };
                case SyntaxKind.EnumDeclaration:
                    return new[] { new EnumTypeWriter(member as EnumDeclarationSyntax, context) };
                default:
                    throw new Exception();
            }
        }

        static IEnumerable<CodeWriter> getMethodWriters(MethodDeclarationSyntax method, ICompilationContextProvider context)
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
