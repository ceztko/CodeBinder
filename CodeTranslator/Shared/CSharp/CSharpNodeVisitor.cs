// Copyright (c) 2017-2018 ICSharpCode
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
    public class CSharpNodeVisitor : NodeVisitor<CSharpSyntaxTreeContext>
    {
        public override void Visit(SyntaxNode node)
        {
            var walker = new Walker(Context);
            walker.Visit(node);
        }

        #region Support

        class Walker : CSharpSyntaxWalker
        {
            CSharpSyntaxTreeContext _context;

            public Walker(CSharpSyntaxTreeContext context)
            {
                _context = context;
            }

            #region Namespace Members

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                _context.Classes.Add(node);
                DefaultVisit(node);
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
            {
                _context.Structs.Add(node);
                DefaultVisit(node);
            }

            public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
            {
                _context.Interfaces.Add(node);
                DefaultVisit(node);
            }

            public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
            {
                _context.Enums.Add(node);
                DefaultVisit(node);
            }

            #endregion // Namespace Members
        }

        #endregion // Support
    }
}
