// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    public abstract class SyntaxTreeContext : ICompilationContextProvider
    {
        public CompilationContext Compilation
        {
            get { return GetCompilationContext(); }
        }

        internal SyntaxTreeContext() { }

        public abstract void Visit(SyntaxTree node);

        public IEnumerable<TypeContext> RootTypes
        {
            get { return GetRootTypes(); }
        }

        protected abstract CompilationContext GetCompilationContext();

        protected abstract IEnumerable<TypeContext> GetRootTypes();
    }
}
