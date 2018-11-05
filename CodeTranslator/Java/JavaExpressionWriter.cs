using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    class BlockWriter : SyntaxWriter<BlockSyntax>
    {
        public BlockWriter(BlockSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            using (Builder.BeginBlock())
            {
                foreach (var expression in Syntax.Statements)
                {
                    var kind = expression.Kind();
                    ISyntaxWriter writer = null;
                    switch (kind)
                    {
                        case SyntaxKind.ExpressionStatement:
                            writer = new ExpressionStamentWriter(expression as ExpressionStatementSyntax, this);
                            break;
                        default:
                            continue; // CHANGE-ME
                    }
                    writer.Write(Builder);
                }
            }
        }
    }

    abstract class StatementWriter<TStatement> : SyntaxWriter<TStatement>
        where TStatement : StatementSyntax
    {
        public StatementWriter(TStatement syntax, ICompilationContextProvider context)
            : base(syntax, context) { }
    }

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

        }
    }

    class ExpressionStamentWriter : StatementWriter<ExpressionStatementSyntax>
    {
        public ExpressionStamentWriter(ExpressionStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            var kind = Syntax.Expression.Kind();
            ISyntaxWriter writer = null;
            switch (kind)
            {
                case SyntaxKind.SimpleAssignmentExpression:
                    writer = new AssignmentExpressionWriter(Syntax.Expression as AssignmentExpressionSyntax, this);
                    break;
                default:
                    return; // CHANGE-ME
            }

            writer.Write(Builder);
        }
    }
}
