// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Text;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;

namespace CodeTranslator
{
    public abstract class LanguageConversion
    {
        public Compilation Compilation { get; private set; }

        public SyntaxTreeContext FirstPass(Compilation compilation, SyntaxTree tree)
        {
            Compilation = compilation;
            return firstPass(compilation, tree);
        }

        public abstract IEnumerable<TypeContext> SecondPass(SyntaxTreeContext context);

        public virtual string GetWarningsOrNull()
        {
            return CompilationWarnings.WarningsForCompilation(Compilation, "source");
        }

        public virtual IEnumerable<ConversionResult> DefaultResults
        {
            get { yield break; }
        }

        protected abstract SyntaxTreeContext firstPass(Compilation compilation, SyntaxTree tree);
    }

    public abstract class LanguageConversion<TSyntaxTreeContext, TTypeContext, TNodeVisistor> : LanguageConversion
        where TSyntaxTreeContext : SyntaxTreeContext, new()
        where TTypeContext : TypeContext
        where TNodeVisistor : NodeVisitor<TSyntaxTreeContext>, new()
    {
        protected sealed override SyntaxTreeContext firstPass(Compilation compilation, SyntaxTree tree)
        {
            var context = new TSyntaxTreeContext();
            context.Compilation = compilation;
            var visitor = new TNodeVisistor();
            visitor.Context = context;
            visitor.Visit(tree.GetRoot());
            return context;
        }

        public sealed override IEnumerable<TypeContext> SecondPass(SyntaxTreeContext context)
        {
            return SecondPass(context as TSyntaxTreeContext);
        }

        protected abstract IEnumerable<TTypeContext> SecondPass(TSyntaxTreeContext context);
    }
}
