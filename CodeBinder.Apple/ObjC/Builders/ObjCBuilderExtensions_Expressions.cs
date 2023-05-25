// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using CodeBinder.CLang;

namespace CodeBinder.Apple
{
    static partial class ObjCBuilderExtension
    {

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static CodeBuilder Append(this CodeBuilder builder, ExpressionSyntax expression, ObjCCompilationContext context)
        {

            void AppendExpression(CodeBuilder builder)
            {
                var kind = expression.Kind();
                switch (kind)
                {
                    case SyntaxKind.ArrayCreationExpression:
                        builder.Append((ArrayCreationExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.OmittedArraySizeExpression:
                        builder.Append((OmittedArraySizeExpressionSyntax)expression, context);
                        break;
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
                        builder.Append((AssignmentExpressionSyntax)expression, context);
                        break;
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
                        builder.Append((BinaryExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.CastExpression:
                        builder.Append((CastExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.ConditionalExpression:
                        builder.Append((ConditionalExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.ElementAccessExpression:
                        builder.Append((ElementAccessExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.ObjectInitializerExpression:
                    case SyntaxKind.CollectionInitializerExpression:
                    case SyntaxKind.ArrayInitializerExpression:
                    case SyntaxKind.ComplexElementInitializerExpression:
                        builder.Append((InitializerExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.BaseExpression:
                        builder.Append((BaseExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.ThisExpression:
                        builder.Append((ThisExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.InvocationExpression:
                        builder.Append((InvocationExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.NumericLiteralExpression:
                    case SyntaxKind.StringLiteralExpression:
                    case SyntaxKind.CharacterLiteralExpression:
                    case SyntaxKind.TrueLiteralExpression:
                    case SyntaxKind.FalseLiteralExpression:
                    case SyntaxKind.NullLiteralExpression:
                        builder.Append((LiteralExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.SimpleMemberAccessExpression:
                        builder.Append((MemberAccessExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.ObjectCreationExpression:
                        builder.Append((ObjectCreationExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.ParenthesizedExpression:
                        builder.Append((ParenthesizedExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.PostIncrementExpression:
                    case SyntaxKind.PostDecrementExpression:
                        builder.Append((PostfixUnaryExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.UnaryPlusExpression:
                    case SyntaxKind.UnaryMinusExpression:
                    case SyntaxKind.BitwiseNotExpression:
                    case SyntaxKind.LogicalNotExpression:
                    case SyntaxKind.PreIncrementExpression:
                    case SyntaxKind.PreDecrementExpression:
                        builder.Append((PrefixUnaryExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.TypeOfExpression:
                        builder.Append((TypeOfExpressionSyntax)expression, context);
                        break;
                    case SyntaxKind.QualifiedName:
                    case SyntaxKind.ArrayType:
                    case SyntaxKind.GenericName:
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.NullableType:
                    case SyntaxKind.OmittedTypeArgument:
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.RefType:
                        builder.Append((TypeSyntax)expression, context);
                        break;
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
                        throw new NotSupportedException("Unsupported syntax");
                }
            }

            var typeInfo = expression.GetTypeInfo(context);
            if (typeInfo.ConvertedType?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
                && typeInfo.Type != null && typeInfo.Type.IsObjCPrimitiveType()
                && !SymbolEqualityComparer.Default.Equals(typeInfo.Type, typeInfo.ConvertedType))
            {
                // Handle primitive value types passed to nullable
                builder.Bracketed().Append("NSNumber").Space().Append(typeInfo.Type.GetNSNumberInitMethod()).Colon().Append(AppendExpression).Close();
            }
            else
            {
                builder.Append(AppendExpression);
            }

            return builder;

        }

        #region Expressions

        public static CodeBuilder Append(this CodeBuilder builder, ArrayCreationExpressionSyntax syntax, ObjCCompilationContext context)
        {

            if (syntax.Initializer == null)
            {
                builder.Bracketed().Bracketed().Append(syntax.Type, context).Space().Append("alloc").Close()
                    .Append("init").Colon().Append(syntax.Type.RankSpecifiers[0].Sizes[0], context).Close();
            }
            else
            {
                // The array initializers requires C variadic syntax
                builder.Bracketed().Bracketed().Append(syntax.Type, context).Space().Append("alloc").Close()
                    .Append("initWithValues").Colon().Append(syntax.Initializer.Expressions.Count.ToString())
                    .CommaSeparator().Append(syntax.Initializer, context).Close();
            }

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, OmittedArraySizeExpressionSyntax syntax, ObjCCompilationContext context)
        {
            // Do nothing
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, AssignmentExpressionSyntax syntax, ObjCCompilationContext context)
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
                                // Determine if the LHS of the assignment is an indexer set operation
                                var property = (IPropertySymbol)symbol;
                                if (!property.IsIndexer)
                                    break;

                                var elementAccess = (ElementAccessExpressionSyntax)syntax.Left;
                                builder.Bracketed().Append(elementAccess.Expression, context).Space() // [expr set :i :value]
                                    .Append(elementAccess, property, context).Space().Append(elementAccess.ArgumentList, false, context).Space()
                                    .Colon().Append(syntax.Right, context).Close();
                                return builder;
                            }

                            break;
                        }
                    default:
                        throw new Exception();
                }
            }

            builder.Append(syntax.Left, context).Space().Append(syntax.GetObjCOperator()).Space().Append(syntax.Right, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, BinaryExpressionSyntax syntax, ObjCCompilationContext context)
        {
            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.IsExpression:
                {
                    builder.Append("CBIsInstanceOf").AngleBracketed().Append(syntax.Right, context).Close().Parenthesized().Append(syntax.Left, context).Close();
                    return builder;
                }
                case SyntaxKind.AsExpression:
                {
                    builder.Append("CBAsOperator").AngleBracketed().Append(syntax.Right, context).Close().Parenthesized().Append(syntax.Left, context).Close();
                    return builder;
                }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                {
                    IMethodSymbol? method;
                    if (syntax.TryGetSymbol(context, out method))
                    {
                        SymbolReplacement? replacement;
                        if (method.HasObjCReplacement(out replacement))
                        {
                            switch (replacement!.Kind)
                            {
                                case SymbolReplacementKind.StaticMethod:
                                {
                                    builder.Append(replacement.Name).Parenthesized().Append(syntax.Left, context).CommaSeparator().Append(syntax.Right, context);
                                    return builder;
                                }
                                default:
                                {
                                    throw new NotImplementedException();
                                }
                            }
                        }
                    }
                    break;
                }
                case SyntaxKind.AddExpression:
                {
                    if (syntax.Left.GetTypeSymbol(context)!.SpecialType == SpecialType.System_String)
                    {
                        void AppendRhsExpression(CodeBuilder builder)
                        {
                            // We need the raw type of the rhs expression otherwise it gets boxed to System.Object
                            var rhsTypeSymbol = syntax.Right.GetTypeSymbolRaw(context)!;
                            if (rhsTypeSymbol.IsObjCPrimitiveType() || rhsTypeSymbol.TypeKind == TypeKind.Enum)
                                builder.Append("CBToString").Parenthesized().Append(syntax.Right, context).Close();
                            else
                                builder.Append(syntax.Right, context).Close();
                        }

                        builder.Append("CBStringAdd").Parenthesized().Append(syntax.Left, context).CommaSeparator()
                            .Append(AppendRhsExpression).Close();

                        return builder;
                    }
                    break;
                }
                default:
                {
                    break;
                }
            }

            builder.Append(syntax.Left, context).Space().Append(syntax.GetObjCOperator()).Space().Append(syntax.Right, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, CastExpressionSyntax syntax, ObjCCompilationContext context)
        {
            var type = syntax.Type.GetTypeSymbol(context);
            if (type.TryGetObjCPrimitiveType(out var objcName))
                builder.Parenthesized().Append(objcName).Close().Append(syntax.Expression, context);
            else
                builder.Append("CBCastOperator").AngleBracketed().Append(syntax.Type, context).Close().Parenthesized().Append(syntax.Expression, context).Close();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ConditionalExpressionSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append(syntax.Condition, context).Space().QuestionMark().Space()
                .Append(syntax.WhenTrue, context).Space().Colon().Space()
                .Append(syntax.WhenFalse, context);
            return builder;
        }

        // Expressions like new double[] { 2, 3, 4 }
        // NOTE: CBxxxxArray(s) initiatialization requires comma separated syntax invocation
        public static CodeBuilder Append(this CodeBuilder builder, InitializerExpressionSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append(syntax.Expressions, true, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, BaseExpressionSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("super");
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThisExpressionSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("self");
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, InvocationExpressionSyntax syntax, ObjCCompilationContext context)
        {
            var methodSymbol = syntax.GetSymbol<IMethodSymbol>(context)!;
            if (methodSymbol.HasObjCReplacement(out var replacement))
            {
                switch(replacement.Kind)
                {
                    case SymbolReplacementKind.Property:
                    {
                        // Property replacement, like [NSObject hash], [NSObject description]
                        if (syntax.Expression.IsExpression(ExpressionKind.MemberAccess))
                        {
                            builder.Append(syntax.Expression, context);
                        }
                        else
                        {
                            // If the invocation expression is a member access, we assume we don't need to qualify the invocation object
                            builder.Append("self").Dot().Append(syntax.Expression, context);
                        }
                        return builder;
                    }
                    case SymbolReplacementKind.StaticMethod:
                    {
                        if (syntax.Expression.IsExpression(ExpressionKind.MemberAccess))
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)syntax.Expression;
                            builder.Append("CBGetHashCode").Parenthesized().Append(memberAccess.Expression, context).Close();
                            return builder;
                        }

                        break;
                    }
                }
            }

            bool hasEmptyBody;
            if (methodSymbol.IsPartialMethod(out hasEmptyBody) && (hasEmptyBody || methodSymbol.PartialImplementationPart!.ShouldDiscard(context.Conversion)))
            {
                // Partial method has no implementation
                return builder;
            }
            if (methodSymbol.IsNative())
            {
                if (methodSymbol.ReturnType.TypeKind == TypeKind.Enum)
                    builder.Parenthesized().Append(methodSymbol.ReturnType.GetObjCName(context)).Close();
                else if (methodSymbol.ReturnType.GetFullName() == "CodeBinder.cbstring")
                    builder.Parenthesized().Append("SN2OC").Close();

                builder.Append(syntax.Expression, context).Parenthesized().Append(syntax.ArgumentList.Arguments, true, context).Close();
            }
            else
            {
                if (syntax.Expression.IsExpression(ExpressionKind.MemberAccess))
                {
                    // If the invocation expression is a member access, we assume we don't need to qualify the invocation with object
                    builder.Bracketed().Append(syntax.Expression, context).Append(syntax.ArgumentList.Arguments, false, context).Close();
                }
                else
                {
                    // Objective C static or instance message, we need to start a bracketed invocation
                    builder.Bracketed().Append(methodSymbol.IsStatic ? methodSymbol.ContainingType.GetObjCName(context) : "self").Space()
                        .Append(syntax.Expression, context).Append(syntax.ArgumentList.Arguments, false, context).Close();
                }

            }

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, LiteralExpressionSyntax syntax, ObjCCompilationContext context)
        {
            var kind = syntax.Kind();
            switch (kind)
            {
                case SyntaxKind.NullLiteralExpression:
                {
                    var typeSymbol = syntax.GetTypeSymbol(context);
                    if (typeSymbol != null) // NOTE: Null check on nullable types doesn't infer a pointer type
                        builder.Parenthesized().Append(typeSymbol.GetObjCType(ObjCTypeUsageKind.Pointer, context)).Close();

                    return builder.Append("nil");
                }
                case SyntaxKind.StringLiteralExpression:
                {
                    if (syntax.Token.IsKind(SyntaxKind.StringLiteralToken) && syntax.Token.Text.StartsWith("@"))
                        return builder.Append($"@{syntax.Token.Text.Replace("\"", "\"\"")}"); // Handle verbatim strings
                    else
                        return builder.Append($"@{syntax.Token.Text}");
                }
                case SyntaxKind.TrueLiteralExpression:
                {
                    return builder.Append("YES");
                }
                case SyntaxKind.FalseLiteralExpression:
                {
                    return builder.Append("NO");
                }
                default:
                {
                    return builder.Append(syntax.Token.Text);
                }
            }
        }

        // Element access like array "arr[5]"
        public static CodeBuilder Append(this CodeBuilder builder, ElementAccessExpressionSyntax syntax, ObjCCompilationContext context)
        {
            var symbol = syntax.GetSymbol(context);
            if (symbol?.Kind == SymbolKind.Property)
            {
                // Handle indexer properties
                var property = (IPropertySymbol)symbol;
                Debug.Assert(property.IsIndexer);

                builder.Bracketed().Append(syntax.Expression, context).Space()
                    .Append(syntax, property, context).Append(syntax.ArgumentList, false, context).Close();

                return builder;
            }

            // Access "arr" type in "arr[5]" syntax
            var typeSymbol = syntax.Expression.GetTypeSymbol(context);
            if (typeSymbol!.TypeKind == TypeKind.Array)
                builder.Append(syntax.Expression, context).Dot().Append("data").Append(syntax.ArgumentList, true, context);
            else
                builder.Append(syntax.Expression, context).Append(syntax.ArgumentList, true, context);

            return builder;
        }

        // Member access like this.Foo(), or obj.Property
        public static CodeBuilder Append(this CodeBuilder builder, MemberAccessExpressionSyntax syntax, ObjCCompilationContext context)
        {
            if (builder.TryToReplace(syntax, context))
                return builder;

            var symbol = syntax.GetSymbol(context)!;
            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                {
                    var field = (IFieldSymbol)symbol;
                    if (field.TryGetDistinctObjCName(context, out string? name))
                        return builder.Append(name);

                    builder.Append(syntax.Expression, context).Dereference().Append(syntax.Name, context);
                    break;
                }
                case SymbolKind.Property:
                {
                    var property = (IPropertySymbol)symbol;
                    if (symbol.OriginalDefinition.ContainingType.SpecialType == SpecialType.System_Nullable_T
                        && symbol.Name == "Value")
                    {
                        var underlyingType = property.Type;
                        Debug.Assert(underlyingType.IsValueType);
                        if (underlyingType.IsObjCPrimitiveType())
                        {
                            // The only supported nullable type in ObjC is NSNumber
                            // https://developer.apple.com/documentation/foundation/nsnumber?language=objc
                            builder.Append(syntax.Expression, context).Dot().Append(underlyingType.GetNSNumberAccessProperty());
                        }
                        else
                        {
                            // We assume the other value types are treated as reference tpes in ObjC
                            builder.Append(syntax.Expression, context);
                        }

                        return builder;
                    }

                    if (property.IsIndexer && syntax.Parent!.IsStatement())
                    {
                        // Handle eg. obj.Property[5] = false -> [obj.Property set:NO]
                        builder.Bracketed().Append(syntax.Expression, context).Dot().Append(syntax.Name, context).Close();
                        return builder;
                    }

                    builder.Append(syntax.Expression, context).Dot().Append(syntax.Name, context);
                    break;
                }
                case SymbolKind.Method:
                {
                    var methodSymbol = (IMethodSymbol)symbol;
                    if (methodSymbol.HasObjCReplacement(out var replacement) && replacement.Kind == SymbolReplacementKind.Property)
                        builder.Append(syntax.Expression, context).Dot().Append(syntax.Name, context);
                    else
                        builder.Append(syntax.Expression, context).Space().Append(syntax.Name, context);
                    break;
                }
                default:
                    throw new NotSupportedException();
            }


            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ObjectCreationExpressionSyntax syntax, ObjCCompilationContext context)
        {
            var constructorSymbol = syntax.GetSymbol<IMethodSymbol>(context);
            builder.Bracketed().Bracketed().Append(syntax.Type, context).Space().Append("alloc").Close()
                .Append(constructorSymbol.GetObjCName(context)).Append(syntax.ArgumentList!.Arguments, false, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ParenthesizedExpressionSyntax syntax, ObjCCompilationContext context)
        {
            builder.Parenthesized().Append(syntax.Expression, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, PostfixUnaryExpressionSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append(syntax.Operand, context).Append(syntax.GetObjCOperator());
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, PrefixUnaryExpressionSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append(syntax.GetObjCOperator()).Append(syntax.Operand, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, TypeOfExpressionSyntax syntax, ObjCCompilationContext context)
        {
            builder.Bracketed().Append(syntax.Type, context).Space().Append("class").Close();
            return builder;
        }

        #endregion // Expressions

        #region Support

        /// <param name="clangBracketed">True if the invocation is a CLang stile bracketed invocation</param>
        public static CodeBuilder Append(this CodeBuilder builder, BracketedArgumentListSyntax syntax, bool clangBracketed, ObjCCompilationContext context)
        {
            if (syntax.Arguments.Count == 0)
                return builder;

            Debug.Assert(syntax.Arguments.Count == 1);
            if (clangBracketed)
                builder.Bracketed().Append(syntax.Arguments[0].Expression, context);
            else
                builder.Colon().Append(syntax.Arguments[0].Expression, context);

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, IReadOnlyList<ArgumentSyntax> arguments, ObjCCompilationContext context)
        {
            builder.Append(arguments, false, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, IReadOnlyList<ArgumentSyntax> arguments, bool nativeInvoke, ObjCCompilationContext context)
        {
            if (nativeInvoke)
            {
                bool first = true;
                foreach (var arg in arguments)
                {
                    // Handle special cases when invoking native methods
                    var type = arg.Expression.GetTypeSymbol(context)!;
                    if (type.TypeKind == TypeKind.Enum)
                    {
                        // Native enums need cast
                        builder.CommaSeparator(ref first).Parenthesized().Append(type.GetCLangName(arg.IsRefLike() ? CLangTypeUsageKind.DeclarationByRef : CLangTypeUsageKind.Declaration, context)).Close().Append(arg.Expression, context);
                        continue;
                    }

                    builder.CommaSeparator(ref first);
                    void appendExpression(CodeBuilder builder)
                    {
                        builder.Append(arg.Expression, context);
                    }

                    if (type.TypeKind == TypeKind.Array)
                    {
                        builder.Append("CBGetNativeArray").Parenthesized(appendExpression);
                    }
                    else
                    {
                        string fullname = type.GetFullNameNormalized();
                        switch (fullname)
                        {
                            case "System.Runtime.InteropServices.HandleRef":
                            {
                                // Passing "System.HandleRef" to native methods needs further accessing handle
                                builder.Append("CBGetNativeHandle").Parenthesized(appendExpression);
                                break;
                            }
                            case "CodeBinder.cbstring":
                            {
                                if (arg.RefKindKeyword.IsKind(SyntaxKind.None))
                                {
                                    builder.Append("SN2OC").Parenthesized((builder) => appendExpression(builder));
                                }
                                else
                                {
                                    builder.Parenthesized().Append("SN2OC").Close();
                                    appendExpression(builder);
                                }

                                break;
                            }
                            default:
                            {
                                appendExpression(builder);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                bool first = true;
                foreach (var arg in arguments)
                    builder.Space(ref first).Colon().Append(arg.Expression, context);
            }

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, IEnumerable<ExpressionSyntax> expressions, bool commaSeparated, ObjCCompilationContext context)
        {
            if (commaSeparated)
            {
                bool first = true;
                foreach (var expression in expressions)
                    builder.CommaSeparator(ref first).Append(expression, context);
            }
            else
            {
                bool first = true;
                foreach (var expression in expressions)
                    builder.Space(ref first).Colon().Append(expression, context);
            }

            return builder;
        }

        #endregion // Support
    }
}
