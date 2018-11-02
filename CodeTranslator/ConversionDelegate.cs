// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Shared;
using CodeTranslator.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeTranslator
{
    public class ConversionDelegate
    {
        IConversionBuilder _builder;

        public string SourcePath { get; private set; }

        public IReadOnlyList<string> Exceptions { get; private set; }

        internal ConversionDelegate(string sourcePath, IConversionBuilder builder, IReadOnlyList<string> exceptions)
        {
            SourcePath = sourcePath;
            _builder = builder;
            Exceptions = exceptions;
        }

        public string TargetFileName
        {
            get { return _builder.FileName; }
        }

        public string TargetBasePath
        {
            get { return _builder.BasePath; }
        }

        public void Write(Stream stream)
        {
            var writer = new StreamWriter(stream);
            write(writer);
        }

        public string ToFullString()
        {
            var writer = new StringWriter();
            write(writer);
            return writer.ToString();
        }

        private void write(TextWriter writer)
        {
            var codeBuilder = new CodeBuilder(writer);
            codeBuilder.AppendLine(_builder.GeneratedPreamble);
            _builder.Write(codeBuilder);
        }
    }
}
