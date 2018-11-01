using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public abstract class BaseWriter : ISemanticModelProvider
    {
        public ISemanticModelProvider Context { get; private set; }
        public CodeBuilder Builder { get; private set; }

        protected BaseWriter(ISemanticModelProvider context)
        {
            Context = context;
        }

        public void Write(CodeBuilder builder)
        {
            Builder = builder;
            Write();
            Builder = null;
        }

        protected abstract void Write();

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return Context.GetSemanticModel(tree);
        }
    }
}
