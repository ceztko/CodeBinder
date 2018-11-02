// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public abstract class TypeConversion : IConversionBuilder, ICompilationContextProvider
    {
        internal TypeConversion() { }

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return Compilation.GetSemanticModel(tree);
        }

        public virtual void Write(CodeBuilder builder)
        {
            throw new NotImplementedException();
        }

        public virtual string GeneratedPreamble
        {
            get { return string.Empty; }
        }

        public virtual IEnumerable<IConversionBuilder> Builders
        {
            get { yield return this; }
        }

        public virtual string FileName
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string BasePath
        {
            get { return ""; }
        }

        public TypeContext TypeContext
        {
            get { return GetTypeContext(); }
        }

        public CompilationContext Compilation
        {
            get { return TypeContext.TreeContext.Compilation; }
        }

        protected abstract TypeContext GetTypeContext();
    }

    public abstract class TypeConversion<TTypeContext> : TypeConversion
        where TTypeContext : TypeContext
    {
        public new TTypeContext TypeContext
        {
            get;
            internal set;
        }

        protected override TypeContext GetTypeContext()
        {
            return TypeContext;
        }
    }
}
