// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

static partial class TypeScriptBuilderExtension
{
    public static CodeBuilder Append(this CodeBuilder builder, BlockSyntax syntax, TypeScriptCompilationContext context, bool skipBraces = false)
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

    static CodeBuilder Append(this CodeBuilder builder, IEnumerable<StatementSyntax> staments, TypeScriptCompilationContext context)
    {
        bool first = true;
        foreach (var statement in staments)
            builder.AppendLine(ref first).Append(statement, context);

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, BreakStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        _ = syntax;
        _ = context;
        builder.Append("break").SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ForEachStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("for").Space().Parenthesized().Append("let").Space().Append(syntax.Identifier.Text)
            .Space().Append("of").Space().Append(syntax.Expression, context).Close().AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ContinueStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("continue").SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, DoStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("do").AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        builder.Append("while").Space().Parenthesized().Append(syntax.Condition, context).Close().SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, EmptyStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        _ = syntax;
        _ = context;
        return builder.SemiColon();
    }

    public static CodeBuilder Append(this CodeBuilder builder, ExpressionStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append(syntax.Expression, context).SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ForStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("for").Space().Parenthesized(() =>
        {
            if (syntax.Declaration != null)
                builder.Append("let").Space().Append(syntax.Declaration, context);

            builder.SemiColonSeparator();
            if (syntax.Condition != null)
                builder.Append(syntax.Condition, context);

            builder.SemiColonSeparator().Append(syntax.Incrementors, context);
        }).AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, IfStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("if").Space().Parenthesized().Append(syntax.Condition, context).Close().AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        if (syntax.Else != null)
            builder.AppendLine().Append(syntax.Else, context);

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, LocalDeclarationStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        if (syntax.IsConst)
            builder.Append("final").Space();

        builder.Append("let").Space().Append(syntax.Declaration, context).SemiColon();
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, LocalFunctionStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("let").Space().Append(syntax.Identifier.Text).Space().Append("=").Space().Append(syntax.ParameterList, context).Space().Colon().Space()
            .Append(syntax.ReturnType.GetTypeScriptType(context)).Space().AppendLine("=>")
            .Append(syntax.Body!, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ReturnStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        if (syntax.Expression != null)
        {
            // FIXME: Workaround for ref arguments invocations. Real fix is improve
            // existing tree manipulation to also add syntetized arguments. See TypeScriptValidationContext
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

    public static CodeBuilder Append(this CodeBuilder builder, SwitchStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("switch").Space().Parenthesized().Append(syntax.Expression, context).Close().AppendLine();
        using (builder.Block(false))
        {
            foreach (var section in syntax.Sections)
                builder.Append(section, context).AppendLine();
        }
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ThrowStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("throw");
        if (syntax.Expression != null)
            builder.Space().Append(syntax.Expression, context);

        return builder.SemiColon();
    }

    public static CodeBuilder Append(this CodeBuilder builder, TryStatementSyntax syntax, TypeScriptCompilationContext context)
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

    public static CodeBuilder Append(this CodeBuilder builder, UsingStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("try").Space().Parenthesized().Append(syntax.Declaration!, context).Close().AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, WhileStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("while").Space().Parenthesized().Append(syntax.Condition, context).Close().AppendLine()
            .IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, YieldStatementSyntax syntax, TypeScriptCompilationContext context)
    {
        if (syntax.Expression == null)
            builder.Append("yield").Space().Append("break");
        else
            builder.Append("yield").Space().Append(syntax.Expression, context);
        return builder.SemiColon();
    }

    // Reference: roslyn/src/Compilers/CSharp/Portable/Generated/Syntax.xml.Main.Generated.cs
    public static CodeBuilder Append(this CodeBuilder builder, StatementSyntax statement, TypeScriptCompilationContext context)
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
            case SyntaxKind.LocalFunctionStatement:
                return builder.Append((LocalFunctionStatementSyntax)statement, context);
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
            case SyntaxKind.YieldReturnStatement:
            case SyntaxKind.YieldBreakStatement:
                return builder.Append((YieldStatementSyntax)statement, context);
            // Unsupported statements
            case SyntaxKind.LockStatement:
            case SyntaxKind.CheckedStatement:
            case SyntaxKind.UnsafeStatement:
            case SyntaxKind.LabeledStatement:
            case SyntaxKind.FixedStatement:
            case SyntaxKind.ForEachVariableStatement:
            // Unsupported goto statements
            case SyntaxKind.GotoStatement:
            case SyntaxKind.GotoCaseStatement:
            case SyntaxKind.GotoDefaultStatement:
            default:
                throw new Exception();
        }
    }

    public static CodeBuilder Append(this CodeBuilder builder, VariableDeclarationSyntax syntax, TypeScriptCompilationContext context)
    {
        Debug.Assert(syntax.Variables.Count == 1);
        var declaration = syntax.Variables[0];
        builder.Append(declaration.Identifier.Text).Colon().Space().Append(syntax.Type, context);
        if (declaration.Initializer == null)
            builder.Space().Append("=").Space().Append(syntax.Type.GetTypeScriptDefaultLiteral(context)!);
        else
            builder.Space().Append(declaration.Initializer, context);  

        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, EqualsValueClauseSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("=").Space().Append(syntax.Value, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, FinallyClauseSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("finally").AppendLine().Append(syntax.Block, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, CatchClauseSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("catch").Space();
        if (syntax.Declaration == null)
            builder.Parenthesized().Append("Exception e").Close().AppendLine();
        else
            builder.Parenthesized().Append(syntax.Declaration.Type, context).Space().Append(syntax.Declaration.Identifier.Text).Close().AppendLine();

        builder.Append(syntax.Block, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, ElseClauseSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append("else").AppendLine();
        builder.IndentChild().Append(syntax.Statement, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, SwitchSectionSyntax syntax, TypeScriptCompilationContext context)
    {
        foreach (var label in syntax.Labels)
            builder.Append(label, context).AppendLine();

        builder.IndentChild().Append(syntax.Statements, context);
        return builder;
    }

    public static CodeBuilder Append(this CodeBuilder builder, SwitchLabelSyntax syntax, TypeScriptCompilationContext context)
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

    public static CodeBuilder Append(this CodeBuilder builder, CaseSwitchLabelSyntax syntax, TypeScriptCompilationContext context)
    {
        builder.Append(syntax.Value, context);
        return builder;
    }
}
