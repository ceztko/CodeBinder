// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeTranslator.Shared.CSharp
{
    public sealed class CSharpSyntaxTreeContext : SyntaxTreeContext<CSharpTypeContext>
    {
        private CSharpLanguageConversion _conversion;

        public CSharpSyntaxTreeContext(SourceCompilation compilation, CSharpLanguageConversion conversion)
            : base(compilation)
        {
            _conversion = conversion;
        }

        public override void Visit(SyntaxTree tree)
        {
            var walker = new Walker(this, _conversion);
            walker.Visit(tree.GetRoot());
        }

        #region Support

        class Walker : CSharpSyntaxWalker
        {
            public CSharpLanguageConversion _conversion;
            public CSharpSyntaxTreeContext _context;
            private Queue<CSharpTypeContext> _parents;

            public Walker(CSharpSyntaxTreeContext context, CSharpLanguageConversion conversion)
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

            public CSharpTypeContext CurrentParent
            {
                get
                {
                    if (_parents.Count == 0)
                        return null;

                    return _parents.Peek();
                }
            }
        }

        #endregion // Support
    }
}
