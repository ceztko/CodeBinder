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

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static IContextWriter GetWriter(this ExpressionSyntax expression, ICompilationContextProvider context)
        {
            var kind = expression.Kind();
            switch (kind)
            {
                case SyntaxKind.ArrayCreationExpression:
                    return new ArrayCreationExpressionWriter(expression as ArrayCreationExpressionSyntax, context);
                case SyntaxKind.OmittedArraySizeExpression:
                    return new OmittedArraySizeExpressionWriter(expression as OmittedArraySizeExpressionSyntax, context);
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    return new AssignmentExpressionWriter(expression as AssignmentExpressionSyntax, context);
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.CoalesceExpression:
                    return new BinaryExpressionWriter(expression as BinaryExpressionSyntax, context);
                case SyntaxKind.ConditionalExpression:
                    return new ConditionalExpressionWriter(expression as ConditionalExpressionSyntax, context);
                case SyntaxKind.DeclarationExpression:
                    return new DeclarationExpressionWriter(expression as DeclarationExpressionSyntax, context);
                case SyntaxKind.ElementAccessExpression:
                    return new ElementAccessExpressionWriter(expression as ElementAccessExpressionSyntax, context);
                case SyntaxKind.ObjectInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.ComplexElementInitializerExpression:
                    return new InitializerExpressionWriter(expression as InitializerExpressionSyntax, context);
                case SyntaxKind.BaseExpression:
                    return new BaseExpressionWriter(expression as BaseExpressionSyntax, context);
                case SyntaxKind.ThisExpression:
                    return new ThisExpressionWriter(expression as ThisExpressionSyntax, context);
                case SyntaxKind.InvocationExpression:
                    return new InvocationExpressionWriter(expression as InvocationExpressionSyntax, context);
                case SyntaxKind.ArgListExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.DefaultLiteralExpression:
                    return new LiteralExpressionWriter(expression as LiteralExpressionSyntax, context);
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.PointerMemberAccessExpression:
                    return new MemberAccessExpressionWriter(expression as MemberAccessExpressionSyntax, context);
                case SyntaxKind.ObjectCreationExpression:
                    return new ObjectCreationExpressionWriter(expression as ObjectCreationExpressionSyntax, context);
                case SyntaxKind.ParenthesizedExpression:
                    return new ParenthesizedExpressionWriter(expression as ParenthesizedExpressionSyntax, context);
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                    return new PostfixUnaryExpressionWriter(expression as PostfixUnaryExpressionSyntax, context);
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                    return new PrefixUnaryExpressionWriter(expression as PrefixUnaryExpressionSyntax, context);
                case SyntaxKind.RefExpression:
                    return new RefExpressionWriter(expression as RefExpressionSyntax, context);
                case SyntaxKind.ThrowExpression:
                    return new ThrowExpressionWriter(expression as ThrowExpressionSyntax, context);
                case SyntaxKind.TypeOfExpression:
                    return new TypeOfExpressionWriter(expression as TypeOfExpressionSyntax, context);
                case SyntaxKind.ArrayType:
                    return new ArrayTypeWriter(expression as ArrayTypeSyntax, context);
                case SyntaxKind.QualifiedName:
                    return new QualifiedNameWriter(expression as QualifiedNameSyntax, context);
                case SyntaxKind.GenericName:
                    return new GenericNameWriter(expression as GenericNameSyntax, context);
                case SyntaxKind.IdentifierName:
                    return new IdenfitiferNameWriter(expression as IdentifierNameSyntax, context);
                case SyntaxKind.NullableType:
                    return new NullableTypeWriter(expression as NullableTypeSyntax, context);
                case SyntaxKind.OmittedTypeArgument:
                    return new OmittedTypeArgumentWriter(expression as OmittedTypeArgumentSyntax, context);
                case SyntaxKind.PredefinedType:
                    return new PredefinedTypeWriter(expression as PredefinedTypeSyntax, context);
                case SyntaxKind.RefTypeExpression:
                    return new RefTypeWriter(expression as RefTypeSyntax, context);
                default:
                    throw new Exception();
            }
        }

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
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
