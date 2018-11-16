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
            Builder.Append(Context.Expression, this);
        }
    }

    class BlockStatementWriter : StatementWriter<BlockSyntax>
    {
        public BlockStatementWriter(BlockSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            // Allows to not doubly indent single statements blocks, e.g after "if" statement
            Builder.ResetChildIndent();
            using (Builder.Block(false))
            {
                foreach (var statement in Context.Statements)
                {
                    Builder.Append(statement, this).AppendLine();
                }
            }
        }
    }

    class BreakStatementWriter : StatementWriter<BreakStatementSyntax>
    {
        public BreakStatementWriter(BreakStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("break").SemiColon();
        }
    }

    class ForEachStatementWriter : StatementWriter<ForEachStatementSyntax>
    {
        public ForEachStatementWriter(ForEachStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("for").Space().Parenthesized().Append(Context.Type, this).Space().Append(Context.Identifier.Text)
                .Space().Colon().Space().Append(Context.Expression, this).Close().AppendLine();
            Builder.IndentChild().Append(Context.Statement, this);
        }
    }

    class ContinueStatementWriter : StatementWriter<ContinueStatementSyntax>
    {
        public ContinueStatementWriter(ContinueStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("continue").SemiColon();
        }
    }
    class DoStatementWriter : StatementWriter<DoStatementSyntax>
    {
        public DoStatementWriter(DoStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("do").AppendLine();
            Builder.IndentChild().Append(Context.Statement, this);
            Builder.Append("while").Space().Parenthesized().Append(Context.Condition, this).Close().SemiColon();
        }
    }

    class EmptyStatementWriter : StatementWriter<EmptyStatementSyntax>
    {
        public EmptyStatementWriter(EmptyStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.SemiColon();
        }
    }

    class ForStatementWriter : StatementWriter<ForStatementSyntax>
    {
        public ForStatementWriter(ForStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("for").Space().Parenthesized(() =>
            {
                Builder.Append(Context.Declaration, this).SemiColonSeparator()
                .Append(Context.Condition, this).SemiColonSeparator();

                bool first = true;
                foreach (var incrementor in Context.Incrementors)
                {
                    if (first)
                        first = true;
                    else
                        Builder.CommaSeparator();

                    Builder.Append(incrementor, this);
                }
            }).AppendLine();
            Builder.IndentChild().Append(Context.Statement, this);
        }
    }

    class IfStatementWriter : StatementWriter<IfStatementSyntax>
    {
        public IfStatementWriter(IfStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("if").Space().Parenthesized().Append(Context.Condition, this).Close().AppendLine();
            Builder.IndentChild().Append(Context.Statement, this);
            if (Context.Else != null)
                Builder.Append(Context.Else, this);
        }
    }

    class LocalDeclarationStatementWriter : StatementWriter<LocalDeclarationStatementSyntax>
    {
        public LocalDeclarationStatementWriter(LocalDeclarationStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            if (Context.IsConst)
                Builder.Append("final").Space();

            Builder.Append(Context.Declaration, this).SemiColon();
        }
    }

    class LockStatementWriter : StatementWriter<LockStatementSyntax>
    {
        public LockStatementWriter(LockStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("synchronized").Space().Parenthesized().Append(Context.Expression, this).Close().AppendLine();
            Builder.IndentChild().Append(Context.Statement, this);
        }
    }

    class ReturnStatementWriter : StatementWriter<ReturnStatementSyntax>
    {
        public ReturnStatementWriter(ReturnStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("return");
            if (Context.Expression != null)
                Builder.Space().Append(Context.Expression, this);
            Builder.SemiColon();
        }
    }

    class SwitchStatementWriter : StatementWriter<SwitchStatementSyntax>
    {
        public SwitchStatementWriter(SwitchStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("switch").Space().Parenthesized().Append(Context.Expression, this).Close().AppendLine();
            using (Builder.Block(false))
            {
                bool first = true;
                foreach (var section in Context.Sections)
                {
                    if (first)
                        first = true;
                    else
                        Builder.AppendLine();

                    Builder.Append(section, this);
                }
            }
        }
    }

    class ThrowStatementWriter : StatementWriter<ThrowStatementSyntax>
    {
        public ThrowStatementWriter(ThrowStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("throw").Space().Append(Context.Expression, this).SemiColon();
        }
    }


    class TryStatementWriter : StatementWriter<TryStatementSyntax>
    {
        public TryStatementWriter(TryStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("try").AppendLine();
            Builder.Append(Context.Block, this).AppendLine();
            bool first = true;
            foreach (var catchClause in Context.Catches)
            {
                if (first)
                    first = true;
                else
                    Builder.AppendLine();

                Builder.Append(catchClause, this);
            }
            if (Context.Finally != null)
                Builder.Append(Context.Finally, this);
        }
    }


    class UsingStatementWriter : StatementWriter<UsingStatementSyntax>
    {
        public UsingStatementWriter(UsingStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class WhileStatementWriter : StatementWriter<WhileStatementSyntax>
    {
        public WhileStatementWriter(WhileStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("while").Space().Append(Context.Condition, this).AppendLine()
                .IndentChild().Append(Context.Statement, this);
        }
    }

    class YieldStatementWriter : StatementWriter<YieldStatementSyntax>
    {
        public YieldStatementWriter(YieldStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }
}
