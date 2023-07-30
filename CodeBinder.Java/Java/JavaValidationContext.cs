// SPDX-FileCopyrightText: (C) 2023 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

using System.Linq;

namespace CodeBinder.Java;

class JavaValidationContext : CSharpValidationContext<ConversionCSharpToJava>
{
    HashSet<ILocalSymbol> _localRefArguments;

    public JavaValidationContext(ConversionCSharpToJava conversion)
        : base(conversion)
    {
        Init += JavaValidationContext_Init;
        _localRefArguments = new HashSet<ILocalSymbol>(SymbolEqualityComparer.Default);
    }

    private void JavaValidationContext_Init(CSharpNodeVisitor visitor)
    {
        visitor.InvocationExpressionVisit += Visitor_InvocationExpressionVisit;
    }

    private void Visitor_InvocationExpressionVisit(CSharpNodeVisitor visitor, InvocationExpressionSyntax node)
    {
        var symbol = node.GetSymbol<IMethodSymbol>(this);
        if (!symbol.IsNative())
            return;

        bool doInjectRefInvocationTrampoline = false;
        var localRefVariables = new List<ILocalSymbol>();
        foreach (var arg in node.ArgumentList.Arguments)
        {
            if (arg.IsRefLike())
            {
                doInjectRefInvocationTrampoline = true;
                var argSymbol = arg.Expression.GetSymbol(this)!;
                if (argSymbol.Kind == SymbolKind.Local)
                {
                    var local = (ILocalSymbol)argSymbol;
                    if (_localRefArguments.Contains(local))
                        continue;

                    _localRefArguments.Add(local);
                    localRefVariables.Add(local);
                }
            }
        }

        if (doInjectRefInvocationTrampoline)
        {
            foreach (var localRefVariable in localRefVariables)
                handleLocalVariable(node, localRefVariable, this);

            injectRefInvocationTrampoline(node, symbol);
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

            var localFunction = SyntaxFactory.LocalFunctionStatement(SyntaxFactory.TokenList(), retType,
                SyntaxFactory.Identifier($"__{method.Name}"), null, SyntaxFactory.ParameterList(),
                SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
                getRefInvocationBlock(node, method), null);

            return localnode.WithStatements(localnode.Statements.Insert(index, localFunction));
        });
        PushReplacement(node, (localnode, options) =>
        {
            node = localnode;
            return SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName($"__{method.Name}"));
        });
    }

    void handleLocalVariable(InvocationExpressionSyntax node, ILocalSymbol local, ICompilationProvider compilation)
    {
        var declaration = (VariableDeclarationSyntax)local.DeclaringSyntaxReferences[0].GetSyntax().Parent!;
        PushReplacement(declaration, (localNode, options) =>
        {
            var declarator = localNode.Variables[0];
            InitializerExpressionSyntax? arrayInitializer = null;
            if (declarator.Initializer != null)
                arrayInitializer = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, SyntaxFactory.SingletonSeparatedList(declarator.Initializer.Value));

            var arrayTypeExpr = SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName(local.Type.GetCSharpTypeName()), SyntaxFactory.SingletonList(
                SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))))));
            var ret = localNode.WithVariables(localNode.Variables.Replace(declarator, declarator.WithInitializer(
                    SyntaxFactory.EqualsValueClause(SyntaxFactory.ArrayCreationExpression(arrayTypeExpr, arrayInitializer)))));

            return ret.WithType(SyntaxFactory.IdentifierName("var"));
        });

        var referenceFinder = new LocalReferenceFinder(local, compilation);
        var block = node.FindAncestorBlock();
        referenceFinder.Visit(block);
        foreach (var reference in referenceFinder.SyntaxReferences)
        {
            PushReplacement(reference, (localNode, options) =>
            {
                return SyntaxFactory.ElementAccessExpression(localNode, SyntaxFactory.BracketedArgumentList(
                    SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0))
                    ))));
            });
        }
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

    class LocalReferenceFinder : CSharpSyntaxWalker
    {
        List<IdentifierNameSyntax> _SyntaxReferences;

        public ILocalSymbol Symbol { get; private set; }

        public ICompilationProvider Compilation { get; private set; }

        public IReadOnlyList<IdentifierNameSyntax> SyntaxReferences => _SyntaxReferences;

        public LocalReferenceFinder(ILocalSymbol local, ICompilationProvider compilation)
        {
            Symbol = local;
            Compilation = compilation;
            _SyntaxReferences = new List<IdentifierNameSyntax>();
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            var symbol = node.GetSymbol(Compilation)!;
            if (symbol.Kind == SymbolKind.Local)
            {
                if (symbol.Equals(Symbol, SymbolEqualityComparer.Default))
                    _SyntaxReferences.Add(node);
            }

            DefaultVisit(node);
        }
    }
}
