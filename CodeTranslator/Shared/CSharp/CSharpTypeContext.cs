// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CodeTranslator.Shared.CSharp
{
    [DebuggerDisplay("Name = {Name}")]
    public abstract class CSharpBaseTypeContext : TypeContext<CSharpBaseTypeContext, CSharpSyntaxTreeContext>
    {
        internal CSharpBaseTypeContext(CSharpSyntaxTreeContext treeContext)
            : base(treeContext) { }

        public BaseTypeDeclarationSyntax Node
        {
            get { return GetBaseType(); }
        }

        public string Name
        {
            get { return Node.Identifier.Text; }
        }

        protected abstract BaseTypeDeclarationSyntax GetBaseType();

        protected internal virtual void FillMemberPartialDeclarations(
            Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations) { }
    }

    public abstract class CSharpTypeContext : CSharpBaseTypeContext
    {
        List<CSharpTypeContext> _partialDeclarations;

        internal CSharpTypeContext(CSharpSyntaxTreeContext treeContext)
            : base(treeContext)
        {
            _partialDeclarations = new List<CSharpTypeContext>();
        }

        public PartialDeclarationsTree ComputePartialDeclarationsTree()
        {
            var partialDeclarations = new List<TypeDeclarationSyntax>();
            foreach (var partialDeclaration in _partialDeclarations)
                partialDeclarations.Add(partialDeclaration.Node);

            var memberPartialDelarations = new Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree>();
            foreach (var child in Children)
                child.FillMemberPartialDeclarations(memberPartialDelarations);

            return new PartialDeclarationsTree(partialDeclarations, memberPartialDelarations);
        }

        public void AddPartialDeclaration(CSharpTypeContext partialDeclaration)
        {
            _partialDeclarations.Add(partialDeclaration);
        }

        public IReadOnlyList<CSharpTypeContext> PartialDeclarations
        {
            get { return _partialDeclarations; }
        }

        public new TypeDeclarationSyntax Node
        {
            get { return GetSyntaxType(); }
        }

        protected override BaseTypeDeclarationSyntax GetBaseType()
        {
            return GetSyntaxType();
        }

        protected abstract TypeDeclarationSyntax GetSyntaxType();

        protected internal override void FillMemberPartialDeclarations(Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations)
        {
            var partialDeclarations = ComputePartialDeclarationsTree();
            memberPartialDeclarations.Add(Node, partialDeclarations);
        }
    }

    public abstract class CSharpBaseTypeContext<TNode, TTypeConversion> : CSharpBaseTypeContext
        where TNode : BaseTypeDeclarationSyntax
        where TTypeConversion : TypeConversion
    {
        public new TNode Node { get; private set; }

        public new TTypeConversion Conversion { get; private set; }

        protected CSharpBaseTypeContext(TNode node, CSharpSyntaxTreeContext treeContext, TTypeConversion conversion)
            : base(treeContext)
        {
            Node = node;
            Conversion = conversion;
        }

        protected override TypeConversion GetConversion()
        {
            return Conversion;
        }

        protected override BaseTypeDeclarationSyntax GetBaseType()
        {
            return Node;
        }
    }

    public abstract class CSharpTypeContext<TNode, TTypeConversion> : CSharpTypeContext
        where TNode : TypeDeclarationSyntax
        where TTypeConversion : TypeConversion
    {
        public new TNode Node { get; private set; }

        public new TTypeConversion Conversion { get; private set; }

        protected CSharpTypeContext(TNode node, CSharpSyntaxTreeContext treeContext, TTypeConversion conversion)
            : base(treeContext)
        {
            Node = node;
            Conversion = conversion;
        }

        protected override TypeConversion GetConversion()
        {
            return Conversion;
        }

        protected override TypeDeclarationSyntax GetSyntaxType()
        {
            return Node;
        }
    }
}
