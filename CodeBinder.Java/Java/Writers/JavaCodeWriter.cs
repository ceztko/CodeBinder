﻿// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Java
{
    abstract class JavaCodeWriter<TItem> : CodeWriter<TItem, JavaCodeConversionContext>
    {
        protected JavaCodeWriter(TItem item, JavaCodeConversionContext context)
            : base(item, context) { }
    }

    // NOTE: This class is needed since the conversion doesn't use a custom CompilationContext
    // so we can't propagate CSharCompilationContext to access the ConversionCSharpToJava
    public class JavaCodeConversionContext : ICompilationContextProvider
    {
        public ICompilationContextProvider Provider { get; private set; }
        public ConversionCSharpToJava Conversion { get; private set; }

        public JavaCodeConversionContext(ICompilationContextProvider provider, ConversionCSharpToJava conversion)
        {
            Provider = provider;
            Conversion = conversion;
        }

        public CompilationContext Compilation
        {
            get { return Provider.Compilation; }
        }
    }
}
