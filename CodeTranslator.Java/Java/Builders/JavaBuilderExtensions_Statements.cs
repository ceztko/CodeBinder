using CodeTranslator.Shared;
using CodeTranslator.Shared.CSharp;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Shared.Java;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace CodeTranslator.Java
{
    static partial class JavaWriterExtension
    {
        public static CodeBuilder Append(this CodeBuilder builder, BlockSyntax syntax, ICompilationContextProvider context, bool skipBraces = false)
        {
            void writeStatements()
            {
                foreach (var statement in syntax.Statements)
                {
                    builder.Append(statement, context).AppendLine();
                }
            }

            if (skipBraces)
            {
                writeStatements();
            }
            else
            {
                // Allows to not doubly indent single statements blocks, e.g after "if" statement
                builder.ResetChildIndent();
                using (builder.Block(false))
                {
                    writeStatements();
                }
            }

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, BreakStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("break").SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ForEachStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("for").Space().Parenthesized().Append(syntax.Type, context).Space().Append(syntax.Identifier.Text)
                .Space().Colon().Space().Append(syntax.Expression, context).Close().AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ContinueStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("continue").SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, DoStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("do").AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            builder.Append("while").Space().Parenthesized().Append(syntax.Condition, context).Close().SemiColon();

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, EmptyStatementSyntax syntax, ICompilationContextProvider context)
        {
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ExpressionStatementSyntax syntax, ICompilationContextProvider context)
        {

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ForStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("for").Space().Parenthesized(() =>
            {
                builder.Append(syntax.Declaration, context).SemiColonSeparator()
                .Append(syntax.Condition, context).SemiColonSeparator();

                bool first = true;
                foreach (var incrementor in syntax.Incrementors)
                {
                    if (first)
                        first = true;
                    else
                        builder.CommaSeparator();

                    builder.Append(incrementor, context);
                }
            }).AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, IfStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("if").Space().Parenthesized().Append(syntax.Condition, context).Close().AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            if (syntax.Else != null)
                builder.Append(syntax.Else, context);

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, LocalDeclarationStatementSyntax syntax, ICompilationContextProvider context)
        {
            if (syntax.IsConst)
                builder.Append("final").Space();

            builder.Append(syntax.Declaration, context).SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, LockStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("synchronized").Space().Parenthesized().Append(syntax.Expression, context).Close().AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ReturnStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("return");
            if (syntax.Expression != null)
                builder.Space().Append(syntax.Expression, context);
            builder.SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, SwitchStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("switch").Space().Parenthesized().Append(syntax.Expression, context).Close().AppendLine();
            using (builder.Block(false))
            {
                bool first = true;
                foreach (var section in syntax.Sections)
                {
                    if (first)
                        first = true;
                    else
                        builder.AppendLine();

                    builder.Append(section, context);
                }
            }
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThrowStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("throw").Space().Append(syntax.Expression, context).SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, TryStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("try").AppendLine();
            builder.Append(syntax.Block, context).AppendLine();
            bool first = true;
            foreach (var catchClause in syntax.Catches)
            {
                if (first)
                    first = true;
                else
                    builder.AppendLine();

                builder.Append(catchClause, context);
            }
            if (syntax.Finally != null)
                builder.Append(syntax.Finally, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, UsingStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("try").Space().Parenthesized().Append(syntax.Declaration, context).Close().AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, WhileStatementSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("while").Space().Append(syntax.Condition, context).AppendLine()
                .IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static CodeBuilder Append(this CodeBuilder builder, StatementSyntax statement, ICompilationContextProvider context)
        {
            var kind = statement.Kind();
            switch (kind)
            {
                case SyntaxKind.Block:
                    return builder.Append(statement as BlockSyntax, context);
                case SyntaxKind.BreakStatement:
                    return builder.Append(statement as BreakStatementSyntax, context);
                case SyntaxKind.ForEachStatement:
                    return builder.Append(statement as ForEachStatementSyntax, context);
                case SyntaxKind.ContinueStatement:
                    return builder.Append(statement as ContinueStatementSyntax, context);
                case SyntaxKind.DoStatement:
                    return builder.Append(statement as DoStatementSyntax, context);
                case SyntaxKind.EmptyStatement:
                    return builder.Append(statement as EmptyStatementSyntax, context);
                case SyntaxKind.ExpressionStatement:
                    return builder.Append(statement as ExpressionStatementSyntax, context);
                case SyntaxKind.ForStatement:
                    return builder.Append(statement as ForStatementSyntax, context);
                case SyntaxKind.IfStatement:
                    return builder.Append(statement as IfStatementSyntax, context);
                case SyntaxKind.LocalDeclarationStatement:
                    return builder.Append(statement as LocalDeclarationStatementSyntax, context);
                case SyntaxKind.LockStatement:
                    return builder.Append(statement as LockStatementSyntax, context);
                case SyntaxKind.ReturnStatement:
                    return builder.Append(statement as ReturnStatementSyntax, context);
                case SyntaxKind.SwitchStatement:
                    return builder.Append(statement as SwitchStatementSyntax, context);
                case SyntaxKind.ThrowStatement:
                    return builder.Append(statement as ThrowStatementSyntax, context);
                case SyntaxKind.TryStatement:
                    return builder.Append(statement as TryStatementSyntax, context);
                case SyntaxKind.UsingStatement:
                    return builder.Append(statement as UsingStatementSyntax, context);
                case SyntaxKind.WhileStatement:
                    return builder.Append(statement as WhileStatementSyntax, context);
                default:
                    throw new Exception();
            }
        }

        public static CodeBuilder Append(this CodeBuilder builder, VariableDeclarationSyntax syntax, ICompilationContextProvider context)
        {
            Debug.Assert(syntax.Variables.Count == 1);
            builder.Append(syntax.Type, context).Space().Append(syntax.Variables[0], context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, VariableDeclaratorSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append(syntax.Identifier.Text);
            if (syntax.Initializer != null)
                builder.Space().Append(syntax.Initializer, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, EqualsValueClauseSyntax syntax, ICompilationContextProvider context)
        {
            builder.Append("=").Space().Append(syntax.Value, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, FinallyClauseSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(CodeWriter.NullWriter());
        }

        public static CodeBuilder Append(this CodeBuilder builder, CatchClauseSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(CodeWriter.NullWriter());
        }

        public static CodeBuilder Append(this CodeBuilder builder, SwitchSectionSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(CodeWriter.NullWriter());
        }

        public static CodeBuilder Append(this CodeBuilder builder, ElseClauseSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(CodeWriter.NullWriter());
        }
    }
}
