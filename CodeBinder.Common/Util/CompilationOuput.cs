// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using CodeBinder.Shared;
using Microsoft.CodeAnalysis;

namespace CodeBinder.Util
{
    internal static class CompilationOuput
    {
        public static string ErrorsForCompilation(Compilation compilation, string compilationDescription)
        {
            var targetErrors = GetDiagnostics(compilation);
            return targetErrors.Any()
                ? $"{targetErrors.Count} {compilationDescription} compilation errors:{Environment.NewLine}{String.Join(Environment.NewLine, targetErrors)}"
                : null;
        }

        private static List<string> GetDiagnostics(Compilation compilation)
        {
            var diagnostics = compilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => $"{d.Id}: {d.GetMessage()}")
                .ToList();
            return diagnostics;
        }
    }
}
