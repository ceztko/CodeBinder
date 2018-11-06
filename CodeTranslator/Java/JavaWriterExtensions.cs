using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    static class JavaWriterExtension
    {
        public static ISyntaxWriter GetWriter(this MemberDeclarationSyntax member, ICompilationContextProvider context)
        {
            var kind = member.Kind();
            switch (kind)
            {
                case SyntaxKind.ConstructorDeclaration:
                    return new ConstructorWriter(member as ConstructorDeclarationSyntax, context);
                case SyntaxKind.DestructorDeclaration:
                    return new DestructorWriter(member as DestructorDeclarationSyntax, context);
                case SyntaxKind.MethodDeclaration:
                    return new MethodWriter(member as MethodDeclarationSyntax, context);
                case SyntaxKind.PropertyDeclaration:
                    return new PropertyWriter(member as PropertyDeclarationSyntax, context);
                case SyntaxKind.IndexerDeclaration:
                    return new IndexerWriter(member as IndexerDeclarationSyntax, context);
                case SyntaxKind.FieldDeclaration:
                    return new FieldWriter(member as FieldDeclarationSyntax, context);
                case SyntaxKind.InterfaceDeclaration:
                    return new InterfaceTypeWriter(member as InterfaceDeclarationSyntax, context);
                case SyntaxKind.ClassDeclaration:
                    return new ClassTypeWriter(member as ClassDeclarationSyntax, context);
                case SyntaxKind.StructKeyword:
                    return new StructTypeWriter(member as StructDeclarationSyntax, context);
                case SyntaxKind.EnumDeclaration:
                    return new EnumTypeWriter(member as EnumDeclarationSyntax, context);
                default:
                    return new NullSyntaxWriter();
            }
        }

        public static ISyntaxWriter GetWriter(this StatementSyntax statement, ICompilationContextProvider context)
        {
            var kind = statement.Kind();
            switch (kind)
            {
                case SyntaxKind.ExpressionStatement:
                    return new ExpressionStamentWriter(statement as ExpressionStatementSyntax, context);
                default:
                    return new NullSyntaxWriter();
            }
        }

        public static ISyntaxWriter GetWriter(this ExpressionSyntax expression, ICompilationContextProvider context)
        {
            var kind = expression.Kind();
            switch (kind)
            {
                case SyntaxKind.SimpleAssignmentExpression:
                    return new AssignmentExpressionWriter(expression as AssignmentExpressionSyntax, context);
                case SyntaxKind.IdentifierName:
                    return new IdenfitiferNameWriter(expression as IdentifierNameSyntax, context);
                default:
                    return new NullSyntaxWriter();
            }
        }
    }
}
