﻿// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Attributes;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeBinder.Shared.CSharp;

public static class CSharpMethodExtensions
{
    static Regex? _splitCamelCase;

    public static BlockSyntax FindAncestorBlock(this SyntaxNode node)
    {
        return FindAncestorBlock(node, out _);
    }

    public static BlockSyntax FindAncestorBlock(this SyntaxNode node, out int index)
    {
        SyntaxNode prev = node;
        while (true)
        {
            if (node.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)node;
                index = -1;
                if (prev != node)
                    index = block.Statements.IndexOf((StatementSyntax)prev);

                return block;
            }

            if (node.Parent == null)
                throw new Exception("Can't find ancestor block");

            prev = node;
            node = node.Parent;
        }
    }

    public static bool TryGetModuleName(this TypeDeclarationSyntax type, ICompilationProvider provider,
        [NotNullWhen(true)]out string? moduleName)
    {
        // To support partial calsses, iterate syntax attributes,
        // don't infer them from context
        foreach (var attributeList in type.AttributeLists)
        {
            foreach (var attribute in attributeList.GetAttributes(provider))
            {
                if (attribute.IsAttribute<ModuleAttribute>())
                {
                    moduleName = attribute.GetConstructorArgument<string>(0);
                    return true;
                }
            }
        }

        // As a last resort ask for all the symbol attributes
        if (type.TryGetAttribute<ModuleAttribute>(provider, out var attr))
        {
            moduleName = attr.GetConstructorArgument<string>(0);
            return true;
        }

        moduleName = null;
        return false;
    }

    public static string GetMappedNamespaceName(this MemberDeclarationSyntax node, NamespaceMappingTree mapping,
        ICompilationProvider provider)
    {
        return mapping.GetMappedNamespace(
            node.GetContainingNamespaceName(provider), NamespaceNormalization.None);
    }

    public static string GetMappedNamespaceName(this MemberDeclarationSyntax node, NamespaceMappingTree mapping,
        NamespaceNormalization normalization, ICompilationProvider provider)
    {
        return mapping.GetMappedNamespace(
            node.GetContainingNamespaceName(provider), normalization);
    }

    public static string GetContainingNamespaceName(this MemberDeclarationSyntax node, ICompilationProvider provider)
    {
        var symbol = node.GetDeclaredSymbol(provider)!;
        return symbol.ContainingNamespace.GetFullName();
    }

    public static MethodDeclarationSyntax? GetDeclarationSyntax(this IMethodSymbol method)
    {
        foreach (var reference in method.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax();
            if (syntax.IsKind(SyntaxKind.MethodDeclaration))
                return (MethodDeclarationSyntax)syntax;
        }

        return null;
    }

    public static bool IsPartialMethod(this BaseMethodDeclarationSyntax method, out bool hasEmptyBody)
    {
        if (method.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            hasEmptyBody = method.Body == null;
            return true;
        }

        hasEmptyBody = false;
        return false;
    }

    // https://github.com/dotnet/roslyn/issues/48#issuecomment-75641847
    public static bool IsPartialMethod(this IMethodSymbol method, out bool hasEmptyBody)
    {
        if (method.MethodKind == MethodKind.LocalFunction || method.IsDefinedInMetadata())
        {
            hasEmptyBody = false;
            return false;
        }

        foreach (var reference in method.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax();
            if (!syntax.IsKind(SyntaxKind.MethodDeclaration))
                continue;

            var node = (MethodDeclarationSyntax)syntax;
            if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                hasEmptyBody = false;
                return false;
            }
        }

        hasEmptyBody = method.PartialImplementationPart == null || method.PartialDefinitionPart != null;
        return true;
    }

    /// <returns>False if it's not defined in source</returns>
    public static bool IsDefinedInMetadata(this ISymbol symbol)
    {
        return symbol.Locations.Any(loc => loc.IsInMetadata);
    }

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

    public static bool IsExpression<TExpression>(this SyntaxNode node, [NotNullWhen(true)]out TExpression? expression)
        where TExpression : ExpressionSyntax
    {
        ExpressionKind kind;
        if (!IsExpression(node, out kind) || getExpressionKind(typeof(TExpression)) != kind)
        {
            expression = null;
            return false;
        }

        expression = (TExpression)node;
        return true;
    }

    public static bool IsExpression(this SyntaxNode node)
    {
        return IsExpression(node, out var kind);
    }

    public static bool IsExpression(this SyntaxNode node, ExpressionKind kind)
    {
        if (IsExpression(node, out var actualkind))
            return actualkind == kind;
        else
            return false;
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
            case SyntaxKind.SuppressNullableWarningExpression:
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

    public static bool IsStatement<TStatement>(this SyntaxNode node, [NotNullWhen(true)]out TStatement? statement)
        where TStatement : StatementSyntax
    {
        StatementKind kind;
        if (!IsStatement(node, out kind) || getStatementKind(typeof(TStatement)) != kind)
        {
            statement = null;
            return false;
        }

        statement = (TStatement)node;
        return true;
    }

    public static bool IsStatement(this SyntaxNode node, StatementKind kind)
    {
        if (IsStatement(node, out var actualkind))
            return actualkind == kind;
        else
            return false;
    }

    public static bool IsStatement(this SyntaxNode node)
    {
        return IsStatement(node, out var kind);
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

    public static string GetFullName(this MemberDeclarationSyntax node, ICompilationProvider provider)
    {
        var symbol = node.GetDeclaredSymbol(provider)!;
        return symbol.GetFullName();
    }

    public static string GetQualifiedName(this MemberDeclarationSyntax node, ICompilationProvider provider)
    {
        var symbol = node.GetDeclaredSymbol(provider)!;
        return symbol.GetQualifiedName();
    }

    public static string GetQualifiedName(this MemberDeclarationSyntax node, bool includeTypeParameters, ICompilationProvider provider)
    {
        var symbol = node.GetDeclaredSymbol(provider)!;
        return symbol.GetQualifiedName(includeTypeParameters);
    }

    public static string GetFullName(this TypeSyntax node, ICompilationProvider provider)
    {
        var symbol = node.GetTypeSymbolThrow(provider);
        return symbol.GetFullName();
    }

    public static string GetQualifiedName(this TypeSyntax node, ICompilationProvider provider)
    {
        var symbol = node.GetTypeSymbolThrow(provider);
        return symbol.GetQualifiedName();
    }

    public static string GetQualifiedName(this TypeSyntax node, bool includeTypeParameters, ICompilationProvider provider)
    {
        var symbol = node.GetTypeSymbolThrow(provider);
        return symbol.GetQualifiedName(includeTypeParameters);
    }

    /// <summary>
    /// Try get the symbol for this node, also in the case of ElementAccessExpression
    /// </summary>
    /// <remarks>Throws if the symbol can't be retrieved</remarks>
    public static ISymbol GetSymbolSafe(this SyntaxNode node, ICompilationProvider provider)
    {
        if (node.IsKind(SyntaxKind.ElementAccessExpression))
        {
            var access = (ElementAccessExpressionSyntax)node;
            node = access.Expression;
        }

        var model = node.GetSemanticModel(provider);
        return model.GetSymbolInfo(node).Symbol ?? throw new Exception($"Unable to get symbol for syntax: {node}");
    }

    public static ITypeSymbol GetTypeSymbolThrow(this TypeSyntax node, ICompilationProvider provider)
    {
        var info = node.GetTypeInfo(provider);
        return info.ConvertedType ?? throw new ArgumentNullException(nameof(info.ConvertedType));
    }

    public static ITypeSymbol? GetTypeSymbol(this TypeSyntax node, ICompilationProvider provider)
    {
        var info = node.GetTypeInfo(provider);
        return info.ConvertedType;
    }

    public static ITypeSymbol GetTypeSymbol(this BaseTypeDeclarationSyntax node, ICompilationProvider provider)
    {
        return node.GetDeclaredSymbol<ITypeSymbol>(provider);
    }

    public static CSharpTypeParameters GetTypeParameters(this MethodDeclarationSyntax syntax, ICompilationProvider provider)
    {
        var symbol = syntax.GetDeclaredSymbol<IMethodSymbol>(provider);
        if (symbol.OverriddenMethod != null)
        {
            // Java requires all constraints to be written as well
            Debug.Assert(symbol.OverriddenMethod.DeclaringSyntaxReferences.Length == 1);
            var parentDeclaration = (MethodDeclarationSyntax)symbol.OverriddenMethod.DeclaringSyntaxReferences[0].GetSyntax();
            return mergeTypeConstraint(syntax.TypeParameterList!.Parameters, parentDeclaration.ConstraintClauses);
        }

        return mergeTypeConstraint(syntax.TypeParameterList!.Parameters, syntax.ConstraintClauses);
    }

    public static CSharpTypeParameters GetTypeParameters(this TypeDeclarationSyntax syntax)
    {
        return mergeTypeConstraint(syntax.TypeParameterList!.Parameters, syntax.ConstraintClauses);
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
        return token.IsKind(SyntaxKind.None);
    }

    /// <summary>
    /// True if the method is DllImport or if it should be converted as native
    /// </summary>
    public static bool IsNative(this IMethodSymbol method)
    {
        var attributes = method.GetAttributes();
        return (method.IsExtern && attributes.HasAttribute<DllImportAttribute>())
            || method.HasAttribute<NativeAttribute>();
    }

    public static bool IsNative(this ITypeSymbol type)
    {
        return type.TypeKind == TypeKind.Delegate && type.HasAttribute<UnmanagedFunctionPointerAttribute>();
    }

    public static bool IsNative(this MethodDeclarationSyntax method, ICompilationProvider provider)
    {
        if (!method.HasAttribute<DllImportAttribute>(provider))
            return false;

        return method.Modifiers.Any(SyntaxKind.ExternKeyword);
    }

    public static bool IsFlag(this EnumDeclarationSyntax node, ICompilationProvider provider)
    {
        return node.HasAttribute<FlagsAttribute>(provider);
    }

    public static bool ShouldDiscard(this SyntaxNode node, CompilationContext context)
    {
        return ShouldDiscard(node, context, context.Conversion);
    }

    public static bool ShouldDiscard(this SyntaxNode node, ICompilationProvider compilation, LanguageConversion conversion)
    {
        var symbol = getDeclaredSymbol(node, compilation);
        if (symbol == null)
            return false;

        return symbol.ShouldDiscard(conversion);
    }

    public static bool HasAttribute<TAttribute>(this CSharpSyntaxNode node, ICompilationProvider provider)
        where TAttribute : Attribute
    {
        return GetDeclaredSymbol(node, provider)?.HasAttribute<TAttribute>() == true;
    }


    public static bool TryGetAttribute<TAttribute>(this CSharpSyntaxNode node, ICompilationProvider provider, [NotNullWhen(true)] out AttributeData? data)
        where TAttribute : Attribute
    {
        var symbol = GetDeclaredSymbol(node, provider);
        if (symbol == null)
        {
            data = null;
            return false;
        }

        return symbol.TryGetAttribute<TAttribute>(out data);
    }

    public static AttributeData GetAttribute<TAttribute>(this CSharpSyntaxNode node, ICompilationProvider provider)
        where TAttribute : Attribute
    {
        AttributeData? ret;
        if (!TryGetAttribute<TAttribute>(node, provider, out ret))
            throw new Exception($"Missing attribute {typeof(TAttribute).Name}");

        return ret!;
    }

    public static IReadOnlyList<AttributeData> GetAttributes<TAttribute>(this CSharpSyntaxNode node, ICompilationProvider provider)
       where TAttribute : Attribute
    {
        var symbol = GetDeclaredSymbol(node, provider);
        if (symbol == null)
            return new AttributeData[0];

        return symbol.GetAttributes<TAttribute>();
    }

    public static IReadOnlyList<AttributeData> GetAttributes(this AttributeListSyntax attributes, ICompilationProvider provider)
    {
        // Collect pertinent syntax trees from these attributes
        var acceptedTrees = new HashSet<SyntaxTree>();
        foreach (var attribute in attributes.Attributes)
            acceptedTrees.Add(attribute.SyntaxTree);

        // https://stackoverflow.com/questions/28947456/how-do-i-get-attributedata-from-attributesyntax-in-roslyn
        var parentSymbol = attributes.Parent!.GetDeclaredSymbol(provider)!;
        var parentAttributes = parentSymbol.GetAttributes();
        var ret = new List<AttributeData>();
        foreach (var attribute in parentAttributes)
        {
            if (acceptedTrees.Contains(attribute.ApplicationSyntaxReference!.SyntaxTree))
                ret.Add(attribute);
        }

        return ret;
    }

    public static TSymbol GetDeclaredSymbol<TSymbol>(this CSharpSyntaxNode node, ICompilationProvider provider)
        where TSymbol : class, ISymbol
    {
        return getDeclaredSymbol(node, provider) as TSymbol ?? throw new Exception($"Unable to get declared symbol {typeof(ISymbol).Name} in syntax: {node}");
    }

    // In fields declaration, get synbol of first declared symbol
    public static ISymbol? GetDeclaredSymbol(CSharpSyntaxNode node, ICompilationProvider provider)
    {
        return getDeclaredSymbol(node, provider);
    }

    static ISymbol? getDeclaredSymbol(SyntaxNode node, ICompilationProvider provider)
    {
        if (node.IsKind(SyntaxKind.FieldDeclaration))
        {
            var field = (FieldDeclarationSyntax)node;
            Debug.Assert(field.Declaration.Variables.Count == 1);
            return field.Declaration.Variables[0].GetDeclaredSymbol(provider);
        }
        else if (node.IsKind(SyntaxKind.VariableDeclaration))
        {
            var field = (VariableDeclarationSyntax)node;
            Debug.Assert(field.Variables.Count == 1);
            return field.Variables[0].GetDeclaredSymbol(provider);
        }
        else
        {
            return node.GetDeclaredSymbol(provider);
        }
    }

    /// <summary>
    /// Return the main declaration (any delcaration with non null baselist if present, or just the first found)
    /// </summary>
    public static TTypeDeclaration GetMainDeclaration<TTypeDeclaration>(this ITypeSymbol symbol)
        where TTypeDeclaration : BaseTypeDeclarationSyntax
    {
        TTypeDeclaration? declaration = null;
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            declaration = (TTypeDeclaration)reference.GetSyntax();
            if (declaration.BaseList != null)
                return declaration;
        }

        if (declaration == null)
            throw new Exception($"Can't find declaration for symbol {symbol}");

        return declaration;
    }

    /// <summary>
    /// Say if type is partial and if so return the main delclaration (delclaration with non null baselist if present)
    /// </summary>
    public static bool IsPartial(this TypeDeclarationSyntax declaration, ICompilationProvider provider, out TypeDeclarationSyntax mainDeclaration)
    {
        bool isPartial = declaration.IsPartial();
        if (isPartial && declaration.BaseList == null)
        {
            var symbol = declaration.GetDeclaredSymbol<ITypeSymbol>(provider);
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                declaration = (TypeDeclarationSyntax)reference.GetSyntax();
                if (declaration.BaseList != null)
                    break;
            }

            mainDeclaration = declaration;
            return true;
        }

        mainDeclaration = declaration;
        return isPartial;
    }

    public static bool IsPartial(this TypeDeclarationSyntax declaration)
    {
        return declaration.Modifiers.Any(SyntaxKind.PartialKeyword);
    }

    /// <summary>
    /// Return base declarations
    /// </summary>
    /// <remarks>Try to identify principal (aka with a base types list) declaration in case of partial types</remarks>
    public static IReadOnlyList<BaseTypeDeclarationSyntax> GetBaseDeclarations(this BaseTypeDeclarationSyntax type, ICompilationProvider provider)
    {
        var ret = new List<BaseTypeDeclarationSyntax>();
        if (type.BaseList != null)
        {
            foreach (var baseType in type.BaseList.Types)
            {
                var symbol = baseType.Type.GetTypeSymbolThrow(provider)!;
                BaseTypeDeclarationSyntax? foundDeclaration = null;
                foreach (var reference in symbol.DeclaringSyntaxReferences)
                {
                    var declaration = (BaseTypeDeclarationSyntax)reference.GetSyntax();
                    if (declaration.BaseList != null)
                    {
                        foundDeclaration = declaration;
                        break;
                    }

                    // Any other declaration is fine if we can't decide if it's principal
                    foundDeclaration = declaration;
                    break;
                }

                if (foundDeclaration != null)
                    ret.Add(foundDeclaration);
            }
        }

        return ret;

    }

    public static bool IsAbstract(this BasePropertyDeclarationSyntax property, ICompilationProvider provider)
    {
        return property.GetDeclaredSymbol<IPropertySymbol>(provider).IsAbstract;
    }

    public static bool IsAbstract(this BaseMethodDeclarationSyntax property, ICompilationProvider provider)
    {
        return property.GetDeclaredSymbol(provider)!.IsAbstract;
    }

    public static bool IsReadOnly(this BasePropertyDeclarationSyntax property, ICompilationProvider provider)
    {
        return property.GetDeclaredSymbol<IPropertySymbol>(provider).IsReadOnly;
    }

    public static bool IsWriteOnly(this BasePropertyDeclarationSyntax property, ICompilationProvider provider)
    {
        return property.GetDeclaredSymbol<IPropertySymbol>(provider).IsWriteOnly;
    }

    public static bool IsAutomatic(this BasePropertyDeclarationSyntax property, ICompilationProvider provider)
    {
        if (property.GetDeclaredSymbol<IPropertySymbol>(provider).IsAbstract)
        {
            return false;
        }
        else
        {
            foreach (var accessor in property.AccessorList!.Accessors)
            {
                if (accessor.Body != null)
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// True if this parameter is a R/W ref parameter
    /// </summary>
    public static bool IsRefLike(this IParameterSymbol parameter)
    {
        switch (parameter.RefKind)
        {
            case RefKind.Ref:
            case RefKind.Out:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// True if this parameter is a ref argument
    /// </summary>
    public static bool IsRefLike(this ArgumentSyntax argument)
    {
        switch(argument.RefKindKeyword.Kind())
        {
            case SyntaxKind.RefKeyword:
            case SyntaxKind.OutKeyword:
            case SyntaxKind.InKeyword:
                return true;
            default:
                return false;
        }
    }

    public static bool IsRefLike(this ParameterSyntax parameter)
    {
        return parameter.Modifiers.Any(SyntaxKind.RefKeyword)
            || parameter.Modifiers.Any(SyntaxKind.OutKeyword)
            || parameter.Modifiers.Any(SyntaxKind.InKeyword);
    }

    public static bool IsRef(this ParameterSyntax parameter)
    {
        return parameter.Modifiers.Any(SyntaxKind.RefKeyword);
    }

    public static bool IsOut(this ParameterSyntax parameter)
    {
        return parameter.Modifiers.Any(SyntaxKind.OutKeyword);
    }

    public static bool HasAccessibility(this MemberDeclarationSyntax member, Accessibility accessibility, ICompilationProvider context)
    {
        var symbol = member.GetDeclaredSymbol<ISymbol>(context);
        return symbol.HasAccessibility(accessibility);
    }

    public static bool HasAccessibility(this AccessorDeclarationSyntax accessor, Accessibility accessibility, ICompilationProvider context)
    {
        var symbol = accessor.GetDeclaredSymbol<ISymbol>(context);
        return symbol.HasAccessibility(accessibility);
    }

    public static Accessibility GetAccessibility(this MemberDeclarationSyntax member, ICompilationProvider context)
    {
        return GetDeclaredSymbol(member, context)!.DeclaredAccessibility;
    }

    public static Accessibility GetAccessibility(this AccessorDeclarationSyntax accessor, ICompilationProvider context)
    {
        return accessor.GetDeclaredSymbol<ISymbol>(context).DeclaredAccessibility;
    }

    public static long GetEnumValue(this EnumMemberDeclarationSyntax node, ICompilationProvider context)
    {
        var symbol = (IFieldSymbol)node.GetDeclaredSymbol(context)!;
        return Convert.ToInt64(symbol.ConstantValue);
    }

    public static bool IsConst(this BaseFieldDeclarationSyntax field, ICompilationProvider context)
    {
        var fieldSymbol = (IFieldSymbol)GetDeclaredSymbol(field, context)!;
        return fieldSymbol.IsConst;
    }

    public static bool IsStatic(this MemberDeclarationSyntax field, ICompilationProvider context)
    {
        var fieldSymbol = GetDeclaredSymbol(field, context)!;
        return fieldSymbol.IsStatic;
    }

    /// <summary>
    /// Return all explicit and implicit modifiers
    /// </summary>
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
                case SyntaxKind.StaticKeyword:
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

    /// <summary>
    /// Return all explicit and implicit modifiers
    /// </summary>
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

    /// <summary>
    /// Return all explicit and implicit modifiers
    /// </summary>
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
                case SyntaxKind.PartialKeyword:
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

    /// <summary>
    /// Return all explicit and implicit modifiers
    /// </summary>
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

    /// <summary>
    /// Return all explicit and implicit modifiers
    /// </summary>
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

    public static string GetName(this DelegateDeclarationSyntax node)
    {
        return node.Identifier.Text;
    }

    public static string GetName(this MethodDeclarationSyntax node)
    {
        return node.Identifier.Text;
    }

    public static string[] SplitCamelCase(this string str)
    {
        var splitCamelCase = GetSplitCamelCaseRegex();
        return splitCamelCase.Split(str);
    }

    static Regex GetSplitCamelCaseRegex()
    {
        if (_splitCamelCase != null)
            return _splitCamelCase;

        lock (typeof(CSharpMethodExtensions))
        {
            if (_splitCamelCase != null)
                return _splitCamelCase;

            // https://stackoverflow.com/a/7594052/213871
            _splitCamelCase = new Regex(@"(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])", RegexOptions.Compiled);
            return _splitCamelCase;
        }
    }


}
