// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-FileCopyrightText: (C) 2017-2018 ICSharpCode
// SPDX-License-Identifier: MIT
using System.Linq;

namespace CodeBinder.Utils;

internal static class CompilationOuput
{
    public static string? ErrorsForCompilation(Compilation compilation, string compilationDescription)
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
            .Select(d => $"{d.Location}, {d.Id}: {d.GetMessage()}")
            .ToList();
        return diagnostics;
    }
}
