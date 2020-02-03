using CodeBinder.CLang;
using CodeBinder.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    abstract class CLangCompilationContextBuilder : ConversionBuilder
    {
        public CLangCompilationContext Compilation { get; private set; }

        public CLangCompilationContextBuilder(CLangCompilationContext compilation)
        {
            Compilation = compilation;
        }
    }
}
