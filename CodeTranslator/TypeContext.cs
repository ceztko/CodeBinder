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
        public TypeConversion Conversion { get; internal set; }

        public string ToFullString()
        {
            var builder = new IndentStringBuilder();
            builder.AppendLine(Conversion.GeneratedPreamble);
            Conversion.Write(builder);
            return builder.ToString();
        }

        public SyntaxTreeContext TreeContext
        {
            get { return GetTreeContext(); }
        }

        public string FileName
        {
            get { return Conversion.FileName; }
        }

        public string BasePath
        {
            get { return Conversion.BasePath; }
        }

        protected abstract SyntaxTreeContext GetTreeContext();

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            return TreeContext.GetSemanticModel(tree);
        }
    }
}
