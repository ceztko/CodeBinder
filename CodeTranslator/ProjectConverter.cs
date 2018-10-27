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
        private readonly Compilation _sourceCompilation;
        private readonly IEnumerable<SyntaxTree> _syntaxTreesToConvert;
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly Dictionary<string, SyntaxTreeContext> _firstPassResults = new Dictionary<string, SyntaxTreeContext>();

        protected ProjectConverter(Project project)
        {
            var solutionFilePath = project.Solution.FilePath;
            var solutionDir = Path.GetDirectoryName(solutionFilePath);
            var compilation = project.GetCompilationAsync().GetAwaiter().GetResult();
            var syntaxTrees = solutionDir == null ? compilation.SyntaxTrees : compilation.SyntaxTrees.Where(t => t.FilePath.StartsWith(solutionDir));

            _sourceCompilation = compilation;
            _syntaxTreesToConvert = syntaxTrees.ToList();
        }

        public static ProjectConverter<TLanguageConversion> CreateFor<TLanguageConversion>(Project project, IProgress<string> progress = null)
            where TLanguageConversion : LanguageConversion, new()
        {
            return new ProjectConverter<TLanguageConversion>(project, new TLanguageConversion());
        }

        public override IEnumerable<ConversionResult> Convert()
        {
            foreach (var pair in convert())
            {
                var errors = _errors.TryRemove(pair.Key, out var nonFatalException)
                    ? new[] {nonFatalException}
                    : new string[0];

                foreach (var type in pair.Value)
                {
                    var conversion = type.Conversion;
                    yield return new ConversionResult(conversion.ToFullString())
                    {
                        SourcePath = pair.Key,
                        TargetFileName = conversion.FileName,
                        TargetBasePath = conversion.BasePath,
                        Exceptions = errors
                    };
                }
            }

            foreach (var error in _errors)
            {
                yield return new ConversionResult {SourcePath = error.Key, Exceptions = new []{ error.Value } };
            }
        }

        private Dictionary<string, List<TypeContext>> convert()
        {
            FirstPass();
            var secondPassByFilePath = SecondPass();
#if DEBUG && ShowCompilationErrors
            AddProjectWarnings();
#endif
            return secondPassByFilePath;
        }

        private Dictionary<string, List<TypeContext>> SecondPass()
        {
            var secondPassByFilePath = new Dictionary<string, List<TypeContext>>();
            foreach (var pair in _firstPassResults)
            {
                var treeFilePath = pair.Key;
                List<TypeContext> units;
                if (!secondPassByFilePath.TryGetValue(treeFilePath, out units))
                {
                    units = new List<TypeContext>();
                    secondPassByFilePath.Add(treeFilePath, units);
                }

                foreach (var unit in Conversion.SecondPass(pair.Value))
                    units.Add(unit);
            }
            return secondPassByFilePath;
        }

        private void AddProjectWarnings()
        {
            var nonFatalWarningsOrNull = Conversion.GetWarningsOrNull();
            if (!string.IsNullOrWhiteSpace(nonFatalWarningsOrNull))
            {
                var warningsDescription = Path.Combine(_sourceCompilation.AssemblyName, "ConversionWarnings.txt");
                _errors.TryAdd(warningsDescription, nonFatalWarningsOrNull);
            }
        }

        private void FirstPass()
        {
            foreach (var tree in _syntaxTreesToConvert)
            {
                var treeFilePath = tree.FilePath ?? "";
                try
                {
                    SingleFirstPass(tree, treeFilePath);
                }
                catch (Exception e)
                {
                    _errors.TryAdd(treeFilePath, e.ToString());
                }
            }
        }

        private void SingleFirstPass(SyntaxTree tree, string treeFilePath)
        {
            var currentSourceCompilation = _sourceCompilation;
            var convertedTree = Conversion.FirstPass(currentSourceCompilation, tree);
            _firstPassResults.Add(treeFilePath, convertedTree);
        }
    }

    public class ProjectConverter<TLanguageConversion> : ProjectConverter
            where TLanguageConversion : LanguageConversion
    {
        TLanguageConversion _Conversion;

        internal ProjectConverter(Project project, TLanguageConversion conversion)
            : base(project)
        {
            _Conversion = conversion;
        }

        public new TLanguageConversion Conversion => _Conversion;

        protected override LanguageConversion GetConversion()
        {
            return _Conversion;
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
