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
        public static IContextWriter GetWriter(this ConstructorDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new ConstructorWriter(member, context);
        }

        public static IContextWriter GetWriter(this DestructorDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new DestructorWriter(member, context);
        }

        public static IContextWriter GetWriter(this MethodDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new MethodWriter(member, context);
        }

        public static IContextWriter GetWriter(this PropertyDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new PropertyWriter(member, context);
        }

        public static IContextWriter GetWriter(this IndexerDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new IndexerWriter(member, context);
        }

        public static IContextWriter GetWriter(this FieldDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new FieldWriter(member, context);
        }

        public static IContextWriter GetWriter(this InterfaceDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new InterfaceTypeWriter(member, context);
        }

        public static IContextWriter GetWriter(this ClassDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new ClassTypeWriter(member, context);
        }

        public static IContextWriter GetWriter(this StructDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new StructTypeWriter(member, context);
        }

        public static IContextWriter GetWriter(this EnumDeclarationSyntax member, ICompilationContextProvider context)
        {
            return new EnumTypeWriter(member, context);
        }

        public static IContextWriter GetWriter(this ArrayCreationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new ArrayCreationExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this OmittedArraySizeExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new OmittedArraySizeExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this AssignmentExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new AssignmentExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this BinaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new BinaryExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this CastExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new CastExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this ConditionalExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new ConditionalExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this DeclarationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new DeclarationExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this ElementAccessExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new ElementAccessExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this InitializerExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new InitializerExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this BaseExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new BaseExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this ThisExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new ThisExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this InvocationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new InvocationExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this LiteralExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new LiteralExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this MemberAccessExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new MemberAccessExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this ObjectCreationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new ObjectCreationExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this ParenthesizedExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new ParenthesizedExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this PostfixUnaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new PostfixUnaryExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this PrefixUnaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new PrefixUnaryExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this RefExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new RefExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this ThrowExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new ThrowExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this TypeOfExpressionSyntax expression, ICompilationContextProvider context)
        {
            return new TypeOfExpressionWriter(expression, context);
        }

        public static IContextWriter GetWriter(this ArrayTypeSyntax expression, ICompilationContextProvider context)
        {
            return new ArrayTypeWriter(expression, context);
        }

        public static IContextWriter GetWriter(this QualifiedNameSyntax expression, ICompilationContextProvider context)
        {
            return new QualifiedNameWriter(expression, context);
        }

        public static IContextWriter GetWriter(this GenericNameSyntax expression, ICompilationContextProvider context)
        {
            return new GenericNameWriter(expression, context);
        }

        public static IContextWriter GetWriter(this IdentifierNameSyntax expression, ICompilationContextProvider context)
        {
            return new IdenfitiferNameWriter(expression, context);
        }

        public static IContextWriter GetWriter(this NullableTypeSyntax expression, ICompilationContextProvider context)
        {
            return new NullableTypeWriter(expression, context);
        }

        public static IContextWriter GetWriter(this OmittedTypeArgumentSyntax expression, ICompilationContextProvider context)
        {
            return new OmittedTypeArgumentWriter(expression, context);
        }

        public static IContextWriter GetWriter(this PredefinedTypeSyntax expression, ICompilationContextProvider context)
        {
            return new PredefinedTypeWriter(expression, context);
        }

        public static IContextWriter GetWriter(this RefTypeSyntax expression, ICompilationContextProvider context)
        {
            return new RefTypeWriter(expression, context);
        }

        public static IContextWriter GetWriter(this BlockSyntax statement, ICompilationContextProvider context)
        {
            return new BlockStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this BreakStatementSyntax statement, ICompilationContextProvider context)
        {
            return new BreakStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this ForEachStatementSyntax statement, ICompilationContextProvider context)
        {
            return new ForEachStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this ForEachVariableStatementSyntax statement, ICompilationContextProvider context)
        {
            return new ForEachVariableStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this ContinueStatementSyntax statement, ICompilationContextProvider context)
        {
            return new ContinueStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this DoStatementSyntax statement, ICompilationContextProvider context)
        {
            return new DoStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this EmptyStatementSyntax statement, ICompilationContextProvider context)
        {
            return new EmptyStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this ExpressionStatementSyntax statement, ICompilationContextProvider context)
        {
            return new ExpressionStamentWriter(statement, context);
        }

        public static IContextWriter GetWriter(this ForStatementSyntax statement, ICompilationContextProvider context)
        {
            return new ForStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this IfStatementSyntax statement, ICompilationContextProvider context)
        {
            return new IfStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this LocalDeclarationStatementSyntax statement, ICompilationContextProvider context)
        {
            return new LocalDeclarationStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this LockStatementSyntax statement, ICompilationContextProvider context)
        {
            return new LockStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this ReturnStatementSyntax statement, ICompilationContextProvider context)
        {
            return new ReturnStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this SwitchStatementSyntax statement, ICompilationContextProvider context)
        {
            return new SwitchStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this ThrowStatementSyntax statement, ICompilationContextProvider context)
        {
            return new ThrowStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this TryStatementSyntax statement, ICompilationContextProvider context)
        {
            return new TryStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this UsingStatementSyntax statement, ICompilationContextProvider context)
        {
            return new UsingStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this WhileStatementSyntax statement, ICompilationContextProvider context)
        {
            return new WhileStatementWriter(statement, context);
        }

        public static IContextWriter GetWriter(this YieldStatementSyntax statement, ICompilationContextProvider context)
        {
            return new YieldStatementWriter(statement, context);
        }

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
                    throw new Exception();
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
                case SyntaxKind.CastExpression:
                    return new CastExpressionWriter(expression as CastExpressionSyntax, context);
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
