// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeBinder.Shared;
using CodeBinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CodeBinder
{
    public class ProjectConverter<TConversion> : Converter<TConversion>
         where TConversion : LanguageConversion
    {
        Project _project;

        internal ProjectConverter(Project project, TConversion conversion)
            : base(conversion)
        {
            _project = project;
        }

        internal protected override IEnumerable<ConversionDelegate> GetConversionDelegates()
        {
            // Add some language specific preprocessor options
            var options = (CSharpParseOptions)_project.ParseOptions!;
            var preprocessorSymbols = options.PreprocessorSymbolNames;
            if (Options.PreprocessorDefinitionsRemoved != null)
                preprocessorSymbols = preprocessorSymbols.Except(Options.PreprocessorDefinitionsRemoved).ToArray();

            options = options.WithPreprocessorSymbols(preprocessorSymbols
                .Concat(new[] { "CODE_BINDER" })
                .Concat(Conversion.PreprocessorDefinitions)
                .Concat(Options.PreprocessorDefinitionsAdded ?? new string[0]));
            var project = _project.WithParseOptions(options);

            var solutionFilePath = project.Solution.FilePath;
            var solutionDir = Path.GetDirectoryName(solutionFilePath);
            var compilation = project.GetCompilationAsync().Result!;
            if (!Options.IgnoreCompilationErrors)
            {
                string? errors = CompilationOuput.ErrorsForCompilation(compilation, "source");
                if (!string.IsNullOrEmpty(errors))
                    throw new Exception("Compilation errors: " + errors);
            }

            // Select only syntax tress that belongs to solution dir, if not null
            var syntaxTrees = solutionDir == null ? compilation.SyntaxTrees.ToArray() : compilation.SyntaxTrees.Where(tree => tree.FilePath.StartsWith(solutionDir)).ToArray();
            var compilationContext = Conversion.GetCompilationContext(compilation);

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

            // Visit trees and create contexts
            foreach (var tree in syntaxTrees)
            {
                var context = compilation.CreateSyntaxTreeContext();
                var visitor = context.CreateVisitor();
                context.SyntaxTree = tree;
                visitor.Visit(tree);
                var treeFilePath = tree.FilePath ?? "";
                syntaxTreeContexts.Add(treeFilePath, context);
            }

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
