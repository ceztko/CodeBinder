using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    class CSharpNodeVisitor : CSharpSyntaxWalker
    {
        public CSharpLanguageConversion _conversion;
        public CSharpSyntaxTreeContext _context;
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
        {
            _context = context;
            _conversion = conversion;
            _parents = new Queue<CSharpTypeContext>();
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var type = new CSharpInterfaceTypeContext(node, _context, _conversion.GetInterfaceTypeConversion());
            _context.AddType(type, CurrentParent);
            DefaultVisit(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var type = new CSharpClassTypeContext(node, _context, _conversion.GetClassTypeConversion());
            _context.AddType(type, CurrentParent);
            _parents.Enqueue(type);
            DefaultVisit(node);
            _parents.Dequeue();
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            var type = new CSharpStructTypeContext(node, _context, _conversion.GetStructTypeConversion());
            _context.AddType(type, CurrentParent);
            _parents.Enqueue(type);
            DefaultVisit(node);
            _parents.Dequeue();
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            var type = new CSharpEnumTypeContext(node, _context, _conversion.GetEnumTypeConversion());
            _context.AddType(type, CurrentParent);
            DefaultVisit(node);
        }

        #endregion Supported types

        #region Unsupported syntax

        // TODO: Add more unsupported syntax

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
}
