// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.JavaScript.TypeScript;

class TypeScriptValidationContext : CSharpValidationContext<ConversionCSharpToTypeScript>
{
    public TypeScriptValidationContext(ConversionCSharpToTypeScript conversion)
        : base(conversion)
    {
        Init += TypeScriptValidationContext_Init;
    }

    private void TypeScriptValidationContext_Init(CSharpNodeVisitor visitor)
    {
        visitor.InvocationExpressionVisit += Visitor_InvocationExpressionVisit;
    }

    private void Visitor_InvocationExpressionVisit(CSharpNodeVisitor visitor, InvocationExpressionSyntax node)
    {
        var symbol = node.GetSymbol<IMethodSymbol>(this);
        if (!symbol.IsNative())
            return;

        foreach (var arg in node.ArgumentList.Arguments)
        {
            if (arg.IsRefLike())
            {
                injectRefInvocationTrampoline(node, symbol);
                break;
            }
        }
    }

    void injectRefInvocationTrampoline(InvocationExpressionSyntax node, IMethodSymbol method)
    {
        int index;
        var block = node.FindAncestorBlock(out index);

        PushReplacement(block, (localnode, options) =>
        {
            // NOTE: SyntaxFactory.ParseTypeName() for some reason fails to parse void
            var retType = method.ReturnsVoid
                ? SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword))
                : SyntaxFactory.ParseTypeName(method.ReturnType.GetCSharpTypeName());

            var localFunction = SyntaxFactory.LocalFunctionStatement(SyntaxFactory.TokenList(), retType, SyntaxFactory.Identifier($"__{method.Name}"),
            null, SyntaxFactory.ParameterList(), SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
            getRefInvocationBlock(node, method), null).NormalizeWhitespace();

            return localnode.WithStatements(localnode.Statements.Insert(index, localFunction));
        });
        PushReplacement(node, (localnode, options) => SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName($"__{method.Name}")));
    }

    BlockSyntax getRefInvocationBlock(InvocationExpressionSyntax node, IMethodSymbol method)
    {
        StatementSyntax statement;
        if (method.ReturnsVoid)
            statement = SyntaxFactory.ExpressionStatement(node);
        else
           statement = SyntaxFactory.ReturnStatement(node);

        return SyntaxFactory.Block(SyntaxFactory.SingletonList(statement));
    }
}
