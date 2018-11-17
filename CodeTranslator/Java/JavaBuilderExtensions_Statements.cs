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
        public static CodeBuilder Append(this CodeBuilder builder, BlockSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new BlockStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, BreakStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new BreakStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ForEachStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ForEachStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ContinueStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ContinueStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, DoStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new DoStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, EmptyStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new EmptyStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ExpressionStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ExpressionStamentWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ForStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ForStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, IfStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new IfStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, LocalDeclarationStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new LocalDeclarationStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, LockStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new LockStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ReturnStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ReturnStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, SwitchStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new SwitchStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ThrowStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new ThrowStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, TryStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new TryStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, UsingStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new UsingStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, WhileStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new WhileStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, YieldStatementSyntax statement, ICompilationContextProvider context)
        {
            return builder.Append(new YieldStatementWriter(statement, context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, ExpressionSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(syntax.GetWriter(context));
        }

        public static CodeBuilder Append(this CodeBuilder builder, StatementSyntax syntax, ICompilationContextProvider context)
        {
            return builder.Append(syntax.GetWriter(context));
        }

        // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
        public static CodeWriter GetWriter(this StatementSyntax statement, ICompilationContextProvider context)
        {
            var kind = statement.Kind();
            switch (kind)
            {
                case SyntaxKind.Block:
                    return new BlockStatementWriter(statement as BlockSyntax, context);
                case SyntaxKind.BreakStatement:
                    return new BreakStatementWriter(statement as BreakStatementSyntax, context);
                case SyntaxKind.ForEachStatement:
                    return new ForEachStatementWriter(statement as ForEachStatementSyntax, context);
                case SyntaxKind.ContinueStatement:
                    return new ContinueStatementWriter(statement as ContinueStatementSyntax, context);
                case SyntaxKind.DoStatement:
                    return new DoStatementWriter(statement as DoStatementSyntax, context);
                case SyntaxKind.EmptyStatement:
                    return new EmptyStatementWriter(statement as EmptyStatementSyntax, context);
                case SyntaxKind.ExpressionStatement:
                    return new ExpressionStamentWriter(statement as ExpressionStatementSyntax, context);
                case SyntaxKind.ForStatement:
                    return new ForStatementWriter(statement as ForStatementSyntax, context);
                case SyntaxKind.IfStatement:
                    return new IfStatementWriter(statement as IfStatementSyntax, context);
                case SyntaxKind.LocalDeclarationStatement:
                    return new LocalDeclarationStatementWriter(statement as LocalDeclarationStatementSyntax, context);
                case SyntaxKind.LockStatement:
                    return new LockStatementWriter(statement as LockStatementSyntax, context);
                case SyntaxKind.ReturnStatement:
                    return new ReturnStatementWriter(statement as ReturnStatementSyntax, context);
                case SyntaxKind.SwitchStatement:
                    return new SwitchStatementWriter(statement as SwitchStatementSyntax, context);
                case SyntaxKind.ThrowStatement:
                    return new ThrowStatementWriter(statement as ThrowStatementSyntax, context);
                case SyntaxKind.TryStatement:
                    return new TryStatementWriter(statement as TryStatementSyntax, context);
                case SyntaxKind.UsingStatement:
                    return new UsingStatementWriter(statement as UsingStatementSyntax, context);
                case SyntaxKind.WhileStatement:
                    return new WhileStatementWriter(statement as WhileStatementSyntax, context);
                case SyntaxKind.YieldReturnStatement:
                    return new YieldStatementWriter(statement as YieldStatementSyntax, context);
                default:
                    throw new Exception();
            }
        }

        public static CodeBuilder Append(this CodeBuilder builder, VariableDeclarationSyntax syntax, ICompilationContextProvider context)
        {
            Debug.Assert(syntax.Variables.Count == 1);
            return builder.Append(syntax.Type.GetJavaType(context)).Space()
                .Append(syntax.Variables[0], context).EndOfStatement();
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
            return builder.Append("=").Space().Append(syntax.Value, context);
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
