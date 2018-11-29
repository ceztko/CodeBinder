// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using CodeBinder.Attributes;
using System.Diagnostics;

namespace CodeBinder.Shared.CSharp
{
    public static class CSharpMethodExtensions
    {
        public static bool IsTypeInterred(this IdentifierNameSyntax syntax)
        {
            // There's no really better way
            return syntax.Identifier.Text == "var";
        }

        public static ExpressionKind ExpressionKind(this ExpressionSyntax node)
        {
            ExpressionKind kind;
            if (IsExpression(node, out kind))
                return kind;

            throw new Exception("Unsupported expression kind");
        }

        public static bool IsExpression<TExpression>(this SyntaxNode node, out TExpression expression)
            where TExpression : ExpressionSyntax
        {
            ExpressionKind kind;
            if (!IsExpression(node, out kind) || getExpressionKind(typeof(TExpression)) != kind)
            {
                expression = null;
                return false;
            }

            expression = node as TExpression;
            return true;
        }

        public static bool IsExpression(this SyntaxNode node, out ExpressionKind kind)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ArrayCreationExpression:
                    kind = CSharp.ExpressionKind.ArrayCreation;
                    return true;
                case SyntaxKind.OmittedArraySizeExpression:
                    kind = CSharp.ExpressionKind.OmittedArraySize;
                    return true;
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
                    kind = CSharp.ExpressionKind.Assignment;
                    return true;
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
                    kind = CSharp.ExpressionKind.Binary;
                    return true;
                case SyntaxKind.CastExpression:
                    kind = CSharp.ExpressionKind.Cast;
                    return true;
                case SyntaxKind.ConditionalExpression:
                    kind = CSharp.ExpressionKind.Conditional;
                    return true;
                case SyntaxKind.ElementAccessExpression:
                    kind = CSharp.ExpressionKind.ElementAccess;
                    return true;
                case SyntaxKind.ObjectInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.ComplexElementInitializerExpression:
                    kind = CSharp.ExpressionKind.Initializer;
                    return true;
                case SyntaxKind.BaseExpression:
                    kind = CSharp.ExpressionKind.Base;
                    return true;
                case SyntaxKind.ThisExpression:
                    kind = CSharp.ExpressionKind.This;
                    return true;
                case SyntaxKind.InvocationExpression:
                    kind = CSharp.ExpressionKind.Invocation;
                    return true;
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.ArgListExpression:
                case SyntaxKind.DefaultLiteralExpression:
                    kind = CSharp.ExpressionKind.Literal;
                    return true;
                case SyntaxKind.PointerMemberAccessExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                    kind = CSharp.ExpressionKind.MemberAccess;
                    return true;
                case SyntaxKind.ObjectCreationExpression:
                    kind = CSharp.ExpressionKind.ObjectCreation;
                    return true;
                case SyntaxKind.ParenthesizedExpression:
                    kind = CSharp.ExpressionKind.Parenthesized;
                    return true;
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                    kind = CSharp.ExpressionKind.PostfixUnary;
                    return true;
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                    kind = CSharp.ExpressionKind.PrefixUnary;
                    return true;
                case SyntaxKind.RefExpression:
                    kind = CSharp.ExpressionKind.Ref;
                    return true;
                case SyntaxKind.TypeOfExpression:
                    kind = CSharp.ExpressionKind.TypeOf;
                    return true;
                case SyntaxKind.ArrayType:
                case SyntaxKind.QualifiedName:
                case SyntaxKind.AliasQualifiedName:
                case SyntaxKind.GenericName:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.NullableType:
                case SyntaxKind.OmittedTypeArgument:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.RefType:
                case SyntaxKind.PointerType:
                case SyntaxKind.TupleType:
                    kind = CSharp.ExpressionKind.Type;
                    return true;
                case SyntaxKind.DeclarationExpression:
                    kind = CSharp.ExpressionKind.Declaration;
                    return true;
                case SyntaxKind.ThrowExpression:
                    kind = CSharp.ExpressionKind.Throw;
                    return true;
                case SyntaxKind.DefaultExpression:
                    kind = CSharp.ExpressionKind.Default;
                    return true;
                case SyntaxKind.AnonymousMethodExpression:
                    kind = CSharp.ExpressionKind.AnonymousMethod;
                    return true;
                case SyntaxKind.ParenthesizedLambdaExpression:
                    kind = CSharp.ExpressionKind.ParenthesizedLambda;
                    return true;
                case SyntaxKind.SimpleLambdaExpression:
                    kind = CSharp.ExpressionKind.SimpleLambda;
                    return true;
                case SyntaxKind.RefValueExpression:
                    kind = CSharp.ExpressionKind.RefValue;
                    return true;
                case SyntaxKind.RefTypeExpression:
                    kind = CSharp.ExpressionKind.RefType;
                    return true;
                case SyntaxKind.ImplicitArrayCreationExpression:
                    kind = CSharp.ExpressionKind.ImplicitArrayCreation;
                    return true;
                case SyntaxKind.ElementBindingExpression:
                    kind = CSharp.ExpressionKind.ElementBinding;
                    return true;
                case SyntaxKind.ImplicitElementAccess:
                    kind = CSharp.ExpressionKind.ImplicitElementAccess;
                    return true;
                case SyntaxKind.MemberBindingExpression:
                    kind = CSharp.ExpressionKind.MemberBinding;
                    return true;
                case SyntaxKind.SizeOfExpression:
                    kind = CSharp.ExpressionKind.SizeOf;
                    return true;
                case SyntaxKind.MakeRefExpression:
                    kind = CSharp.ExpressionKind.MakeRef;
                    return true;
                case SyntaxKind.ImplicitStackAllocArrayCreationExpression:
                    kind = CSharp.ExpressionKind.ImplicitStackAllocArrayCreation;
                    return true;
                case SyntaxKind.InterpolatedStringExpression:
                    kind = CSharp.ExpressionKind.InterpolatedString;
                    return true;
                case SyntaxKind.AwaitExpression:
                    kind = CSharp.ExpressionKind.Await;
                    return true;
                case SyntaxKind.QueryExpression:
                    kind = CSharp.ExpressionKind.Query;
                    return true;
                case SyntaxKind.StackAllocArrayCreationExpression:
                    kind = CSharp.ExpressionKind.StackAllocArrayCreation;
                    return true;
                case SyntaxKind.AnonymousObjectCreationExpression:
                    kind = CSharp.ExpressionKind.AnonymousObjectCreation;
                    return true;
                case SyntaxKind.TupleExpression:
                    kind = CSharp.ExpressionKind.Tuple;
                    return true;
                case SyntaxKind.IsPatternExpression:
                    kind = CSharp.ExpressionKind.IsPattern;
                    return true;
                case SyntaxKind.CheckedExpression:
                    kind = CSharp.ExpressionKind.Checked;
                    return true;
                case SyntaxKind.ConditionalAccessExpression:
                    kind = CSharp.ExpressionKind.ConditionalAccess;
                    return true;
                default:
                    kind = CSharp.ExpressionKind.Unknown;
                    return false;
            }
        }

        static ExpressionKind getExpressionKind(Type type)
        {
            switch (type.Name)
            {
                case nameof(AnonymousMethodExpressionSyntax):
                    return CSharp.ExpressionKind.AnonymousMethod;
                case nameof(ParenthesizedLambdaExpressionSyntax):
                    return CSharp.ExpressionKind.ParenthesizedLambda;
                case nameof(SimpleLambdaExpressionSyntax):
                    return CSharp.ExpressionKind.SimpleLambda;
                case nameof(AnonymousObjectCreationExpressionSyntax):
                    return CSharp.ExpressionKind.AnonymousObjectCreation;
                case nameof(ArrayCreationExpressionSyntax):
                    return CSharp.ExpressionKind.ArrayCreation;
                case nameof(AssignmentExpressionSyntax):
                    return CSharp.ExpressionKind.Assignment;
                case nameof(AwaitExpressionSyntax):
                    return CSharp.ExpressionKind.Await;
                case nameof(BinaryExpressionSyntax):
                    return CSharp.ExpressionKind.Binary;
                case nameof(CastExpressionSyntax):
                    return CSharp.ExpressionKind.Cast;
                case nameof(CheckedExpressionSyntax):
                    return CSharp.ExpressionKind.Checked;
                case nameof(ConditionalAccessExpressionSyntax):
                    return CSharp.ExpressionKind.ConditionalAccess;
                case nameof(ConditionalExpressionSyntax):
                    return CSharp.ExpressionKind.Conditional;
                case nameof(DeclarationExpressionSyntax):
                    return CSharp.ExpressionKind.Declaration;
                case nameof(DefaultExpressionSyntax):
                    return CSharp.ExpressionKind.Default;
                case nameof(ElementAccessExpressionSyntax):
                    return CSharp.ExpressionKind.ElementAccess;
                case nameof(ElementBindingExpressionSyntax):
                    return CSharp.ExpressionKind.ElementBinding;
                case nameof(ImplicitArrayCreationExpressionSyntax):
                    return CSharp.ExpressionKind.ImplicitArrayCreation;
                case nameof(ImplicitElementAccessSyntax):
                    return CSharp.ExpressionKind.ImplicitElementAccess;
                case nameof(ImplicitStackAllocArrayCreationExpressionSyntax):
                    return CSharp.ExpressionKind.ImplicitStackAllocArrayCreation;
                case nameof(InitializerExpressionSyntax):
                    return CSharp.ExpressionKind.Initializer;
                case nameof(BaseExpressionSyntax):
                    return CSharp.ExpressionKind.Base;
                case nameof(ThisExpressionSyntax):
                    return CSharp.ExpressionKind.This;
                case nameof(InterpolatedStringExpressionSyntax):
                    return CSharp.ExpressionKind.InterpolatedString;
                case nameof(InvocationExpressionSyntax):
                    return CSharp.ExpressionKind.Invocation;
                case nameof(IsPatternExpressionSyntax):
                    return CSharp.ExpressionKind.IsPattern;
                case nameof(LiteralExpressionSyntax):
                    return CSharp.ExpressionKind.Literal;
                case nameof(MakeRefExpressionSyntax):
                    return CSharp.ExpressionKind.MakeRef;
                case nameof(MemberAccessExpressionSyntax):
                    return CSharp.ExpressionKind.MemberAccess;
                case nameof(MemberBindingExpressionSyntax):
                    return CSharp.ExpressionKind.MemberBinding;
                case nameof(ObjectCreationExpressionSyntax):
                    return CSharp.ExpressionKind.ObjectCreation;
                case nameof(OmittedArraySizeExpressionSyntax):
                    return CSharp.ExpressionKind.OmittedArraySize;
                case nameof(ParenthesizedExpressionSyntax):
                    return CSharp.ExpressionKind.Parenthesized;
                case nameof(PostfixUnaryExpressionSyntax):
                    return CSharp.ExpressionKind.PostfixUnary;
                case nameof(PrefixUnaryExpressionSyntax):
                    return CSharp.ExpressionKind.PrefixUnary;
                case nameof(QueryExpressionSyntax):
                    return CSharp.ExpressionKind.Query;
                case nameof(RefExpressionSyntax):
                    return CSharp.ExpressionKind.Ref;
                case nameof(RefTypeExpressionSyntax):
                    return CSharp.ExpressionKind.RefType;
                case nameof(RefValueExpressionSyntax):
                    return CSharp.ExpressionKind.RefValue;
                case nameof(SizeOfExpressionSyntax):
                    return CSharp.ExpressionKind.SizeOf;
                case nameof(StackAllocArrayCreationExpressionSyntax):
                    return CSharp.ExpressionKind.StackAllocArrayCreation;
                case nameof(ThrowExpressionSyntax):
                    return CSharp.ExpressionKind.Throw;
                case nameof(TupleExpressionSyntax):
                    return CSharp.ExpressionKind.Tuple;
                case nameof(TypeOfExpressionSyntax):
                    return CSharp.ExpressionKind.TypeOf;
                case nameof(TypeSyntax):
                    return CSharp.ExpressionKind.Type;
                default:
                    throw new Exception();
            }
        }

        public static StatementKind StatementKind(this StatementSyntax node)
        {
            StatementKind kind;
            if (IsStatement(node, out kind))
                return kind;

            throw new Exception("Unsupported statement kind");
        }

        public static bool IsStatement<TStatement>(this SyntaxNode node, out TStatement statement)
            where TStatement : StatementSyntax
        {
            StatementKind kind;
            if (!IsStatement(node, out kind) || getStatementKind(typeof(TStatement)) != kind)
            {
                statement = null;
                return false;
            }

            statement = node as TStatement;
            return true;
        }

        public static bool IsStatement(this SyntaxNode node, out StatementKind kind)
        {
            switch (node.Kind())
            {
                case SyntaxKind.Block:
                    kind = CSharp.StatementKind.Block;
                    return true;
                case SyntaxKind.BreakStatement:
                    kind = CSharp.StatementKind.Break;
                    return true;
                case SyntaxKind.ForEachStatement:
                    kind = CSharp.StatementKind.ForEach;
                    return true;
                case SyntaxKind.ForEachVariableStatement:
                    kind = CSharp.StatementKind.ForEachVariable;
                    return true;
                case SyntaxKind.ContinueStatement:
                    kind = CSharp.StatementKind.Continue;
                    return true;
                case SyntaxKind.DoStatement:
                    kind = CSharp.StatementKind.Do;
                    return true;
                case SyntaxKind.EmptyStatement:
                    kind = CSharp.StatementKind.Empty;
                    return true;
                case SyntaxKind.ExpressionStatement:
                    kind = CSharp.StatementKind.Expression;
                    return true;
                case SyntaxKind.ForStatement:
                    kind = CSharp.StatementKind.For;
                    return true;
                case SyntaxKind.IfStatement:
                    kind = CSharp.StatementKind.If;
                    return true;
                case SyntaxKind.LocalDeclarationStatement:
                    kind = CSharp.StatementKind.LocalDeclaration;
                    return true;
                case SyntaxKind.LockStatement:
                    kind = CSharp.StatementKind.Lock;
                    return true;
                case SyntaxKind.ReturnStatement:
                    kind = CSharp.StatementKind.Return;
                    return true;
                case SyntaxKind.SwitchStatement:
                    kind = CSharp.StatementKind.Switch;
                    return true;
                case SyntaxKind.ThrowStatement:
                    kind = CSharp.StatementKind.Throw;
                    return true;
                case SyntaxKind.TryStatement:
                    kind = CSharp.StatementKind.Try;
                    return true;
                case SyntaxKind.UsingStatement:
                    kind = CSharp.StatementKind.Using;
                    return true;
                case SyntaxKind.WhileStatement:
                    kind = CSharp.StatementKind.While;
                    return true;
                case SyntaxKind.CheckedStatement:
                    kind = CSharp.StatementKind.Checked;
                    return true;
                case SyntaxKind.UnsafeStatement:
                    kind = CSharp.StatementKind.Unsafe;
                    return true;
                case SyntaxKind.LabeledStatement:
                    kind = CSharp.StatementKind.Labeled;
                    return true;
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                    kind = CSharp.StatementKind.Goto;
                    return true;
                case SyntaxKind.FixedStatement:
                    kind = CSharp.StatementKind.Fixed;
                    return true;
                case SyntaxKind.LocalFunctionStatement:
                    kind = CSharp.StatementKind.LocalFunction;
                    return true;
                case SyntaxKind.YieldBreakStatement:
                case SyntaxKind.YieldReturnStatement:
                    kind = CSharp.StatementKind.Yield;
                    return true;
                default:
                    kind = CSharp.StatementKind.Unknown;
                    return false;
            }
        }

        static StatementKind getStatementKind(Type type)
        {
            switch (type.Name)
            {
                case nameof(BlockSyntax):
                    return CSharp.StatementKind.Block;
                case nameof(BreakStatementSyntax):
                    return CSharp.StatementKind.Break;
                case nameof(CheckedStatementSyntax):
                    return CSharp.StatementKind.Checked;
                case nameof(ForEachStatementSyntax):
                    return CSharp.StatementKind.ForEach;
                case nameof(ForEachVariableStatementSyntax):
                    return CSharp.StatementKind.ForEachVariable;
                case nameof(ContinueStatementSyntax):
                    return CSharp.StatementKind.Continue;
                case nameof(DoStatementSyntax):
                    return CSharp.StatementKind.Do;
                case nameof(EmptyStatementSyntax):
                    return CSharp.StatementKind.Empty;
                case nameof(ExpressionStatementSyntax):
                    return CSharp.StatementKind.Expression;
                case nameof(FixedStatementSyntax):
                    return CSharp.StatementKind.Fixed;
                case nameof(ForStatementSyntax):
                    return CSharp.StatementKind.For;
                case nameof(GotoStatementSyntax):
                    return CSharp.StatementKind.Goto;
                case nameof(IfStatementSyntax):
                    return CSharp.StatementKind.If;
                case nameof(LabeledStatementSyntax):
                    return CSharp.StatementKind.Labeled;
                case nameof(LocalDeclarationStatementSyntax):
                    return CSharp.StatementKind.LocalDeclaration;
                case nameof(LocalFunctionStatementSyntax):
                    return CSharp.StatementKind.LocalFunction;
                case nameof(LockStatementSyntax):
                    return CSharp.StatementKind.Lock;
                case nameof(ReturnStatementSyntax):
                    return CSharp.StatementKind.Return;
                case nameof(SwitchStatementSyntax):
                    return CSharp.StatementKind.Switch;
                case nameof(ThrowStatementSyntax):
                    return CSharp.StatementKind.Throw;
                case nameof(TryStatementSyntax):
                    return CSharp.StatementKind.Try;
                case nameof(UnsafeStatementSyntax):
                    return CSharp.StatementKind.Unsafe;
                case nameof(UsingStatementSyntax):
                    return CSharp.StatementKind.Using;
                case nameof(WhileStatementSyntax):
                    return CSharp.StatementKind.While;
                case nameof(YieldStatementSyntax):
                    return CSharp.StatementKind.Yield;
                default:
                    throw new Exception();
            }
        }

        // Note: Declations -> GetDeclaredSymbol()
        public static string GetFullName(this MemberDeclarationSyntax node, ICompilationContextProvider provider)
        {
            var symbol = node.GetDeclaredSymbol(provider);
            return symbol.GetFullName();
        }

        // Note: Declations -> GetDeclaredSymbol()
        public static string GetQualifiedName(this MemberDeclarationSyntax node, ICompilationContextProvider provider)
        {
            var symbol = node.GetDeclaredSymbol(provider);
            return symbol.GetQualifiedName();
        }

        // Note: Types -> GetTypeSymbol()
        public static string GetFullName(this TypeSyntax node, ICompilationContextProvider provider)
        {
            var symbol = node.GetTypeSymbol(provider);
            return symbol.GetFullName();
        }

        // Note: Types -> GetTypeSymbol()
        public static string GetQualifiedName(this TypeSyntax node, ICompilationContextProvider provider)
        {
            var symbol = node.GetTypeSymbol(provider);
            return symbol.GetQualifiedName();
        }

        public static CSharpTypeParameters GetTypeParameters(this MethodDeclarationSyntax syntax, ICompilationContextProvider provider)
        {
            var symbol = syntax.GetDeclaredSymbol<IMethodSymbol>(provider);
            if (symbol.OverriddenMethod != null)
            {
                // Java requires all constraints to be written as well
                Debug.Assert(symbol.OverriddenMethod.DeclaringSyntaxReferences.Length == 1);
                var parentDeclaration = (MethodDeclarationSyntax)symbol.OverriddenMethod.DeclaringSyntaxReferences[0].GetSyntax();
                return mergeTypeConstraint(syntax.TypeParameterList.Parameters, parentDeclaration.ConstraintClauses);
            }

            return mergeTypeConstraint(syntax.TypeParameterList.Parameters, syntax.ConstraintClauses);
        }

        public static CSharpTypeParameters GetTypeParameters(this TypeDeclarationSyntax syntax)
        {
            return mergeTypeConstraint(syntax.TypeParameterList.Parameters, syntax.ConstraintClauses);
        }

        private static CSharpTypeParameters mergeTypeConstraint(
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            var parameters = new List<CSharpTypeParameter>(typeParameters.Count);
            for (int i = 0; i < typeParameters.Count; i++)
            {
                var type = typeParameters[i];
                var constraints = constraintClauses.FirstOrDefault((element) => element.Name.Identifier.Text == type.Identifier.Text);
                parameters.Add(new CSharpTypeParameter(type, constraints));
            }
            return new CSharpTypeParameters(parameters);
        }

        public static bool IsNone(this SyntaxToken token)
        {
            return token.Kind() == SyntaxKind.None;
        }

        public static string GetTypeName(ref this MethodParameterInfo parameter, out ITypeSymbol typeSymbol)
        {
            if (parameter.Type.Type == null)
            {
                typeSymbol = null; // TODO: Lookup for proper System.Void ITypeSymbol?
                return "System.Void";
            }

            string constantTypeName = parameter.Type.Type.GetFullName();
            switch (constantTypeName)
            {
                case "System.String":
                {
                    typeSymbol = null;
                    return parameter.Type.Value.ToString();
                }
                case "System.Type":
                {
                    typeSymbol = parameter.Type.Value as ITypeSymbol;
                    return typeSymbol.GetFullName();
                }
                default:
                    throw new Exception();
            }
        }

        public static IReadOnlyList<MethodSignatureInfo> GetMethodSignatures(this MethodDeclarationSyntax method, ICompilationContextProvider provider)
        {
            var ret = new List<MethodSignatureInfo>();
            var attributes = method.GetAttributes(provider);
            foreach (var attribute in attributes)
            {
                if (attribute.IsAttribute<SignatureAttribute>())
                {
                    var methodData = getMethodDataFromConstructorParameter(method, attribute);
                    ret.Add(methodData);
                }
            }
            return ret;
        }

        private static MethodSignatureInfo getMethodDataFromConstructorParameter(MethodDeclarationSyntax method, AttributeData attribute)
        {
            if (attribute.ConstructorArguments.Length != 1)
                throw new Exception("SignatureAttribute must be constructed with single parameter");

            var constructorParam = attribute.ConstructorArguments[0];
            string constructorParamTypeName = constructorParam.Type.GetFullName();
            MethodParameterInfo[] parameters;
            switch (constructorParamTypeName)
            {
                case "System.Object[]":
                {
                    if (constructorParam.Values.Length % 2 != 0)
                        throw new Exception("Object count with parameters name must be divisible by two");

                    int parameterCount = constructorParam.Values.Length / 2;
                    parameters = new MethodParameterInfo[parameterCount];
                    for (int i = 0; i < parameterCount; i++)
                    {
                        var typeConstant = constructorParam.Values[i * 2];
                        string parameterName = constructorParam.Values[i * 2 + 1].Value as string;
                        if (parameterName == null)
                            throw new Exception("Parameter name must be a string");

                        parameters[i] = new MethodParameterInfo(typeConstant, parameterName);
                    }

                    break;
                }
                case "System.Type[]":
                {
                    if (method.ParameterList.Parameters.Count != constructorParam.Values.Length)
                        throw new Exception("Method parameter count must be same as provided type count");

                    int parameterCount = constructorParam.Values.Length;
                    parameters = new MethodParameterInfo[parameterCount];
                    for (int i = 0; i < parameterCount; i++)
                    {
                        var typeConstant = constructorParam.Values[i];
                        string parameterName = method.ParameterList.Parameters[i].Identifier.Text;
                        parameters[i] = new MethodParameterInfo(typeConstant, parameterName);
                    }
                    break;
                }
                default:
                    throw new Exception();
            }

            TypedConstant returnType = new TypedConstant();
            string methodName = method.Identifier.Text;
            foreach (var namedArgument in attribute.NamedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "ReturnType":
                    {
                        returnType = namedArgument.Value;
                        break;
                    }
                    case "MethodName":
                    {
                        methodName = namedArgument.Value.Value?.ToString() ?? methodName;
                        break;
                    }
                    default:
                        throw new Exception();
                }
            }

            var ret = new MethodSignatureInfo();
            ret.MethodName = methodName;
            ret.Modifiers = method.GetCSharpModifiers().ToArray();
            ret.ReturnType = new MethodParameterInfo(returnType, null);
            ret.Parameters = parameters;
            return ret;
        }

        public static bool IsNative(this IMethodSymbol method)
        {
            return method.IsExtern && method.HasAttribute<DllImportAttribute>();
        }

        public static bool IsNative(this MethodDeclarationSyntax method, ICompilationContextProvider provider)
        {
            if (!method.HasAttribute<DllImportAttribute>(provider))
                return false;

            return method.Modifiers.Any(SyntaxKind.ExternKeyword);
        }

        public static bool IsFlag(this EnumDeclarationSyntax node, ICompilationContextProvider provider)
        {
            return node.HasAttribute<FlagsAttribute>(provider);
        }

        public static bool IsRef(this ParameterSyntax parameter)
        {
            return parameter.Modifiers.Any(SyntaxKind.RefKeyword);
        }

        public static bool IsOut(this ParameterSyntax parameter)
        {
            return parameter.Modifiers.Any(SyntaxKind.OutKeyword);
        }

        public static int GetEnumValue(this EnumMemberDeclarationSyntax node, ICompilationContextProvider provider)
        {
            return node.EqualsValue.Value.GetValue<int>(provider);
        }

        public static IReadOnlyList<SyntaxKind> GetCSharpModifiers(this BaseFieldDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        explicitAccessibility = true;
                        break;
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.ConstKeyword:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<SyntaxKind>();

            if (!explicitAccessibility)
                ret.Add(SyntaxKind.InternalKeyword);

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Kind());

            return ret;
        }

        public static IReadOnlyList<SyntaxKind> GetCSharpModifiers(this BaseTypeDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        explicitAccessibility = true;
                        break;
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.PartialKeyword:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<SyntaxKind>();

            if (!explicitAccessibility)
                ret.Add(SyntaxKind.InternalKeyword);

            foreach (var modifier in node.Modifiers)
            {
                var kind = modifier.Kind();
                if (kind == SyntaxKind.PartialKeyword)
                    continue;

                ret.Add(modifier.Kind());
            }

            return ret;
        }

        public static IReadOnlyList<SyntaxKind> GetCSharpModifiers(this BaseMethodDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        explicitAccessibility = true;
                        break;
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.ExternKeyword:
                    case SyntaxKind.NewKeyword:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<SyntaxKind>();

            if (!explicitAccessibility)
                ret.Add(SyntaxKind.PrivateKeyword);

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Kind());

            return ret;
        }

        public static IReadOnlyList<SyntaxKind> GetCSharpModifiers(this BasePropertyDeclarationSyntax node)
        {
            bool explicitAccessibility = false;
            foreach (var modifier in node.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        explicitAccessibility = true;
                        break;
                    case SyntaxKind.StaticKeyword:
                    case SyntaxKind.VirtualKeyword:
                    case SyntaxKind.AbstractKeyword:
                    case SyntaxKind.OverrideKeyword:
                    case SyntaxKind.SealedKeyword:
                    case SyntaxKind.NewKeyword:
                        break;
                    default:
                        throw new Exception();
                }
            }

            var ret = new List<SyntaxKind>();

            if (!explicitAccessibility)
                ret.Add(SyntaxKind.PrivateKeyword);

            foreach (var modifier in node.Modifiers)
                ret.Add(modifier.Kind());

            return ret;
        }

        public static IReadOnlyList<SyntaxKind> GetCSharpModifiers(this AccessorDeclarationSyntax node)
        {
            var ret = new List<SyntaxKind>();
            foreach (var modifier in node.Modifiers)
            {
                var kind = modifier.Kind();
                switch (kind)
                {
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.PrivateKeyword:
                        ret.Add(kind);
                        break;
                    default:
                        throw new Exception();
                }
            }

            return ret;
        }

        public static string GetName(this GenericNameSyntax node)
        {
            return node.Identifier.Text;
        }

        public static string GetName(this IdentifierNameSyntax node)
        {
            return node.Identifier.Text;
        }

        public static string GetName(this BaseTypeDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }

        public static string GetName(this EnumMemberDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }

        public static string GetName(this MethodDeclarationSyntax node)
        {
            return node.Identifier.Text;
        }
    }
}
