// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto<ceztko@gmail.com>
// SPDX-FileCopyrightText: (C) 2017-2018 ICSharpCode
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis;
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
        checkCompilationErrors(compilation);

        // Select only syntax tress that belongs to solution dir, if not null
        var conversion = Converter.Conversion;
        var validationContext = conversion.CreateValidationContext();
        var syntaxTrees = filterTrees(compilation, solutionDir);
        if (validationContext != null)
        {
            var validationVisitor = conversion.CreateVisitor();
            validationContext.Init(new CompilationProvider(compilation), validationVisitor);
            validationVisitor.Visit(syntaxTrees);

            var errorBuilder = new StringBuilder();
            foreach (var error in validationContext.Errors)
                errorBuilder.AppendLine(error);

            if (errorBuilder.Length != 0)
                throw new Exception(errorBuilder.ToString());

            if (validationContext.Replacements.Count != 0)
            {
                compilation = doReplacements(compilation, ref syntaxTrees, validationContext.Replacements);
                checkCompilationErrors(compilation);
            }
        }

        var collectionVisitor = conversion.CreateVisitor();
        // Visit trees and create contexts
        var compilationContext = conversion.CreateCompilationContext();
        compilationContext.Compilation = compilation;
        var collectionContext = compilationContext.CreateCollectionContext();
        collectionContext.Init(collectionVisitor);
        collectionVisitor.Visit(syntaxTrees);

        foreach (var type in compilationContext.RootTypes)
        {
            foreach (var conv in type.Conversions)
                yield return new ConversionDelegate(conv);
        }

        // Emit also out of context items
        foreach (var defaultConversion in compilationContext.ConversionDelegates)
            yield return defaultConversion;
    }

    void checkCompilationErrors(Compilation compilation)
    {
        if (!Converter.Options.IgnoreCompilationErrors)
        {
            string? errors = CompilationOuput.ErrorsForCompilation(compilation, "source");
            if (!errors.IsNullOrEmpty())
                throw new Exception("Compilation errors: " + errors);
        }
    }

    static SyntaxTree[] filterTrees(Compilation compilation, string? solutionDir)
    {
        if (solutionDir == null)
            return compilation.SyntaxTrees.ToArray();
        else
            return compilation.SyntaxTrees.Where(tree => tree.FilePath.StartsWith(solutionDir)).ToArray();
    }

    static Compilation doReplacements(Compilation compilation, ref SyntaxTree[] syntaxTress,
        IReadOnlyDictionary<SyntaxTree, IReadOnlyDictionary<SyntaxNode, IReadOnlyList<NodeReplacementAction>>> allReplacements)
    {
        var treeMap = syntaxTress.ToDictionary((source) => source);
        foreach (var pair in allReplacements)
        {
            // Execute all replacement actions, retrieving the modified syntax tree
            var nodeReplacements = pair.Value;
            treeMap[pair.Key] = pair.Key.GetRoot().ReplaceNodes(nodeReplacements.Keys,
                (original, rewritten) =>
                {
                    var replacements = nodeReplacements[original];
                    // Apply replacements in reversed order
                    for (int i = 0; i < replacements.Count; i++)
                    {
                        var replacement = replacements[i];
                        var options = new ReplacementOptions();
                        rewritten = replacement(rewritten, options);
                        if (!options.SkipNormalization)
                            rewritten = rewritten.NormalizeWhitespace();
                    }

                    return rewritten;
                }).SyntaxTree;
        }

        foreach (var pair in treeMap)
        {
            if (pair.Key != pair.Value)
                compilation = compilation.ReplaceSyntaxTree(pair.Key, pair.Value);
        }

        // Return the modified syntax trees
        syntaxTress = treeMap.Values.ToArray();
        return compilation;
    }
}
