// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
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
