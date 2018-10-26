// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    public abstract class CSharpTypeContext : TypeContext
    {
        public new CSharpSyntaxTreeContext TreeContext { get; private set; }

        protected CSharpTypeContext(CSharpSyntaxTreeContext treeContext)
        {
            TreeContext = treeContext;
        }

        public CSharpSyntaxNode Node
        {
            get { return GetNode(); }
        }

        protected override SyntaxTreeContext GetTreeContext()
        {
            return TreeContext;
        }

        protected abstract CSharpSyntaxNode GetNode();
    }

    public class CSharpTypeContext<TNode> : CSharpTypeContext
        where TNode : CSharpSyntaxNode
    {
        public new TNode Node { get; private set; }

        protected CSharpTypeContext(TNode node, CSharpSyntaxTreeContext treeContext)
            : base(treeContext)
        {
            Node = node;
        }

        protected override CSharpSyntaxNode GetNode()
        {
            return Node;
        }
    }
}
