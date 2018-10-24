// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    public class CSharpEnumTypeContext : CSharpTypeContext
    {
        public new EnumDeclarationSyntax Node { get; private set; }

        public bool IsFlag { get; private set; }

        public CSharpEnumTypeContext(EnumDeclarationSyntax node, CSharpSyntaxTreeContext treeContext)
            : base(treeContext)
        {
            Node = node;
            IsFlag = node.IsFlag(treeContext);
        }

        protected override CSharpSyntaxNode GetNode()
        {
            return Node;
        }
    }
}
