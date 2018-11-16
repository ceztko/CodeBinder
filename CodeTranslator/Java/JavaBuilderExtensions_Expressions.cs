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
        public static CodeBuilder Append(this CodeBuilder builder, ArrayCreationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ArrayCreationExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, OmittedArraySizeExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new OmittedArraySizeExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, AssignmentExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new AssignmentExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, BinaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new BinaryExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, CastExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new CastExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ConditionalExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ConditionalExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, DeclarationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new DeclarationExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ElementAccessExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, InitializerExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new InitializerExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, BaseExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new BaseExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThisExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ThisExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, InvocationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new InvocationExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, LiteralExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new LiteralExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, MemberAccessExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new MemberAccessExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ObjectCreationExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ObjectCreationExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ParenthesizedExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ParenthesizedExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, PostfixUnaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new PostfixUnaryExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, PrefixUnaryExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new PrefixUnaryExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, RefExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new RefExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThrowExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ThrowExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeOfExpressionSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new TypeOfExpressionWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ArrayTypeSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new ArrayTypeWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, QualifiedNameSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new QualifiedNameWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, GenericNameSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new GenericNameWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, IdentifierNameSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new IdentifierNameWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, NullableTypeSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new NullableTypeWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, OmittedTypeArgumentSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new OmittedTypeArgumentWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, PredefinedTypeSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new PredefinedTypeWriter(expression, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, RefTypeSyntax expression, ICompilationContextProvider context)
        {
            return builder.Append(new RefTypeWriter(expression, context));
        }

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static CodeWriter GetWriter(this ExpressionSyntax expression, ICompilationContextProvider context)
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
                    return new IdentifierNameWriter(expression as IdentifierNameSyntax, context);
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
    }
}
