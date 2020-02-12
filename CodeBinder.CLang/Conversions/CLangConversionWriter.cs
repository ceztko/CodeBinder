using CodeBinder.CLang;
using CodeBinder.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.CLang
{
    abstract class CLangConversionWriter : ConversionWriter
    {
        public CLangCompilationContext Compilation { get; private set; }

        public CLangConversionWriter(CLangCompilationContext compilation)
        {
            Compilation = compilation;
        }
    }
}
