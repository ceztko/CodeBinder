// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeTranslator
{
    public class ConversionResult
    {
        private string _sourcePath;
        public bool Success { get; private set; }
        public string ConvertedCode { get; private set; }
        public IReadOnlyList<string> Exceptions { get; internal set; }

        public ConversionResult(string convertedCode = null)
        {
            Success = !string.IsNullOrWhiteSpace(convertedCode);
            ConvertedCode = convertedCode;
        }

        public string SourcePath
        {
            get => _sourcePath;
            set => _sourcePath = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public string TargetFileName { get; set; }

        public string TargetBasePath { get; set; }

        public ConversionResult(params Exception[] exceptions)
        {
            Success = exceptions.Length == 0;
            Exceptions = exceptions.Select(e => e.ToString()).ToList();
        }

        public string GetExceptionsAsString()
        {
            if (Exceptions == null || Exceptions.Count == 0)
                return String.Empty;

            var builder = new StringBuilder();

            for (int i = 0; i < Exceptions.Count; i++) {
                if (Exceptions.Count > 1) {
                    builder.AppendFormat("----- Exception {0} of {1} -----" + Environment.NewLine, i + 1, Exceptions.Count);
                }
                builder.AppendLine(Exceptions[i]);
            }
            return builder.ToString();
        }
    }
}
