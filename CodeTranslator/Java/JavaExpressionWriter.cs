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
    abstract class ExpressiontWriter<TExpression> : SyntaxWriter<TExpression>
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
            if (Syntax.Left.Kind() == SyntaxKind.IdentifierName)
            {
                var symbol = Syntax.Left.GetSymbolInfo(this);
                if (symbol.Symbol.Kind == SymbolKind.Property)
                {
                    var operatorKind = Syntax.OperatorToken.Kind();
                    switch (operatorKind)
                    {
                        case SyntaxKind.EqualsToken:
                            Builder.Append("set").Append((Syntax.Left as IdentifierNameSyntax).Identifier.Text)
                                .Append("(").Append(Syntax.Right.GetWriter(this)).Append(")");
                            break;
                        default:
                            break;
                    }
                    return;
                }
            }
            Syntax.Left.GetWriter(this).Write(Builder);
            Builder.Space().Append(Syntax.OperatorToken.Text).Space();
            Syntax.Right.GetWriter(this).Write(Builder);
        }
    }

    class IdenfitiferNameWriter : ExpressiontWriter<IdentifierNameSyntax>
    {
        public IdenfitiferNameWriter(IdentifierNameSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            var symbol = Syntax.GetSymbolInfo(this);
            Builder.Append(Syntax.Identifier.Text);
        }
    }
}
