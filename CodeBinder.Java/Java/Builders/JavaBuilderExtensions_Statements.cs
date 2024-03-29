﻿// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using CodeBinder.Java.Shared;

namespace CodeBinder.Java;

static partial class JavaBuilderExtension
{
    // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
    public static CodeBuilder Append(this CodeBuilder builder, StatementSyntax statement, JavaCodeConversionContext context)
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
            case SyntaxKind.LocalFunctionStatement:
                return builder.Append((LocalFunctionStatementSyntax)statement, context);
            // Unsupported statements
            case SyntaxKind.LockStatement:
            case SyntaxKind.CheckedStatement:
            case SyntaxKind.UnsafeStatement:
            case SyntaxKind.LabeledStatement:
            case SyntaxKind.FixedStatement:
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

    public static CodeBuilder Append(this CodeBuilder builder, BlockSyntax syntax, JavaCodeConversionContext context, bool skipBraces = false)
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

    static CodeBuilder Append(this CodeBuilder builder, IEnumerable<StatementSyntax> staments, JavaCodeConversionContext context)
    {
        bool first = true;
        foreach (var statement in staments)
            builder.AppendLine(ref first).Append(statement, context);

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, BreakStatementSyntax syntax, JavaCodeConversionContext context)
    {
        _ = syntax;
        _ = context;
        builder.Append("break").SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ForEachStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("for").Space().Parenthesized().Append(syntax.Type, context).Space().Append(syntax.Identifier.Text)
            .Space().Colon().Space().Append(syntax.Expression, context).Close().AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ContinueStatementSyntax syntax, JavaCodeConversionContext context)
    {
        _ = syntax;
        _ = context;
        builder.Append("continue").SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, DoStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("do").AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        builder.Append("while").Space().Parenthesized().Append(syntax.Condition, context).Close().SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, EmptyStatementSyntax syntax, JavaCodeConversionContext context)
    {
        _ = syntax;
        _ = context;
        return builder.SemiColon();
    }

    public static CodeBuilder Append(this CodeBuilder builder, ExpressionStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append(syntax.Expression, context).SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ForStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("for").Space().Parenthesized(() =>
        {
            if (syntax.Declaration != null)
                builder.Append(syntax.Declaration, context);
            builder.SemiColonSeparator();
            if (syntax.Condition != null)
                builder.Append(syntax.Condition, context);
            builder.SemiColonSeparator().Append(syntax.Incrementors, context);
        }).AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, IfStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("if").Space().Parenthesized().Append(syntax.Condition, context).Close().AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        if (syntax.Else != null)
            builder.AppendLine().Append(syntax.Else, context);

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, LocalDeclarationStatementSyntax syntax, JavaCodeConversionContext context)
    {
        if (syntax.IsConst)
            builder.Append("final").Space();

        builder.Append(syntax.Declaration, context).SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ReturnStatementSyntax syntax, JavaCodeConversionContext context)
    {
        if (syntax.Expression != null)
        {
            // FIXME: Workaround for ref arguments invocations. Real fix is improve
            // existing tree manipulation to also add syntetized arguments. See JavaValidationContext
            var block = syntax.FindAncestorBlock();
            if (block.Parent.IsKind(SyntaxKind.LocalFunctionStatement))
                builder.Append(syntax.Expression, context);
            else
                builder.Append("return").Space().Append(syntax.Expression, context);
        }
        else
        {
            builder.Append("return");
        }

        builder.SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, SwitchStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("switch").Space().Parenthesized().Append(syntax.Expression, context).Close().AppendLine();
        using (builder.Block(false))
        {
            bool first = true;
            foreach (var section in syntax.Sections)
                builder.AppendLine(ref first).Append(section, context);
        }
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ThrowStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("throw");
        if (syntax.Expression != null)
            builder.Space().Append(syntax.Expression, context);

        return builder.SemiColon();
    }

    public static CodeBuilder Append(this CodeBuilder builder, TryStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("try").AppendLine();
        builder.Append(syntax.Block, context).AppendLine();
        bool first = true;
        foreach (var catchClause in syntax.Catches)
            builder.AppendLine(ref first).Append(catchClause, context);

        if (syntax.Finally != null)
            builder.AppendLine().Append(syntax.Finally, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, UsingStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("try").Space().Parenthesized().Append(syntax.Declaration!, context).Close().AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, WhileStatementSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("while").Space().Parenthesized().Append(syntax.Condition, context).Close().AppendLine()
            .IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, LocalFunctionStatementSyntax syntax, JavaCodeConversionContext context)
    {
        var method = syntax.GetDeclaredSymbol<IMethodSymbol>(context);
        void writeTypeArguments()
        {
            bool first = true;
            foreach (var parameter in method.Parameters)
                builder.CommaSeparator(ref first).Append(method.ReturnType.GetJavaType());

            if (!method.ReturnsVoid)
            {
                if (method.Parameters.Length != 0)
                    builder.CommaSeparator();

                builder.Append(method.ReturnType.GetJavaType(JavaTypeFlags.TypeArgument));
            }
        }

        builder.Append($"{(method.ReturnsVoid ? "Action" : "Function")}{method.Parameters.Length}");
        if (!method.ReturnsVoid || method.Parameters.Length != 0)
            builder.AngleBracketed(writeTypeArguments);

        builder.Space().Append(syntax.Identifier.Text).Space().Append("=").Space()
            .Append(syntax.ParameterList, context).Space().AppendLine("->").Append(syntax.Body!, context).SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, VariableDeclarationSyntax syntax, JavaCodeConversionContext context)
    {
        Debug.Assert(syntax.Variables.Count == 1);
        builder.Append(syntax.Type, context).Space().Append(syntax.Variables[0], context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, VariableDeclaratorSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append(syntax.Identifier.Text);
        if (syntax.Initializer != null)
            builder.Space().Append(syntax.Initializer, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, EqualsValueClauseSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("=").Space().Append(syntax.Value, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, FinallyClauseSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("finally").AppendLine().Append(syntax.Block, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, CatchClauseSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("catch").Space();
        if (syntax.Declaration == null)
            builder.Parenthesized().Append("Exception e").Close().AppendLine();
        else
            builder.Parenthesized().Append(syntax.Declaration.Type, context).Space().Append(syntax.Declaration.Identifier.Text).Close().AppendLine();

        builder.Append(syntax.Block, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ElseClauseSyntax syntax, JavaCodeConversionContext context)
    {
        builder.Append("else").AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, SwitchSectionSyntax syntax, JavaCodeConversionContext context)
    {
        foreach (var label in syntax.Labels)
            builder.Append(label, context).AppendLine();

        builder.IndentChild().Append(syntax.Statements, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, SwitchLabelSyntax syntax, JavaCodeConversionContext context)
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

    public static CodeBuilder Append(this CodeBuilder builder, CaseSwitchLabelSyntax syntax, JavaCodeConversionContext context)
    {
        var typeSymbol = syntax.Value.GetTypeSymbol(context)!;
        if (typeSymbol.TypeKind == TypeKind.Enum)
        {
            // Shitty Java wants enum elements to be written unqualified
            var symbol = syntax.Value.GetSymbol<IFieldSymbol>(context);
            builder.Append(symbol.Name);
            return builder;
        }

        builder.Append(syntax.Value, context);
        return builder;
    }
}
