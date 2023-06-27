// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Utils;
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
        public override abstract TCompilationContext Compilation { get; }

        protected TypeContext() { }
    }

    /// <remarks>Inherited by CSharpBaseTypeContext together with ITypeContext</remarks>
    public abstract class TypeContext<TTypeContext> : TypeContext
    where TTypeContext : TypeContext
    {
        private List<TTypeContext> _Children;

        public new TTypeContext? Parent { get; internal set; }

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

        protected override sealed TypeContext? GetParent()
        {
            return Parent;
        }
    }

    [DebuggerDisplay("Name = {Name}")]
    public abstract class TypeContext : ICompilationContextProvider
    {
        internal TypeContext() { }

        public abstract CompilationContext Compilation { get; }

        public IEnumerable<TypeContext> Children => GetChildren();

        /// <summary>
        /// Create a conversion for this type.
        /// Overrides this method to extend hiearchy. See CSharpTypeContext/CSharpBaseTypeContext
        /// </summary>
        protected abstract IEnumerable<TypeConversion> GetConversions();

        internal IEnumerable<TypeConversion> Conversions => GetConversions();

        protected abstract IEnumerable<TypeContext> GetChildren();

        protected abstract TypeContext? GetParent();

        public abstract string Name { get; }

        public virtual string FullName => Name;

        public TypeContext? Parent => GetParent();
    }

    public interface ITypeContext<TCompilationContext>
        where TCompilationContext : CompilationContext
    {
        TCompilationContext Compilation { get; }
    }
}
