using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class StatementWriter<TStatement> : SyntaxWriter<TStatement>
        where TStatement : StatementSyntax
    {
        public StatementWriter(TStatement syntax, ICompilationContextProvider context)
            : base(syntax, context) { }
    }

    class ExpressionStamentWriter : StatementWriter<ExpressionStatementSyntax>
    {
        public ExpressionStamentWriter(ExpressionStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Syntax.Expression.GetWriter(this).Write(Builder);
        }
    }

    class BlockWriter : StatementWriter<BlockSyntax>
    {
        public BlockWriter(BlockSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            using (Builder.BeginBlock())
            {
                foreach (var statement in Syntax.Statements)
                {
                    statement.GetWriter(this).Write(Builder);
                    Builder.EndOfLine();
                }
            }
        }
    }

}
