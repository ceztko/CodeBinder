using CodeBinder.Shared;
using CodeBinder.Shared.CSharp;
using CodeBinder.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace CodeBinder.Apple
{
    static partial class ObjCBuilderExtension
    {
        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static CodeBuilder Append(this CodeBuilder builder, StatementSyntax statement, ObjCCompilationContext context)
        {
            var kind = statement.Kind();
            switch (kind)
            {
                case SyntaxKind.Block:
                    return builder.Append((BlockSyntax)statement, context);
                case SyntaxKind.BreakStatement:
                    return builder.Append((BreakStatementSyntax)statement, context);
                case SyntaxKind.ForEachStatement:
                    return builder.Append((ForEachStatementSyntax)statement, context);
                case SyntaxKind.ContinueStatement:
                    return builder.Append((ContinueStatementSyntax)statement, context);
                case SyntaxKind.DoStatement:
                    return builder.Append((DoStatementSyntax)statement, context);
                case SyntaxKind.EmptyStatement:
                    return builder.Append((EmptyStatementSyntax)statement, context);
                case SyntaxKind.ExpressionStatement:
                    return builder.Append((ExpressionStatementSyntax)statement, context);
                case SyntaxKind.ForStatement:
                    return builder.Append((ForStatementSyntax)statement, context);
                case SyntaxKind.IfStatement:
                    return builder.Append((IfStatementSyntax)statement, context);
                case SyntaxKind.LocalDeclarationStatement:
                    return builder.Append((LocalDeclarationStatementSyntax)statement, context);
                case SyntaxKind.LockStatement:
                    return builder.Append((LockStatementSyntax)statement, context);
                case SyntaxKind.ReturnStatement:
                    return builder.Append((ReturnStatementSyntax)statement, context);
                case SyntaxKind.SwitchStatement:
                    return builder.Append((SwitchStatementSyntax)statement, context);
                case SyntaxKind.ThrowStatement:
                    return builder.Append((ThrowStatementSyntax)statement, context);
                case SyntaxKind.TryStatement:
                    return builder.Append((TryStatementSyntax)statement, context);
                case SyntaxKind.UsingStatement:
                    return builder.Append((UsingStatementSyntax)statement, context);
                case SyntaxKind.WhileStatement:
                    return builder.Append((WhileStatementSyntax)statement, context);
                // Unsupported statements
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UnsafeStatement:
                case SyntaxKind.LabeledStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.LocalFunctionStatement:
                case SyntaxKind.ForEachVariableStatement:
                // Unsupported yield statements
                case SyntaxKind.YieldBreakStatement:
                case SyntaxKind.YieldReturnStatement:
                // Unsupported goto statements
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                default:
                    throw new NotSupportedException();
            }
        }

        #region Statements

        public static CodeBuilder Append(this CodeBuilder builder, BlockSyntax syntax, ObjCCompilationContext context, bool skipBraces = false)
        {
            if (skipBraces)
            {
                builder.Append(syntax.Statements, context);
            }
            else
            {
                // Allows to not doubly indent single statements blocks, e.g after "if" statement
                builder.ResetChildIndent();
                using (builder.Block(false))
                {
                    builder.Append(syntax.Statements, context).AppendLine();
                }
            }

            return builder;
        }

        static CodeBuilder Append(this CodeBuilder builder, IEnumerable<StatementSyntax> staments, ObjCCompilationContext context)
        {
            bool first = true;
            foreach (var statement in staments)
                builder.AppendLine(ref first).Append(statement, context);

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, BreakStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("break").SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ForEachStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("for").Space().Parenthesized().Append(syntax.Type, ObjCTypeUsageKind.Declaration, context).
                Space().Append(syntax.Identifier.Text).Space().Append("in").Space().Append(syntax.Expression, context).Close().AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ContinueStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("continue").SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, DoStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("do").AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            builder.Append("while").Space().Parenthesized().Append(syntax.Condition, context).Close().SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, EmptyStatementSyntax syntax, ObjCCompilationContext context)
        {
            return builder.SemiColon();
        }

        public static CodeBuilder Append(this CodeBuilder builder, ExpressionStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append(syntax.Expression, context).SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ForStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("for").Space().Parenthesized((builder) =>
            {
                if (syntax.Declaration != null)
                    builder.Append(syntax.Declaration, context);
                builder.SemiColonSeparator();
                if (syntax.Condition != null)
                    builder.Append(syntax.Condition, context);
                builder.SemiColonSeparator().Append(syntax.Incrementors, true, context);
            }).AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, IfStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("if").Space().Parenthesized().Append(syntax.Condition, context).Close()!.AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            if (syntax.Else != null)
                builder.AppendLine().Append(syntax.Else, context);

            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, LocalDeclarationStatementSyntax syntax, ObjCCompilationContext context)
        {
            if (syntax.IsConst)
                builder.Append("const").Space();

            builder.Append(syntax.Declaration, context).SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, LockStatementSyntax syntax, ObjCCompilationContext context)
        {
            // https://developer.apple.com/library/archive/documentation/Cocoa/Conceptual/Multithreading/ThreadSafety/ThreadSafety.html
            builder.Append("@synchronized").Space().Parenthesized().Append(syntax.Expression, context).Close().AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, ReturnStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("return");
            if (syntax.Expression != null)
                builder.Space().Append(syntax.Expression, context);
            builder.SemiColon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, SwitchStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("switch").Space().Parenthesized().Append(syntax.Expression, context).Close().AppendLine();
            using (builder.Block(false))
            {
                bool first = true;
                foreach (var section in syntax.Sections)
                    builder.AppendLine(ref first).Append(section, context);
                builder.AppendLine();
            }
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, WhileStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("while").Space().Append(syntax.Condition, context).AppendLine()
                .IndentChild().Append(syntax.Statement, context);
            return builder;
        }


        #endregion // Statements

        #region Exception Handling

        // https://developer.apple.com/library/archive/documentation/Cocoa/Conceptual/Exceptions/Tasks/HandlingExceptions.html
        public static CodeBuilder Append(this CodeBuilder builder, ThrowStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("@throw");
            if (syntax.Expression != null)
                builder.Space().Append(syntax.Expression, context);

            return builder.SemiColon();
        }

        public static CodeBuilder Append(this CodeBuilder builder, TryStatementSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("@try").AppendLine();
            builder.Append(syntax.Block, context).AppendLine();
            bool first = true;
            foreach (var catchClause in syntax.Catches)
                builder.AppendLine(ref first).Append(catchClause, context);

            if (syntax.Finally != null)
                builder.AppendLine().Append(syntax.Finally, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, UsingStatementSyntax syntax, ObjCCompilationContext context)
        {
            Debug.Assert(syntax.Expression == null);
            Debug.Assert(syntax.Declaration != null);
            builder.Append(syntax.Declaration, context).Close().EndOfStatement();
            builder.Append("@try");
            using (builder.Block())
            {
                builder.Append(syntax.Statement, context);
            }
            builder.Append("@finally");
            using (builder.Block())
            {
                builder.Bracketed().Append(syntax.Declaration.Variables[0].Identifier.Text).Space().Append("dispose").Close();
            }
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, FinallyClauseSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("@finally").AppendLine().Append(syntax.Block, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, CatchClauseSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("@catch").Space();
            if (syntax.Declaration == null)
                builder.Parenthesized().Append("Exception e").Close().AppendLine();
            else
                builder.Parenthesized().Append(syntax.Declaration.Type, context).Space().Append(syntax.Declaration.Identifier.Text).Close().AppendLine();

            builder.Append(syntax.Block, context);
            return builder;
        }

        #endregion // Exception Handling

        #region Support

        public static CodeBuilder Append(this CodeBuilder builder, ElseClauseSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("else").AppendLine();
            builder.IndentChild().Append(syntax.Statement, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, VariableDeclarationSyntax syntax, ObjCCompilationContext context)
        {
            Debug.Assert(syntax.Variables.Count == 1);
            builder.Append(syntax.Type, ObjCTypeUsageKind.Declaration, context).Space().Append(syntax.Variables[0], context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, VariableDeclaratorSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append(syntax.Identifier.Text);
            if (syntax.Initializer != null)
                builder.Space().Append(syntax.Initializer, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, EqualsValueClauseSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append("=").Space().Append(syntax.Value, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, SwitchSectionSyntax syntax, ObjCCompilationContext context)
        {
            foreach (var label in syntax.Labels)
                builder.Append(label, context).AppendLine();

            builder.IndentChild().Append(syntax.Statements, context);
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, SwitchLabelSyntax syntax, ObjCCompilationContext context)
        {
            builder.Append(syntax.Keyword.Text);
            switch (syntax.Kind())
            {
                case SyntaxKind.DefaultSwitchLabel:
                    break;
                case SyntaxKind.CaseSwitchLabel:
                    builder.Space().Append((CaseSwitchLabelSyntax)syntax, context);
                    break;
                default:
                    throw new Exception();
            }
            builder.Colon();
            return builder;
        }

        public static CodeBuilder Append(this CodeBuilder builder, CaseSwitchLabelSyntax syntax, ObjCCompilationContext context)
        {
            if (syntax.TryGetDistinctObjCName(context, out var name))
            {
                builder.Append(name);
                return builder;
            }

            builder.Append(syntax.Value, context);
            return builder;
        }

        #endregion // Support
    }
}
