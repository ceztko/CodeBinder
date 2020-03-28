// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBinder.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CodeBinder
{
    class ProjectConverter : ConverterActual
    {
        public Project Project { get; private set; }

        internal ProjectConverter(Converter converter, Project project)
            : base(converter)
        {
            Project = project;
        }

        override internal protected IEnumerable<ConversionDelegate> GetConversionDelegates()
        {
            // Add some language specific preprocessor options
            var options = (CSharpParseOptions)Project.ParseOptions!;
            var preprocessorSymbols = options.PreprocessorSymbolNames;
            if (Converter.Options.PreprocessorDefinitionsRemoved != null)
                preprocessorSymbols = preprocessorSymbols.Except(Converter.Options.PreprocessorDefinitionsRemoved).ToArray();

            options = options.WithPreprocessorSymbols(preprocessorSymbols
                .Concat(new[] { "CODE_BINDER" })
                .Concat(Converter.Conversion.PreprocessorDefinitions)
                .Concat(Converter.Options.PreprocessorDefinitionsAdded ?? new string[0]));
            var project = Project.WithParseOptions(options);

            var solutionFilePath = project.Solution.FilePath;
            var solutionDir = Path.GetDirectoryName(solutionFilePath);
            var compilation = project.GetCompilationAsync().Result!;
            if (!Converter.Options.IgnoreCompilationErrors)
            {
                string? errors = CompilationOuput.ErrorsForCompilation(compilation, "source");
                if (!string.IsNullOrEmpty(errors))
                    throw new Exception("Compilation errors: " + errors);
            }

            // Select only syntax tress that belongs to solution dir, if not null
            var syntaxTrees = solutionDir == null ? compilation.SyntaxTrees.ToArray() : compilation.SyntaxTrees.Where(tree => tree.FilePath.StartsWith(solutionDir)).ToArray();
            var compilationContext = Converter.Conversion.GetCompilationContext(compilation);

            var syntaxTreeContextTypes = getSyntaxTreeContextTypes(compilationContext, syntaxTrees);
            foreach (var pair in syntaxTreeContextTypes)
            {
                foreach (var type in pair.Value)
                {
                    foreach (var conversion in type.Conversions)
                        yield return new ConversionDelegate(pair.Key, conversion);
                }
            }

            // Convert also non-syntax tree types
            foreach (var type in compilationContext.RootTypes)
            {
                foreach (var conversion in type.Conversions)
                    yield return new ConversionDelegate(conversion);
            }

            // Emit also out of context items
            foreach (var defaultConversion in compilationContext.ConversionDelegates)
                yield return defaultConversion;
        }

        private Dictionary<string, List<TypeContext>> getSyntaxTreeContextTypes(CompilationContext compilation, IEnumerable<SyntaxTree> syntaxTrees)
        {
            var syntaxTreeContexts = new Dictionary<string, SyntaxTreeContext>();

            var errorBuilder = new StringBuilder();
            // Visit trees and create contexts
            foreach (var tree in syntaxTrees)
            {
                var context = compilation.CreateSyntaxTreeContext();
                var visitor = context.CreateVisitor();
                context.SyntaxTree = tree;
                visitor.Visit(tree);
                foreach (var error in visitor.Errors)
                    errorBuilder.AppendLine(error);

                var treeFilePath = tree.FilePath ?? "";
                syntaxTreeContexts.Add(treeFilePath, context);
            }

            if (errorBuilder.Length != 0)
                throw new Exception(errorBuilder.ToString());

            // FIX-ME: Remove me, adn move to the visitor a similar mechanism
            compilation.AfterVisit();

            var ret = new Dictionary<string, List<TypeContext>>();
            foreach (var pair in syntaxTreeContexts)
            {
                foreach (var type in pair.Value.RootTypes)
                {
                    List<TypeContext>? types;
                    if (!ret.TryGetValue(pair.Key, out types))
                    {
                        types = new List<TypeContext>();
                        ret.Add(pair.Key, types);
                    }

                    types.Add(type);
                }
            }

            return ret;
        }
    }
}
