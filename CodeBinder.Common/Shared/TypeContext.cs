// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    /// <summary>
    /// TypeContext coupled to specific CompilationContext
    /// </summary>
    /// <remarks>If needed a more specific CSharp conversion type context inherit
    /// CSharpBaseTypeContext or CSharpTypeContext</remarks> 
    public abstract class TypeContext<TTypeContext, TCompilationContext> : TypeContext<TTypeContext>
        where TTypeContext : TypeContext
        where TCompilationContext : CompilationContext
    {
        public new TCompilationContext Compilation => getCompilationContext();

        protected TypeContext() { }

        protected abstract TCompilationContext getCompilationContext();

        protected sealed override CompilationContext GetCompilationContext()
        {
            return getCompilationContext();
        }
    }

    /// <remarks>Inherited by CSharpBaseTypeContext together with ITypeContext</remarks>
    public abstract class TypeContext<TTypeContext> : TypeContext
    where TTypeContext : TypeContext
    {
        private List<TTypeContext> _Children;

        public TTypeContext? Parent { get; internal set; }

        internal TypeContext()
        {
            _Children = new List<TTypeContext>();
        }

        internal void AddChild(TTypeContext child)
        {
            _Children.Add(child);
        }

        internal protected override TypeConversion CreateConversion()
        {
            return createConversion();
        }

        protected abstract TypeConversion<TTypeContext> createConversion();

        public new IReadOnlyList<TTypeContext> Children
        {
            get { return _Children; }
        }

        protected override IEnumerable<TypeContext> GetChildren()
        {
            return _Children;
        }
    }

    public abstract class TypeContext : ICompilationContextProvider
    {
        internal TypeContext() { }

        public CompilationContext Compilation
        {
            get { return GetCompilationContext(); }
        }

        public IEnumerable<TypeContext> Children
        {
            get { return GetChildren(); }
        }

        protected abstract CompilationContext GetCompilationContext();

        /// <summary>
        /// Create a conversion for this type.
        /// Overrides this method to extend hiearchy. See CSharpTypeContext/CSharpBaseTypeContext
        /// </summary>
        internal protected abstract TypeConversion CreateConversion();

        protected abstract IEnumerable<TypeContext> GetChildren();
    }
}
