// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    /// <summary>
    /// Context built around a CodeAnalysis.SyntaxTree
    /// </summary>
    public abstract class SyntaxTreeContext : ICompilationContextProvider
    {
        private SyntaxTree _syntaxTree = null!;

        public EventHandler? SyntaxTreeSet;

        public CompilationContext Compilation
        {
            get { return GetCompilationContext(); }
        }

        internal SyntaxTreeContext() { }

        public SyntaxTree SyntaxTree
        {
            get { return _syntaxTree; }
            internal set
            {
                _syntaxTree = value;
                SyntaxTreeSet?.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerable<TypeContext> RootTypes
        {
            get { return GetRootTypes(); }
        }

        protected abstract CompilationContext GetCompilationContext();

        protected abstract IEnumerable<TypeContext> GetRootTypes();
    }

    /// <summary>
    /// This interface is needed for CSharpNodeVisitor.Compilation
    /// TODO:  Evaluate remove it
    /// </summary>
    public interface ISyntaxTreeContext<TCompilationContext>
    {
        TCompilationContext Compilation { get; }
    }
}
