﻿// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

static partial class TypeScriptExtensions
{
    public static bool HasReplacementWriters(this StatementSyntax statement,
        TypeScriptCompilationContext context, [NotNullWhen(true)]out IEnumerable<CodeWriter>? writers)
    {
        switch (statement.StatementKind())
        {
            case StatementKind.Expression:
            {
                return hasReplacementWriters((ExpressionStatementSyntax)statement, context, out writers);
            }
            case StatementKind.LocalDeclaration:
            {
                return hasReplacementWriters((LocalDeclarationStatementSyntax)statement, context, out writers);
            }
            case StatementKind.Return:
            {
                return hasReplacementWriters((ReturnStatementSyntax)statement, context, out writers);
            }
            default:
            {
                writers = null;
                return false;
            }
        }
    }

    static bool hasReplacementWriters(ReturnStatementSyntax statement, TypeScriptCompilationContext context,
        [NotNullWhen(true)]out IEnumerable<CodeWriter>? writers)
    {
        if (statement.Expression == null)
            goto NoReplacement;

        switch (statement.Expression.ExpressionKind())
        {
            case ExpressionKind.Invocation:
            {
                var invocation = (InvocationExpressionSyntax)statement.Expression;
                if (hasReplacementWriters(invocation, statement, context, out writers))
                    return true;

                break;
            }
        }

    NoReplacement:
        writers = null;
        return false;
    }

    static bool hasReplacementWriters(ExpressionStatementSyntax statement, TypeScriptCompilationContext context,
        [NotNullWhen(true)]out IEnumerable<CodeWriter>? writers)
    {
        switch (statement.Expression.ExpressionKind())
        {
            case ExpressionKind.Assignment:
            {
                // Look for invocation expression in the assignment right hand 
                var assigment = (AssignmentExpressionSyntax)statement.Expression;
                if (assigment.Right.ExpressionKind() == ExpressionKind.Invocation)
                {
                    var invocation = (InvocationExpressionSyntax)assigment.Right;
                    if (hasReplacementWriters(invocation, statement, context, out writers))
                        return true;
                }

                break;
            }
            case ExpressionKind.Invocation:
            {
                var invocation = (InvocationExpressionSyntax)statement.Expression;
                if (hasReplacementWriters(invocation, statement, context, out writers))
                    return true;

                break;
            }
        }

        writers = null;
        return false;
    }

    static bool hasReplacementWriters(LocalDeclarationStatementSyntax statement, TypeScriptCompilationContext context,
        [NotNullWhen(true)]out IEnumerable<CodeWriter>? writers)
    {
        Debug.Assert(statement.Declaration.Variables.Count == 1);
        var variable = statement.Declaration.Variables[0];
        if (variable.Initializer != null && variable.Initializer.Value.IsKind(SyntaxKind.InvocationExpression))
        {
            return hasReplacementWriters((InvocationExpressionSyntax)variable.Initializer.Value,
                statement, context, out writers);
        }

        writers = null;
        return false;
    }

    static bool hasReplacementWriters(InvocationExpressionSyntax invocation, StatementSyntax statement,
        TypeScriptCompilationContext context, [NotNullWhen(true)]out IEnumerable<CodeWriter>? writers)
    {
        var refArguments = getRefArguments(invocation, context);
        foreach (var arg in refArguments)
        {
            if (arg.Type.TypeKind == TypeKind.Enum || arg.Type.IsCLRPrimitiveType())
            {
                writers = getReplacementWriters(invocation, statement, refArguments, context);
                return true;
            }
        }

        writers = null;
        return false;
    }

    static IEnumerable<CodeWriter> getReplacementWriters(InvocationExpressionSyntax invocation,
        StatementSyntax statement, List<RefArgument> refArguments, TypeScriptCompilationContext context)
    {
        var writers = new List<CodeWriter>();
        foreach (var arg in refArguments)
        {
            string boxType;
            if (arg.Type.TypeKind == TypeKind.Enum)
                boxType = "IntegerBox";
            else
                boxType = TypeScriptUtils.GetRefBoxType(arg.Type.GetFullName());

            writers.Add(CodeWriter.Create((builder) => {
                builder.Append(boxType).Space().Append("__" + arg.Symbol.Name).Space().Append("=").Space()
                    .Append("new").Space().Append(boxType).EmptyParameterList().SemiColon();
            }));
        }

        if (statement.StatementKind() == StatementKind.Return)
        {
            writers.Add(CodeWriter.Create((builder) =>
            {
                var method = invocation.GetSymbol<IMethodSymbol>(context);
                var declaration = method.GetDeclarationSyntax()!;
                builder.Append(declaration.ReturnType, context).Space().Append("__ret").Space()
                    .Append("=").Space().Append(invocation, context).SemiColon(); 
            }));
        }
        else
        {
            writers.Add(CodeWriter.Create((builder) => builder.Append(statement, context)));
        }

        foreach (var arg in refArguments)
        {
            writers.Add(CodeWriter.Create((builder) => {
                builder.Append(arg.Symbol.Name).Space().Append("=").Space();

                void appendAssingmentRHS()
                {
                    builder.Append("__").Append(arg.Symbol.Name).Dot().Append("value");
                }

                if (arg.Type.TypeKind == TypeKind.Enum)
                    builder.Append(arg.Type.Name).Dot().Append("fromValue").Parenthesized(() => appendAssingmentRHS());
                else
                    appendAssingmentRHS();

                builder.SemiColon();
            }));
        }

        if (statement.StatementKind() == StatementKind.Return)
            writers.Add(CodeWriter.Create((builder) => builder.Append("return").Space().Append("__ret").SemiColon()));

        return writers;
    }

    static List<RefArgument> getRefArguments(InvocationExpressionSyntax invocation, TypeScriptCompilationContext context)
    {
        var ret = new List<RefArgument>();
        foreach (var arg in invocation.ArgumentList.Arguments)
        {
            if (!arg.RefKindKeyword.IsNone())
            {
                var symbol = arg.Expression.GetSymbol(context)!;
                ITypeSymbol type;
                switch (symbol.Kind)
                {
                    case SymbolKind.Local:
                    {
                        type = (symbol as ILocalSymbol)!.Type;
                        break;
                    }
                    case SymbolKind.Parameter:
                    {
                        type = (symbol as IParameterSymbol)!.Type;
                        break;
                    }
                    default:
                        throw new Exception();
                }

                ret.Add(new RefArgument() { Argument = arg, Symbol = symbol, Type = type });
            }
        }

        return ret;
    }

    struct RefArgument
    {
        public ArgumentSyntax Argument;
        public ISymbol Symbol;
        public ITypeSymbol Type;
    }
}