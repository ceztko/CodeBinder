using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Java
{
    abstract class StatementWriter<TStatement> : ContextWriter<TStatement>
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
            Builder.Append(Context.Expression.GetWriter(this));
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
                foreach (var statement in Context.Statements)
                {
                    Builder.Append(statement.GetWriter(this)).EndOfLine();
                }
            }
        }
    }

}
