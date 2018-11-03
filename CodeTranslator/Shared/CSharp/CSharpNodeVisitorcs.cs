using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
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

        private void Unsupported(SyntaxNode node)
        {
            throw new Exception("Unsupported node: " + node);
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

        // TODO: Add more unsupported syntax

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            if (node.Rank > 1)
                Unsupported(node);

            DefaultVisit(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            Unsupported(node);
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            Unsupported(node);
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

        public CSharpNodeVisitor(TSyntaxTree treeContext, TLanguageConversion conversion)
        {
            TreeContext = treeContext;
            Conversion = conversion;
        }

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return TreeContext.Compilation.GetSemanticModel(tree);
        }
    }
}
