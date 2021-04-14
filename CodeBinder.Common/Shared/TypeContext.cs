// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CodeBinder.Shared
{
    /// <summary>
    /// TypeContext coupled to specific CompilationContext
    /// </summary>
    /// <remarks>If needed a more specific CSharp conversion type context inherit
    /// CSharpBaseTypeContext or CSharpTypeContext</remarks> 
    public abstract class TypeContext<TTypeContext, TCompilationContext> : TypeContext<TTypeContext>, ITypeContext<TCompilationContext>
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

        protected override IEnumerable<TypeConversion> GetConversions()
        {
            return getConversions();
        }

        protected abstract IEnumerable<TypeConversion<TTypeContext>> getConversions();
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

        public new IReadOnlyList<TTypeContext> Children
        {
            get { return _Children; }
        }

        protected override IEnumerable<TypeContext> GetChildren()
        {
            return _Children;
        }
    }

    [DebuggerDisplay("Name = {Name}")]
    public abstract class TypeContext : ICompilationContextProvider
    {
        internal TypeContext() { }

        public CompilationContext Compilation => GetCompilationContext();

        public IEnumerable<TypeContext> Children => GetChildren();

        protected abstract CompilationContext GetCompilationContext();

        /// <summary>
        /// Create a conversion for this type.
        /// Overrides this method to extend hiearchy. See CSharpTypeContext/CSharpBaseTypeContext
        /// </summary>
        protected abstract IEnumerable<TypeConversion> GetConversions();

        internal IEnumerable<TypeConversion> Conversions => GetConversions();

        protected abstract IEnumerable<TypeContext> GetChildren();

        public abstract string Name { get; }
    }

    public interface ITypeContext<TCompilationContext>
        where TCompilationContext : CompilationContext
    {
        TCompilationContext Compilation { get; }
    }
}
