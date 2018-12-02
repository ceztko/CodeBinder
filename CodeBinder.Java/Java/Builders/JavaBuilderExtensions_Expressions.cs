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
using System.Diagnostics;

namespace CodeBinder.Java
{
    static partial class JavaBuilderExtension
    {
        public static CodeBuilder Append(this CodeBuilder builder, ArrayCreationExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append("new").Space().Append(syntax.Type, context);
            if (syntax.Initializer != null)
                builder.Append(syntax.Initializer, context);

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, OmittedArraySizeExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            // Do nothing
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, AssignmentExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            var symbol = syntax.Left.GetSymbol(context);
            // Symbol can be null https://github.com/dotnet/roslyn/issues/31471
            if (symbol?.Kind == SymbolKind.Property)
            {
                var operatorKind = syntax.OperatorToken.Kind();
                switch (operatorKind)
                {
                    case SyntaxKind.EqualsToken:
                    {
                        if (syntax.Left.Kind() == SyntaxKind.ElementAccessExpression)
                        {
                            var elementAccess = syntax.Left as ElementAccessExpressionSyntax;
                            builder.Append(elementAccess.Expression, context).Dot()
                                .Append(elementAccess, symbol as IPropertySymbol, context)
                                    .Parenthesized().Append(elementAccess.ArgumentList, false, context).CommaSeparator().Append(syntax.Right, context); ;
                            return builder;
                        }

                        builder.Append(syntax.Left, context).Parenthesized().Append(syntax.Right, context);
                        return builder;
                    }
                    default:
                        throw new Exception();
                }
            }

            builder.Append(syntax.Left, context).Space().Append(syntax.GetJavaOperator()).Space().Append(syntax.Right, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, BinaryExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            if (syntax.Kind() == SyntaxKind.AsExpression)
            {
                builder.Append("BinderUtils").Dot().Append("as").Parenthesized().Append(syntax.Left, context).CommaSeparator().Append(syntax.Right, context).Dot().Append("class");
                return builder;
            }

            builder.Append(syntax.Left, context).Space().Append(syntax.GetJavaOperator()).Space().Append(syntax.Right, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, CastExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Parenthesized().Append(syntax.Type, context).Close().Append(syntax.Expression, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ConditionalExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append(syntax.Condition, context).Space().QuestionMark().Space()
                .Append(syntax.WhenTrue, context).Space().Colon().Space()
                .Append(syntax.WhenFalse, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            var symbol = syntax.GetSymbol(context);
            if (symbol?.Kind == SymbolKind.Property)
            {
                var property = symbol as IPropertySymbol;
                Debug.Assert(property.IsIndexer);
                builder.Append(syntax, property, context).Parenthesized().Append(syntax.ArgumentList, false, context);
                return builder;
            }

            builder.Append(syntax.Expression, context).Append(syntax.ArgumentList, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, InitializerExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Braced().Append(syntax.Expressions, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, BaseExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append("super");
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThisExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append("this");
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, InvocationExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            var methodSymbol = syntax.GetSymbol<IMethodSymbol>(context);
            if (methodSymbol.ReturnType.TypeKind == TypeKind.Enum)
                builder.Append(methodSymbol.ReturnType.Name).Dot().Append("fromValue").Parenthesized(() => append(builder, syntax, context));
            else
                append(builder, syntax, context);

            return builder;
        }

        static void append(CodeBuilder builder, InvocationExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append(syntax.Expression, context).Append(syntax.ArgumentList, context);
        }

        public static CodeBuilder Append(this CodeBuilder builder, LiteralExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append(syntax.Token.Text);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, MemberAccessExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            if (builder.TryToReplace(syntax, context))
                return builder;

            var typeSymbol = syntax.Expression.GetTypeSymbol(context);
            if (typeSymbol.IsCLRPrimitiveType())
            {
                string javaBoxType = JavaUtils.GetJavaBoxType(typeSymbol.GetFullName());
                builder.Parenthesized().Parenthesized().Append(javaBoxType).Close().Append(syntax.Expression, context).Close().Dot().Append(syntax.Name, context);
                return builder;
            }

            var symbol = syntax.GetSymbol(context);
            if (symbol.Kind == SymbolKind.Property
                && symbol.OriginalDefinition.ContainingType.GetFullName() == "System.Nullable<T>"
                && symbol.Name == "Value")
            {
                // There are no nullable types in Java, just discard ".Value" accessor
                builder.Append(syntax.Expression, context);
                return builder;
            }

            builder.Append(syntax.Expression, context).Dot().Append(syntax.Name, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ObjectCreationExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append("new").Space().Append(syntax.Type, context).Append(syntax.ArgumentList, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ParenthesizedExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Parenthesized().Append(syntax.Expression, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, PostfixUnaryExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append(syntax.Operand, context).Append(syntax.GetJavaOperator());
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, PrefixUnaryExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append(syntax.GetJavaOperator()).Append(syntax.Operand, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeOfExpressionSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append(syntax.Type, context).Append(".class");
            return builder;
        }

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static CodeBuilder Append(this CodeBuilder builder, ExpressionSyntax expression, JavaCodeConversionContext context)
        {
            var kind = expression.Kind();
            switch (kind)
            {
                case SyntaxKind.ArrayCreationExpression:
                    return builder.Append(expression as ArrayCreationExpressionSyntax, context);
                case SyntaxKind.OmittedArraySizeExpression:
                    return builder.Append(expression as OmittedArraySizeExpressionSyntax, context);
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
                    return builder.Append(expression as AssignmentExpressionSyntax, context);
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
                    return builder.Append(expression as BinaryExpressionSyntax, context);
                case SyntaxKind.CastExpression:
                    return builder.Append(expression as CastExpressionSyntax, context);
                case SyntaxKind.ConditionalExpression:
                    return builder.Append(expression as ConditionalExpressionSyntax, context);
                case SyntaxKind.ElementAccessExpression:
                    return builder.Append(expression as ElementAccessExpressionSyntax, context);
                case SyntaxKind.ObjectInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.ComplexElementInitializerExpression:
                    return builder.Append(expression as InitializerExpressionSyntax, context);
                case SyntaxKind.BaseExpression:
                    return builder.Append(expression as BaseExpressionSyntax, context);
                case SyntaxKind.ThisExpression:
                    return builder.Append(expression as ThisExpressionSyntax, context);
                case SyntaxKind.InvocationExpression:
                    return builder.Append(expression as InvocationExpressionSyntax, context);
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                    return builder.Append(expression as LiteralExpressionSyntax, context);
                case SyntaxKind.SimpleMemberAccessExpression:
                    return builder.Append(expression as MemberAccessExpressionSyntax, context);
                case SyntaxKind.ObjectCreationExpression:
                    return builder.Append(expression as ObjectCreationExpressionSyntax, context);
                case SyntaxKind.ParenthesizedExpression:
                    return builder.Append(expression as ParenthesizedExpressionSyntax, context);
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                    return builder.Append(expression as PostfixUnaryExpressionSyntax, context);
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                    return builder.Append(expression as PrefixUnaryExpressionSyntax, context);
                case SyntaxKind.TypeOfExpression:
                    return builder.Append(expression as TypeOfExpressionSyntax, context);
                case SyntaxKind.QualifiedName:
                case SyntaxKind.ArrayType:
                case SyntaxKind.GenericName:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.NullableType:
                case SyntaxKind.OmittedTypeArgument:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.RefType:
                    return builder.Append(expression as TypeSyntax, context);
                // Unsupported expressions
                case SyntaxKind.RefExpression:
                case SyntaxKind.DeclarationExpression:
                case SyntaxKind.ThrowExpression:
                case SyntaxKind.DefaultExpression:
                case SyntaxKind.AnonymousMethodExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.RefValueExpression:
                case SyntaxKind.RefTypeExpression:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.ElementBindingExpression:
                case SyntaxKind.ImplicitElementAccess:
                case SyntaxKind.MemberBindingExpression:
                case SyntaxKind.SizeOfExpression:
                case SyntaxKind.MakeRefExpression:
                case SyntaxKind.ImplicitStackAllocArrayCreationExpression:
                case SyntaxKind.InterpolatedStringExpression:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.QueryExpression:
                case SyntaxKind.StackAllocArrayCreationExpression:
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.TupleExpression:
                case SyntaxKind.IsPatternExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.ConditionalAccessExpression:
                // Unsupported prefix unary expressions
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                // Unsupported binary expressions
                case SyntaxKind.CoalesceExpression:
                // Unsupported member access expressions
                case SyntaxKind.PointerMemberAccessExpression:
                // Unsupported literal expressions
                case SyntaxKind.ArgListExpression:
                case SyntaxKind.DefaultLiteralExpression:
                // Unsupported type expressions
                case SyntaxKind.AliasQualifiedName:
                case SyntaxKind.TupleType:
                case SyntaxKind.PointerType:
                default:
                    throw new Exception();
            }
        }

        public static CodeBuilder Append(this CodeBuilder builder, BracketedArgumentListSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Append(syntax, true, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, BracketedArgumentListSyntax syntax, bool bracketed, JavaCodeConversionContext context)
        {
            if (bracketed)
                builder.Bracketed().Append(syntax.Arguments, context);
            else
                builder.Append(syntax.Arguments, context);

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ArgumentListSyntax syntax, JavaCodeConversionContext context)
        {
            builder.Parenthesized().Append(syntax.Arguments, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, IEnumerable<ArgumentSyntax> arguments, JavaCodeConversionContext context)
        {
            bool first = true;
            foreach (var arg in arguments)
            {
                builder.CommaSeparator(ref first);

                // For ref/out variables
                if (!arg.RefKindKeyword.IsNone())
                    builder.Append("__");

                builder.Append(arg.Expression, context);
            }

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, IEnumerable<ExpressionSyntax> expressions, JavaCodeConversionContext context)
        {
            bool first = true;
            foreach (var expression in expressions)
                builder.CommaSeparator(ref first).Append(expression, context);

            return builder;
        }
    }
}
