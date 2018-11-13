using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
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
        public static IEnumerable<IContextWriter> GetWriters(this MemberDeclarationSyntax member, ICompilationContextProvider context)
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
                    return new[] { new NullContextWriter() };
            }
        }

        public static IContextWriter GetWriter(this ExpressionSyntax expression, ICompilationContextProvider context)
        {
            var kind = expression.Kind();
            switch (kind)
            {
                case SyntaxKind.SimpleAssignmentExpression:
                    return new AssignmentExpressionWriter(expression as AssignmentExpressionSyntax, context);
                case SyntaxKind.IdentifierName:
                    return new IdenfitiferNameWriter(expression as IdentifierNameSyntax, context);
                default:
                    return new NullContextWriter();
            }
        }

        public static IContextWriter GetWriter(this StatementSyntax statement, ICompilationContextProvider context)
        {
            var kind = statement.Kind();
            switch (kind)
            {
                case SyntaxKind.Block:
                    return new BlockStatementWriter(statement as BlockSyntax, context);
                case SyntaxKind.BreakStatement:
                    return new BreakStatementWriter(statement as BreakStatementSyntax, context);
                case SyntaxKind.ForEachStatement:
                    return new ForEachStatementWriter(statement as ForEachStatementSyntax, context);
                case SyntaxKind.ForEachVariableStatement:
                    return new ForEachVariableStatementWriter(statement as ForEachVariableStatementSyntax, context);
                case SyntaxKind.ContinueStatement:
                    return new ContinueStatementWriter(statement as ContinueStatementSyntax, context);
                case SyntaxKind.DoStatement:
                    return new DoStatementWriter(statement as DoStatementSyntax, context);
                case SyntaxKind.EmptyStatement:
                    return new EmptyStatementWriter(statement as EmptyStatementSyntax, context);
                case SyntaxKind.ExpressionStatement:
                    return new ExpressionStamentWriter(statement as ExpressionStatementSyntax, context);
                case SyntaxKind.ForStatement:
                    return new ForStatementWriter(statement as ForStatementSyntax, context);
                case SyntaxKind.IfStatement:
                    return new IfStatementWriter(statement as IfStatementSyntax, context);
                case SyntaxKind.LocalDeclarationStatement:
                    return new LocalDeclarationStatementWriter(statement as LocalDeclarationStatementSyntax, context);
                case SyntaxKind.LockStatement:
                    return new LockStatementWriter(statement as LockStatementSyntax, context);
                case SyntaxKind.ReturnStatement:
                    return new ReturnStatementWriter(statement as ReturnStatementSyntax, context);
                case SyntaxKind.SwitchStatement:
                    return new SwitchStatementWriter(statement as SwitchStatementSyntax, context);
                case SyntaxKind.ThrowStatement:
                    return new ThrowStatementWriter(statement as ThrowStatementSyntax, context);
                case SyntaxKind.TryStatement:
                    return new TryStatementWriter(statement as TryStatementSyntax, context);
                case SyntaxKind.UsingStatement:
                    return new UsingStatementWriter(statement as UsingStatementSyntax, context);
                case SyntaxKind.WhileStatement:
                    return new WhileStatementWriter(statement as WhileStatementSyntax, context);
                case SyntaxKind.YieldReturnStatement:
                    return new YieldStatementWriter(statement as YieldStatementSyntax, context);
                default:
                    throw new Exception();
            }
        }

        static IEnumerable<IContextWriter> getMethodWriters(MethodDeclarationSyntax method, ICompilationContextProvider context)
        {
            var signatures = method.GetMethodSignatures(context);
            if (signatures.Length == 0)
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
