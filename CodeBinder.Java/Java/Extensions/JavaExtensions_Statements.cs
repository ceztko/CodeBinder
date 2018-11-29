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
    static partial class JavaExtensions
    {
        public static bool HasReplacementWriter(this StatementSyntax statement,
            ICompilationContextProvider context, out IEnumerable<CodeWriter> writers)
        {
            switch (statement.StatementKind())
            {
                case StatementKind.Expression:
                {
                    return hasReplacementWriters(statement as ExpressionStatementSyntax, context, out writers);
                }
                case StatementKind.LocalDeclaration:
                {
                    return hasReplacementWriters(statement as LocalDeclarationStatementSyntax, context, out writers);
                }
                default:
                {
                    writers = null;
                    return false;
                }
            }
        }

        static bool hasReplacementWriters(ExpressionStatementSyntax statement, ICompilationContextProvider context,
          out IEnumerable<CodeWriter> writers)
        {
            switch (statement.Expression.ExpressionKind())
            {
                case ExpressionKind.Assignment:
                {
                    // Look for invocation expression in the assignment right hand 
                    var assigment = statement.Expression as AssignmentExpressionSyntax;
                    if (assigment.Right.ExpressionKind() == ExpressionKind.Invocation)
                    {
                        var invocation = assigment.Right as InvocationExpressionSyntax;
                        if (hasReplacementWriters(invocation, statement, context, out writers))
                            return true;
                    }

                    break;
                }
                case ExpressionKind.Invocation:
                {
                    var invocation = statement.Expression as InvocationExpressionSyntax;
                    if (hasReplacementWriters(invocation, statement, context, out writers))
                        return true;

                    break;
                }
            }

            writers = null;
            return false;
        }

        static bool hasReplacementWriters(LocalDeclarationStatementSyntax statement, ICompilationContextProvider context,
          out IEnumerable<CodeWriter> writers)
        {
            Debug.Assert(statement.Declaration.Variables.Count == 1);
            var variable = statement.Declaration.Variables[0];
            if (variable.Initializer != null && variable.Initializer.Value.IsKind(SyntaxKind.InvocationExpression))
            {
                return hasReplacementWriters(variable.Initializer.Value as InvocationExpressionSyntax,
                    statement, context, out writers);
            }

            writers = null;
            return false;
        }

        static bool hasReplacementWriters(InvocationExpressionSyntax invocation, StatementSyntax statement,
            ICompilationContextProvider context, out IEnumerable<CodeWriter> writers)
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
            StatementSyntax statement, List<RefArgument> refArguments, ICompilationContextProvider context)
        {
            var writers = new List<CodeWriter>();
            foreach (var arg in refArguments)
            {
                string boxType;
                if (arg.Type.TypeKind == TypeKind.Enum)
                    boxType = "IntegerBox";
                else
                    boxType = JavaUtils.GetJavaRefBoxType(arg.Type.GetFullName());

                writers.Add(CodeWriter.Create((builder) => {
                    builder.Append(boxType).Space().Append("__" + arg.Symbol.Name).Space().Append("=").Space()
                        .Append("new").Space().Append(boxType).EmptyParameterList().SemiColon();
                }));
            }

            writers.Add(CodeWriter.Create((builder) => builder.Append(statement, context)));

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

            return writers;
        }

        static List<RefArgument> getRefArguments(InvocationExpressionSyntax invocation, ICompilationContextProvider context)
        {
            var ret = new List<RefArgument>();
            foreach (var arg in invocation.ArgumentList.Arguments)
            {
                if (!arg.RefKindKeyword.IsNone())
                {
                    var symbol = arg.Expression.GetSymbol(context);
                    ITypeSymbol type;
                    switch (symbol.Kind)
                    {
                        case SymbolKind.Local:
                        {
                            type = (symbol as ILocalSymbol).Type;
                            break;
                        }
                        case SymbolKind.Parameter:
                        {
                            type = (symbol as IParameterSymbol).Type;
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
}
