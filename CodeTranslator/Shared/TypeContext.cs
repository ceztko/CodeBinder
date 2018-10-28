// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public abstract class TypeContext : ISemanticModelProvider
    {
        internal TypeContext() { }

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return TreeContext.Compilation.GetSemanticModel(tree);
        }

        public SyntaxTreeContext TreeContext
        {
            get { return GetCompilationContext(); }
        }

        public TypeConversion Conversion
        {
            get { return GetConversion(); }
        }

        public IEnumerable<TypeContext> Children
        {
            get { return GetChildren(); }
        }

        protected abstract SyntaxTreeContext GetCompilationContext();

        protected abstract TypeConversion GetConversion();

        protected abstract IEnumerable<TypeContext> GetChildren();
    }

    public abstract class TypeContext<TTypeContext> : TypeContext
        where TTypeContext : TypeContext
    {
        private List<TTypeContext> _Children;

        protected TypeContext()
        {
            _Children = new List<TTypeContext>();
        }

        internal void AddChild(TTypeContext child)
        {
            _Children.Add(child);
        }

        public new IEnumerable<TTypeContext> Children
        {
            get { return _Children; }
        }

        protected override IEnumerable<TypeContext> GetChildren()
        {
            return _Children;
        }
    }
}
