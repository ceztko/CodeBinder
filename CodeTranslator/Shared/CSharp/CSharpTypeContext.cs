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
    public abstract class CSharpBaseTypeContext : TypeContext<CSharpBaseTypeContext, CSharpSyntaxTreeContext>
    {
        List<CSharpBaseTypeContext> _partialDeclarations;

        internal CSharpBaseTypeContext(CSharpSyntaxTreeContext treeContext)
            : base(treeContext)
        {
            _partialDeclarations = new List<CSharpBaseTypeContext>();
        }

        public BaseTypeDeclarationSyntax Node
        {
            get { return GetBaseType(); }
        }

        public void AddPartialDeclaration(CSharpBaseTypeContext partialDeclaration)
        {
            _partialDeclarations.Add(partialDeclaration);
        }

        public IReadOnlyList<CSharpBaseTypeContext> PartialDeclarations
        {
            get { return _partialDeclarations; }
        }

        public PartialDeclarationsTree BuildPartialDeclarationTree()
        {
            return null;
        }

        protected abstract BaseTypeDeclarationSyntax GetBaseType();
    }

    public abstract class CSharpTypeContext : CSharpBaseTypeContext
    {
        internal CSharpTypeContext(CSharpSyntaxTreeContext treeContext)
            : base(treeContext) { }

        public new BaseTypeDeclarationSyntax Node
        {
            get { return GetBaseType(); }
        }

        protected override BaseTypeDeclarationSyntax GetBaseType()
        {
            return GetSyntaxType();
        }

        protected abstract TypeDeclarationSyntax GetSyntaxType();
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
