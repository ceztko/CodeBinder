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

namespace CodeBinder.Java
{
    static partial class JavaExtensions
    {
        public static bool HasReplacementWriter(this StatementSyntax member, ICompilationContextProvider context, out IEnumerable<CodeWriter> writers)
        {
            if (!member.IsKind(SyntaxKind.ExpressionStatement))
            {
                writers = null;
                return false;
            }

            var expressionStatement = member as ExpressionStatementSyntax;
            switch (expressionStatement.Expression.ExpressionKind())
            {
                case ExpressionKind.Assignment:
                    break;
                case ExpressionKind.Invocation:
                    break;
            }

            writers = null;
            return false;
        }
    }
}
