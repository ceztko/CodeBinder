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
    [DebuggerDisplay("TypeName = {TypeName}")]
    public abstract class CSharpBaseTypeContext : TypeContext<CSharpBaseTypeContext>, ITypeContext<CSharpCompilationContext>
    {
        internal CSharpBaseTypeContext() { }

        public BaseTypeDeclarationSyntax Node
        {
            get { return GetBaseType(); }
        }

        public string TypeName
        {
            get { return Node.Identifier.Text; }
        }

        public new CSharpCompilationContext Compilation => getCompilationContext();

        protected abstract CSharpCompilationContext getCompilationContext();

        // We override TypeConversion GetConversion() in inherited class
        protected sealed override TypeConversion<CSharpBaseTypeContext> createConversion()
        {
            throw new NotImplementedException();
        }

        protected abstract BaseTypeDeclarationSyntax GetBaseType();

        protected internal virtual void FillMemberPartialDeclarations(
            Dictionary<TypeDeclarationSyntax, PartialDeclarationsTree> memberPartialDeclarations) { }
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

    public abstract class CSharpBaseTypeContext<TNode, TCompilationContext, TTypeContext> : CSharpBaseTypeContext
        where TNode : BaseTypeDeclarationSyntax
        where TCompilationContext : CSharpCompilationContext
        where TTypeContext : CSharpBaseTypeContext
    {
        public new TNode Node { get; private set; }

        public new TCompilationContext Compilation { get; private set; }

        protected CSharpBaseTypeContext(TNode node, TCompilationContext compilation)
        {
            Node = node;
            Compilation = compilation;
        }

        protected sealed override CompilationContext GetCompilationContext() => Compilation;

        protected sealed override CSharpCompilationContext getCompilationContext() => Compilation;

        protected internal override TypeConversion CreateConversion()
        {
            return createConversion();
        }

        protected new abstract TypeConversion<TTypeContext> createConversion();

        protected override BaseTypeDeclarationSyntax GetBaseType()
        {
            return Node;
        }
    }

    public abstract class CSharpTypeContext<TNode, TCompilationContext, TTypeContext> : CSharpTypeContext
        where TNode : TypeDeclarationSyntax
        where TCompilationContext : CSharpCompilationContext
        where TTypeContext : CSharpTypeContext
    {
        public new TNode Node { get; private set; }

        public new TCompilationContext Compilation { get; private set; }

        protected CSharpTypeContext(TNode node, TCompilationContext compilation)
        {
            Node = node;
            Compilation = compilation;
        }

        protected sealed override CompilationContext GetCompilationContext() => Compilation;

        protected sealed override CSharpCompilationContext getCompilationContext() => Compilation;

        protected internal override TypeConversion CreateConversion()
        {
            return createConversion();
        }

        protected new abstract TypeConversion<TTypeContext> createConversion();

        protected override TypeDeclarationSyntax GetSyntaxType()
        {
            return Node;
        }
    }
}
