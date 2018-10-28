// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using CodeTranslator.Shared;
using Microsoft.CodeAnalysis;

namespace CodeTranslator.Util
{
    internal static class CompilationWarnings
    {
        public static string WarningsForCompilation(SourceCompilation finalCompilation, string compilationDescription)
        {
            var targetErrors = GetDiagnostics(finalCompilation);
            return targetErrors.Any()
                ? $"{targetErrors.Count} {compilationDescription} compilation errors:{Environment.NewLine}{String.Join(Environment.NewLine, targetErrors)}"
                : null;
        }

        private static List<string> GetDiagnostics(SourceCompilation compilation)
        {
            var diagnostics = compilation.Compilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => $"{d.Id}: {d.GetMessage()}")
                .ToList();
            return diagnostics;
        }
    }
}
