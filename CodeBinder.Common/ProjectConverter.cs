// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto<ceztko@gmail.com>
// SPDX-FileCopyrightText: (C) 2017-2018 ICSharpCode
// SPDX-License-Identifier: MIT
using System.IO;
using System.Linq;
using System.Text;

namespace CodeBinder;

class ProjectConverter
{
    static readonly string[] PreprocessorDefinitionsToRemove = new string[] {
        "NETFRAMEWORK",
        "NET452",
        "NET20_OR_GREATER",
        "NET30_OR_GREATER",
        "NET35_OR_GREATER",
        "NET40_OR_GREATER",
        "NET45_OR_GREATER",
        "NET451_OR_GREATER",
        "NET452_OR_GREATER",
    };

    public Microsoft.CodeAnalysis.Project Project { get; private set; }

    public Converter Converter { get; private set; }

    public ProjectConverter(Converter converter, Microsoft.CodeAnalysis.Project project)
    {
        Project = project;
        Converter = converter;
    }

    public IEnumerable<ConversionDelegate> GetConversionDelegates()
    {
        var conversion = Converter.Conversion;

        // Add some language specific preprocessor options
        var options = (CSharpParseOptions)Project.ParseOptions!;

        var preprocessorSymbols = new HashSet<string>(options.PreprocessorSymbolNames);
        preprocessorSymbols.ExceptWith(PreprocessorDefinitionsToRemove);
        preprocessorSymbols.Add("CODE_BINDER");
        if (conversion.PreprocessorDefinitions.Count != 0)
        {
            foreach (var definition in conversion.PreprocessorDefinitions)
                preprocessorSymbols.Add(definition);
        }

        options = options.WithPreprocessorSymbols(preprocessorSymbols);
        var project = Project.WithParseOptions(options);

        var solutionFilePath = project.Solution.FilePath;
        var solutionDir = Path.GetDirectoryName(solutionFilePath);
        var compilation = project.GetCompilationAsync().Result!;
        checkCompilationErrors(compilation);

        // Select only syntax tress that belongs to solution dir, if not null

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
