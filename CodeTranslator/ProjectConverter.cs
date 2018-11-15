// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeTranslator.Shared;
using CodeTranslator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace CodeTranslator
{
    public abstract class ProjectConverter : Converter
    {
        Project _project;

        protected ProjectConverter(Project project)
        {
            _project = project;
        }

        public static ProjectConverter<TLanguageConversion> CreateFor<TLanguageConversion>(Project project, IProgress<string> progress = null)
            where TLanguageConversion : LanguageConversion, new()
        {
            return new ProjectConverter<TLanguageConversion>(project, new TLanguageConversion());
        }

        internal protected override IEnumerable<ConversionDelegate> Convert()
        {
            var solutionFilePath = _project.Solution.FilePath;
            var solutionDir = Path.GetDirectoryName(solutionFilePath);
            var compilation = _project.GetCompilationAsync().GetAwaiter().GetResult();
            if (!IgnoreCompilationErrors)
            {
                string errors = CompilationOuput.ErrorsForCompilation(compilation, "source");
                if (!string.IsNullOrEmpty(errors))
                    throw new Exception("Compilation errors: " + errors);
            }

            var syntaxTrees = solutionDir == null ? compilation.SyntaxTrees.ToArray() : compilation.SyntaxTrees.Where(t => t.FilePath.StartsWith(solutionDir)).ToArray();
            var compilationContext = new CompilationContext(compilation);

            var syntaxTreeContextTypes = getSyntaxTreeContextTypes(compilationContext, syntaxTrees);
            foreach (var pair in syntaxTreeContextTypes)
            {
                foreach (var type in pair.Value)
                {
                    foreach (var builder in type.Conversion.Builders)
                        yield return new ConversionDelegate(pair.Key, builder);
                }
            }

            // Convert also out-of-context types
            foreach (var type in compilationContext.RootTypes)
            {
                foreach (var builder in type.Conversion.Builders)
                    yield return new ConversionDelegate(builder);
            }
        }

        private Dictionary<string, List<TypeContext>> getSyntaxTreeContextTypes(CompilationContext compilation, IEnumerable<SyntaxTree> syntaxTrees)
        {
            var syntaxTreeContexts = new Dictionary<string, SyntaxTreeContext>();
            foreach (var tree in syntaxTrees)
            {
                var syntaxTree = Conversion.GetSyntaxTreeContext();
                syntaxTree.Compilation = compilation;

                var treeFilePath = tree.FilePath ?? "";
                syntaxTree.Visit(tree);
                syntaxTreeContexts.Add(treeFilePath, syntaxTree);
            }

            var ret = new Dictionary<string, List<TypeContext>>();
            foreach (var pair in syntaxTreeContexts)
            {
                foreach (var type in pair.Value.RootTypes)
                {
                    List<TypeContext> types;
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

    public class ProjectConverter<TLanguageConversion> : ProjectConverter
            where TLanguageConversion : LanguageConversion
    {
        public new TLanguageConversion Conversion { get; private set; }

        internal ProjectConverter(Project project, TLanguageConversion conversion)
            : base(project)
        {
            Conversion = conversion;
        }

        protected override LanguageConversion GetConversion()
        {
            return Conversion;
        }
    }

    class ProjectConverterSimple : ProjectConverter
    {
        LanguageConversion _Conversion;

        public ProjectConverterSimple(Project project, LanguageConversion conversion)
            : base(project)
        {
            _Conversion = conversion;
        }

        protected override LanguageConversion GetConversion()
        {
            return _Conversion;
        }
    }

}
