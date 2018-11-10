using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class ExpressiontWriter<TExpression> : ContextWriter<TExpression>
        where TExpression : ExpressionSyntax
    {
        public ExpressiontWriter(TExpression syntax, ICompilationContextProvider context)
            : base(syntax, context) { }
    }

    class AssignmentExpressionWriter : ExpressiontWriter<AssignmentExpressionSyntax>
    {
        public AssignmentExpressionWriter(AssignmentExpressionSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            if (Context.Left.Kind() == SyntaxKind.IdentifierName)
            {
                var symbol = Context.Left.GetSymbolInfo(this);
                if (symbol.Symbol.Kind == SymbolKind.Property)
                {
                    var operatorKind = Context.OperatorToken.Kind();
                    switch (operatorKind)
                    {
                        case SyntaxKind.EqualsToken:
                            Builder.Append("set").Append((Context.Left as IdentifierNameSyntax).Identifier.Text)
                                .Append("(").Append(Context.Right.GetWriter(this)).Append(")");
                            break;
                        default:
                            break;
                    }
                    return;
                }
            }

            Builder.Append(Context.Left.GetWriter(this));
            Builder.Space().Append(Context.OperatorToken.Text).Space();
            Builder.Append(Context.Right.GetWriter(this));
        }
    }

    class IdenfitiferNameWriter : ExpressiontWriter<IdentifierNameSyntax>
    {
        public IdenfitiferNameWriter(IdentifierNameSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            var symbol = Context.GetSymbolInfo(this);
            Builder.Append(Context.Identifier.Text);
        }
    }
}
