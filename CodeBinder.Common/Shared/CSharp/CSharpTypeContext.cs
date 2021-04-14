// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CodeBinder.Shared.CSharp
{
    public abstract class CSharpBaseTypeContext : TypeContext<CSharpBaseTypeContext>, ITypeContext<CSharpCompilationContext>
    {
        internal CSharpBaseTypeContext() { }

        public BaseTypeDeclarationSyntax Node
        {
            get { return GetBaseType(); }
        }

        public override string Name => Node.Identifier.Text;

        public new CSharpCompilationContext Compilation => GetCSharpCompilationContext();

        protected abstract CSharpCompilationContext GetCSharpCompilationContext();

        protected abstract BaseTypeDeclarationSyntax GetBaseType();

        protected internal virtual void FillMemberPartialDeclarations(
            Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations) { }

        protected sealed override CompilationContext GetCompilationContext() => GetCSharpCompilationContext();
    }

    public abstract class CSharpTypeContext : CSharpBaseTypeContext
    {
        List<CSharpTypeContext> _partialDeclarations;

        internal CSharpTypeContext()
        {
            _partialDeclarations = new List<CSharpTypeContext>();
        }

        public PartialDeclarationsTree ComputePartialDeclarationsTree()
        {
            var rootPartialDeclarations = new List<TypeDeclarationSyntax>();
            foreach (var partialDeclaration in _partialDeclarations)
                rootPartialDeclarations.Add(partialDeclaration.Node);

            var childrenPartialDeclarations = new Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree>();
            foreach (var child in Children)
                child.FillMemberPartialDeclarations(childrenPartialDeclarations);

            return new PartialDeclarationsTree(rootPartialDeclarations, childrenPartialDeclarations);
        }

        internal void AddPartialDeclaration(CSharpTypeContext partialDeclaration)
        {
            _partialDeclarations.Add(partialDeclaration);
        }

        public IReadOnlyList<CSharpTypeContext> PartialDeclarations
        {
            get { return _partialDeclarations; }
        }

        public new TypeDeclarationSyntax Node => GetSyntaxType();

        protected override BaseTypeDeclarationSyntax GetBaseType() => GetSyntaxType();

        protected abstract TypeDeclarationSyntax GetSyntaxType();

        protected internal override void FillMemberPartialDeclarations(Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations)
        {
            var partialDeclarations = ComputePartialDeclarationsTree();
            memberPartialDeclarations.Add(Node, partialDeclarations);
        }
    }

    public abstract class CSharpBaseTypeContext<TNode, TTypeContext> : CSharpBaseTypeContext
        where TNode : BaseTypeDeclarationSyntax
    {
        public new TNode Node { get; private set; }

        internal CSharpBaseTypeContext(TNode node)
        {
            Node = node;
        }

        protected override BaseTypeDeclarationSyntax GetBaseType() => Node;
    }

    public abstract class CSharpTypeContext<TNode> : CSharpTypeContext
        where TNode : TypeDeclarationSyntax
    {
        public new TNode Node { get; private set; }

        internal CSharpTypeContext(TNode node)
        {
            Node = node;
        }

        protected override TypeDeclarationSyntax GetSyntaxType() => Node;
    }
}
