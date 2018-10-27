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
    public abstract class CSharpTypeContext : TypeContext
    {
        public new CSharpSyntaxTreeContext TreeContext { get; private set; }

        protected CSharpTypeContext(CSharpSyntaxTreeContext treeContext)
        {
            TreeContext = treeContext;
        }

        public BaseTypeDeclarationSyntax Node
        {
            get { return GetNode(); }
        }

        protected override SyntaxTreeContext GetTreeContext()
        {
            return TreeContext;
        }

        protected abstract BaseTypeDeclarationSyntax GetNode();
    }

    public abstract class CSharpTypeContext<TNode, TTypeConversion> : CSharpTypeContext
        where TNode : BaseTypeDeclarationSyntax
        where TTypeConversion : TypeConversion
    {
        public new TTypeConversion Conversion { get; private set; }

        public new TNode Node { get; private set; }

        protected CSharpTypeContext(TNode node, CSharpSyntaxTreeContext treeContext, TTypeConversion conversion)
            : base(treeContext)
        {
            Node = node;
            Conversion = conversion;
        }

        protected override BaseTypeDeclarationSyntax GetNode()
        {
            return Node;
        }

        protected override TypeConversion GetConversion()
        {
            return Conversion;
        }
    }
}
