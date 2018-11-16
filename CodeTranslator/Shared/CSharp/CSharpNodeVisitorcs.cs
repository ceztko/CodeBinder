using CodeTranslator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    class CSharpNodeVisitor : CSharpNodeVisitor<CSharpSyntaxTreeContext, CSharpLanguageConversion>
    {
        private Queue<CSharpTypeContext> _parents;

        public CSharpTypeContext CurrentParent
        {
            get
            {
                if (_parents.Count == 0)
                    return null;

                return _parents.Peek();
            }
        }

        private void Unsupported(SyntaxNode node, string message = null)
        {
            if (message == null)
                throw new Exception("Unsupported node: " + node);
            else
                throw new Exception("Unsupported node: " + node + ", " + message);
        }

        #region Supported types

        public CSharpNodeVisitor(CSharpSyntaxTreeContext context, CSharpLanguageConversion conversion)
            : base(context, conversion)
        {
            _parents = new Queue<CSharpTypeContext>();
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var type = new CSharpInterfaceTypeContext(node, TreeContext, Conversion.GetInterfaceTypeConversion());
            TreeContext.AddType(type, CurrentParent);
            DefaultVisit(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var type = new CSharpClassTypeContext(node, TreeContext, Conversion.GetClassTypeConversion());
            TreeContext.AddType(type, CurrentParent);
            _parents.Enqueue(type);
            DefaultVisit(node);
            _parents.Dequeue();
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            var type = new CSharpStructTypeContext(node, TreeContext, Conversion.GetStructTypeConversion());
            TreeContext.AddType(type, CurrentParent);
            _parents.Enqueue(type);
            DefaultVisit(node);
            _parents.Dequeue();
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            var type = new CSharpEnumTypeContext(node, TreeContext, Conversion.GetEnumTypeConversion());
            TreeContext.AddType(type, CurrentParent);
            DefaultVisit(node);
        }

        #endregion Supported types

        #region Unsupported syntax

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            if (!node.VarianceKeyword.IsNone())
                Unsupported(node, "Type parameter with unsupported variance modifier");

            DefaultVisit(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (node.Modifiers.Any((token) => token.Kind() != SyntaxKind.ConstKeyword))
                Unsupported(node, "Variable declaration with unsupported modifiers");

            DefaultVisit(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (node.AccessorList == null)
                Unsupported(node, "Unsupported property with no accessor definied: use \"get\" or \"set\"");

            DefaultVisit(node);
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            if (node.Rank > 1)
                Unsupported(node, "Unsupported array with rank > 1");

            DefaultVisit(node);
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            var typeSymbol = node.Type.GetTypeSymbol(this);
            if (typeSymbol.TypeKind == TypeKind.TypeParameter)
                Unsupported(node, "Unsupported typeof expression with parameterized type");

            DefaultVisit(node);
        }

        public override void VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            Unsupported(node);
        }

        #endregion // Unsupported syntax

        #region Unsupported Linq


        public override void VisitFromClause(FromClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            Unsupported(node);
        }

        #endregion // Unsupported Linq

        #region Unsupported statements

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            Unsupported(node);
        }

        #endregion // Unsupported statements

        #region Unsupported expressions

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitTupleType(TupleTypeSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitTupleExpression(TupleExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            Unsupported(node);
        }

        #endregion // Unsupported expressions
    }

    public class CSharpNodeVisitor<TSyntaxTree, TLanguageConversion> : CSharpSyntaxWalker, ICompilationContextProvider
        where TSyntaxTree : SyntaxTreeContext
        where TLanguageConversion : LanguageConversion
    {
        public TLanguageConversion Conversion { get; private set; }
        public TSyntaxTree TreeContext { get; private set; }

        public CompilationContext Compilation
        {
            get { return TreeContext.Compilation; }
        }

        public override void Visit(SyntaxNode node)
        {
            if (node.HasAttribute<IgnoreAttribute>(this))
                return;

            base.Visit(node);
        }

        public CSharpNodeVisitor(TSyntaxTree treeContext, TLanguageConversion conversion)
        {
            TreeContext = treeContext;
            Conversion = conversion;
        }
    }
}
