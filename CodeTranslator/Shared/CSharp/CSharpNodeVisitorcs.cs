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

        public override void Visit(SyntaxNode node)
        {
            if (node.HasAttribute<IgnoreAttribute>(this))
                return;

            var kind = node.Kind();
            switch (kind)
            {
                // Type constraints
                case SyntaxKind.ConstructorConstraint:
                case SyntaxKind.StructConstraint:
                case SyntaxKind.ClassConstraint:
                // Declarations
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.OperatorDeclaration:
                // Statements
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UnsafeStatement:
                case SyntaxKind.LabeledStatement:
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.LocalFunctionStatement:
                case SyntaxKind.ForEachVariableStatement:
                // Expressions
                case SyntaxKind.DefaultExpression:
                case SyntaxKind.AnonymousMethodExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.PointerType:
                case SyntaxKind.RefValueExpression:
                case SyntaxKind.RefTypeExpression:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.ElementBindingExpression:
                case SyntaxKind.ImplicitElementAccess:
                case SyntaxKind.MemberBindingExpression:
                case SyntaxKind.SizeOfExpression:
                case SyntaxKind.MakeRefExpression:
                case SyntaxKind.ImplicitStackAllocArrayCreationExpression:
                case SyntaxKind.InterpolatedStringExpression:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.QueryExpression:
                case SyntaxKind.StackAllocArrayCreationExpression:
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.TupleType:
                case SyntaxKind.TupleExpression:
                case SyntaxKind.IsPatternExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.ConditionalAccessExpression:
                case SyntaxKind.AliasQualifiedName:
                // Prefix unary expression
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                // Binary expression
                case SyntaxKind.CoalesceExpression:
                // Member access expression
                case SyntaxKind.PointerMemberAccessExpression:
                // Linq
                case SyntaxKind.FromClause:
                case SyntaxKind.WhereClause:
                case SyntaxKind.SelectClause:
                case SyntaxKind.GroupClause:
                case SyntaxKind.JoinIntoClause:
                case SyntaxKind.OrderByClause:
                case SyntaxKind.JoinClause:
                case SyntaxKind.LetClause:
                // Misc
                case SyntaxKind.ArrowExpressionClause:
                {
                    Unsupported(node);
                    break;
                }
            }

            visit(node);
        }

        #endregion Supported types

        #region Unsupported syntax

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
            var parentKind = node.Parent.Kind();
            switch (parentKind)
            {
                case SyntaxKind.UsingDirective:
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.Attribute:
                    break;
                default:
                    Unsupported(node, "Unsupported qualified name expression with parent " + node.Parent);
                    break;
            }

            // NOTE: Just ignore the node
        }

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

        #endregion // Unsupported syntax
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

            visit(node);
        }

        protected void visit(SyntaxNode node)
        {
            base.Visit(node);
        }

        public CSharpNodeVisitor(TSyntaxTree treeContext, TLanguageConversion conversion)
        {
            TreeContext = treeContext;
            Conversion = conversion;
        }
    }
}
