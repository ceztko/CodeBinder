// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using CodeBinder.Shared;
using CodeBinder.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeBinder.Java
{
    abstract class JavaConversionWriter : ConversionWriter
    {
        public ConversionCSharpToJava Conversion { get; private set; }

        public JavaConversionWriter(ConversionCSharpToJava conversion)
        {
            Conversion = conversion;
        }
    }

    abstract class JavaConversionWriterBase : ConversionWriter
    {
        protected override string? GetGeneratedPreamble() => ConversionCSharpToJava.SourcePreamble;
    }
}
