// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace CodeBinder.Shared.CSharp
{
    /// <summary>
    /// CSharp base type contexts, namely classes, structs, interfaces, enums and delegates
    /// </summary>
    /// <remarks>We consider base types also delegates, differently from Microsoft.CodeAnalysis.CSharp hieararchy</remarks>
    public abstract class CSharpMemberTypeContext : TypeContext<CSharpMemberTypeContext>, ITypeContext<CSharpCompilationContext>
    {
        string _FullName;

        internal CSharpMemberTypeContext()
        {
            _FullName = null!;
        }

        public override string FullName => _FullName;

        public INamedTypeSymbol Symbol { get; private set; } = null!;

        public abstract MemberDeclarationSyntax Node { get; }

        public abstract override CSharpCompilationContext Compilation { get; }

        /// <summary>
        /// True if this is the main declaration
        /// </summary>
        public virtual bool IsMainDeclaration => true;

        internal void Init()
        {
            Symbol = Node.GetDeclaredSymbol<INamedTypeSymbol>(Compilation);
            _FullName = Symbol.GetFullName();
        }

        protected internal virtual void FillMemberPartialDeclarations(
            Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations) { }
    }

    public abstract class CSharpBaseTypeContext : CSharpMemberTypeContext
    {
        internal CSharpBaseTypeContext() { }

        public override abstract BaseTypeDeclarationSyntax Node { get; }
    }

    /// <summary>
    /// CSharp type contexts, namely classes, structs, interfaces
    /// </summary>
    public abstract class CSharpTypeContext : CSharpBaseTypeContext
    {
        List<CSharpTypeContext> _PartialDeclarations;

        internal CSharpTypeContext()
        {
            _PartialDeclarations = new List<CSharpTypeContext>();
        }

        public PartialDeclarationsTree ComputePartialDeclarationsTree()
        {
            var rootPartialDeclarations = new List<TypeDeclarationSyntax>();
            foreach (var partialDeclaration in _PartialDeclarations)
                rootPartialDeclarations.Add(partialDeclaration.Node);

            var childrenPartialDeclarations = new Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree>();
            foreach (var child in Children)
                child.FillMemberPartialDeclarations(childrenPartialDeclarations);

            return new PartialDeclarationsTree(rootPartialDeclarations, childrenPartialDeclarations);
        }

        internal void AddPartialDeclaration(CSharpTypeContext partialDeclaration)
        {
            _PartialDeclarations.Add(partialDeclaration);
        }

        /// <summary>
        /// True if this is the main declaration, with PartialDeclarations.Count != 0
        /// </summary>
        public override bool IsMainDeclaration => _PartialDeclarations.Count != 0;

        public override abstract TypeDeclarationSyntax Node { get; }

        public IReadOnlyList<CSharpTypeContext> PartialDeclarations
        {
            get { return _PartialDeclarations; }
        }

        protected internal override void FillMemberPartialDeclarations(Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations)
        {
            var partialDeclarations = ComputePartialDeclarationsTree();
            memberPartialDeclarations.Add(Node, partialDeclarations);
        }
    }

    public abstract class CSharpMemberTypeContext<TNode> : CSharpMemberTypeContext
        where TNode : MemberDeclarationSyntax
    {
        TNode _Node;

        internal CSharpMemberTypeContext(TNode node)
        {
            _Node = node;
        }

        public override TNode Node => _Node;
    }

    public abstract class CSharpBaseTypeContext<TNode> : CSharpBaseTypeContext
        where TNode : BaseTypeDeclarationSyntax
    {
        TNode _Node;

        internal CSharpBaseTypeContext(TNode node)
        {
            _Node = node;
        }

        public override TNode Node => _Node;

        public override string Name => Node.Identifier.Text;
    }

    public abstract class CSharpTypeContext<TNode> : CSharpTypeContext
        where TNode : TypeDeclarationSyntax
    {
        TNode _Node;

        internal CSharpTypeContext(TNode node)
        {
            _Node = node;
        }

        public override TNode Node => _Node;

        public override string Name => Node.Identifier.Text;
    }
}
