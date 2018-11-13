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

    class BlockStatementWriter : StatementWriter<BlockSyntax>
    {
        public BlockStatementWriter(BlockSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            using (Builder.BeginBlock(false))
            {
                foreach (var statement in Context.Statements)
                {
                    Builder.Append(statement.GetWriter(this)).EndOfLine();
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
            Builder.Append("NULL");
        }
    }

    class ForEachStatementWriter : StatementWriter<ForEachStatementSyntax>
    {
        public ForEachStatementWriter(ForEachStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ForEachVariableStatementWriter : StatementWriter<ForEachVariableStatementSyntax>
    {
        public ForEachVariableStatementWriter(ForEachVariableStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ContinueStatementWriter : StatementWriter<ContinueStatementSyntax>
    {
        public ContinueStatementWriter(ContinueStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }
    class DoStatementWriter : StatementWriter<DoStatementSyntax>
    {
        public DoStatementWriter(DoStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class EmptyStatementWriter : StatementWriter<EmptyStatementSyntax>
    {
        public EmptyStatementWriter(EmptyStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ForStatementWriter : StatementWriter<ForStatementSyntax>
    {
        public ForStatementWriter(ForStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class IfStatementWriter : StatementWriter<IfStatementSyntax>
    {
        public IfStatementWriter(IfStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class LocalDeclarationStatementWriter : StatementWriter<LocalDeclarationStatementSyntax>
    {
        public LocalDeclarationStatementWriter(LocalDeclarationStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class LockStatementWriter : StatementWriter<LockStatementSyntax>
    {
        public LockStatementWriter(LockStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ReturnStatementWriter : StatementWriter<ReturnStatementSyntax>
    {
        public ReturnStatementWriter(ReturnStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class SwitchStatementWriter : StatementWriter<SwitchStatementSyntax>
    {
        public SwitchStatementWriter(SwitchStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }

    class ThrowStatementWriter : StatementWriter<ThrowStatementSyntax>
    {
        public ThrowStatementWriter(ThrowStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
        }
    }


    class TryStatementWriter : StatementWriter<TryStatementSyntax>
    {
        public TryStatementWriter(TryStatementSyntax syntax, ICompilationContextProvider context)
            : base(syntax, context) { }

        protected override void Write()
        {
            Builder.Append("NULL");
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
            Builder.Append("NULL");
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
