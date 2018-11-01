// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public abstract class TypeConversion : ICompilationContextProvider
    {
        internal TypeConversion() { }

        public string ToFullString()
        {
            var builder = new CodeBuilder();
            builder.AppendLine(GeneratedPreamble);
            Write(builder);
            return builder.ToString();
        }

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return Compilation.GetSemanticModel(tree);
        }

        public abstract void Write(CodeBuilder builder);

        public virtual string GeneratedPreamble
        {
            get { return string.Empty; }
        }

        public abstract string FileName
        {
            get;
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
