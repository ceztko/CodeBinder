// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public abstract class SyntaxTreeContext : ICompilationContextProvider
    {
        public CompilationContext Compilation { get; private set; }

        public SyntaxTree SyntaxTree { get; private set; }

        public SyntaxTreeContext(CompilationContext compilation)
        {
            Compilation = compilation;
        }

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return Compilation.GetSemanticModel(tree);
        }

        public abstract void Visit(SyntaxTree node);

        public abstract IEnumerable<TypeContext> GetRootTypes();
    }

    public abstract class SyntaxTreeContext<TTypeContext> : SyntaxTreeContext
        where TTypeContext : TypeContext<TTypeContext>
    {
        public List<TTypeContext> _RootTypes;

        public SyntaxTreeContext(CompilationContext compilation)
            : base(compilation)
        {
            _RootTypes = new List<TTypeContext>();
        }

        public void AddType(TTypeContext type, TTypeContext parent)
        {
            if (parent == null)
                _RootTypes.Add(type);
            else
                parent.AddChild(type);
        }

        public override IEnumerable<TypeContext> GetRootTypes()
        {
            return _RootTypes;
        }
    }
}
