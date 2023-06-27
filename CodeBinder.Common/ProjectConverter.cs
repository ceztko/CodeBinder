// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto<ceztko@gmail.com>
// SPDX-FileCopyrightText: (C) 2017-2018 ICSharpCode
// SPDX-License-Identifier: MIT
using System.IO;
using System.Linq;
using System.Text;

namespace CodeBinder;

class ProjectConverter
{
    public Microsoft.CodeAnalysis.Project Project { get; private set; }

    public Converter Converter { get; private set; }

    public ProjectConverter(Converter converter, Microsoft.CodeAnalysis.Project project)
    {
        Project = project;
        Converter = converter;
    }

    public IEnumerable<ConversionDelegate> GetConversionDelegates()
    {
        // Add some language specific preprocessor options
        var options = (CSharpParseOptions)Project.ParseOptions!;
        var preprocessorSymbols = options.PreprocessorSymbolNames;
        if (Converter.Options.PreprocessorDefinitionsRemoved != null)
            preprocessorSymbols = preprocessorSymbols.Except(Converter.Options.PreprocessorDefinitionsRemoved).ToArray();

        var solutionFilePath = Project.Solution.FilePath;
        var solutionDir = Path.GetDirectoryName(solutionFilePath);
        var compilation = Project.GetCompilationAsync().Result!;
        if (!Converter.Options.IgnoreCompilationErrors)
        {
            string? errors = CompilationOuput.ErrorsForCompilation(compilation, "source");
            if (!errors.IsNullOrEmpty())
                throw new Exception("Compilation errors: " + errors);
        }

        // Select only syntax tress that belongs to solution dir, if not null
        var syntaxTrees = solutionDir == null ? compilation.SyntaxTrees.ToArray() : compilation.SyntaxTrees.Where(tree => tree.FilePath.StartsWith(solutionDir)).ToArray();
        var compilationContext = Converter.Conversion.GetCompilationContext(compilation);

        var visitor = compilationContext.CreateVisitor();
        var errorBuilder = new StringBuilder();
        // Visit trees and create contexts
        foreach (var tree in syntaxTrees)
            visitor.Visit(tree);

        foreach (var error in visitor.Errors)
            errorBuilder.AppendLine(error);

        if (errorBuilder.Length != 0)
            throw new Exception(errorBuilder.ToString());

        visitor.AfterVisit();

        foreach (var type in compilationContext.RootTypes)
        {
            foreach (var conversion in type.Conversions)
                yield return new ConversionDelegate(conversion);
        }

        // Emit also out of context items
        foreach (var defaultConversion in compilationContext.ConversionDelegates)
            yield return defaultConversion;
    }
}
