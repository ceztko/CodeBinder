// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator
{
    public abstract class TypeContext : ISemanticModelProvider
    {
        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return TreeContext.GetSemanticModel(tree);
        }

        public SyntaxTreeContext TreeContext
        {
            get { return GetTreeContext(); }
        }

        public TypeConversion Conversion
        {
            get { return GetConversion(); }
        }

        protected abstract SyntaxTreeContext GetTreeContext();

        protected abstract TypeConversion GetConversion();
    }
}
