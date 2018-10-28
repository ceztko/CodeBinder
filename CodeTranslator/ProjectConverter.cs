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
        private readonly SourceCompilation _compilation;
        private readonly IEnumerable<SyntaxTree> _syntaxTreesToConvert;
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();

        protected ProjectConverter(Project project)
        {
            var solutionFilePath = project.Solution.FilePath;
            var solutionDir = Path.GetDirectoryName(solutionFilePath);
            var compilation = project.GetCompilationAsync().GetAwaiter().GetResult();
            var syntaxTrees = solutionDir == null ? compilation.SyntaxTrees : compilation.SyntaxTrees.Where(t => t.FilePath.StartsWith(solutionDir));
            _compilation = new SourceCompilation(compilation);
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
            var syntaxTreeContexts = new Dictionary<string, SyntaxTreeContext>();
            foreach (var tree in _syntaxTreesToConvert)
            {
                var syntaxTree = Conversion.GetSyntaxTreeContext(_compilation);

                var treeFilePath = tree.FilePath ?? "";
                try
                {
                    syntaxTree.Visit(tree);
                    syntaxTreeContexts.Add(treeFilePath, syntaxTree);
                }
                catch (Exception e)
                {
                    _errors.TryAdd(treeFilePath, e.ToString());
                }
            }

            var ret = new Dictionary<string, List<TypeContext>>();
            foreach (var pair in syntaxTreeContexts)
            {
                foreach (var type in pair.Value.GetRootTypes())
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

#if DEBUG
            AddProjectWarnings();
#endif
            return ret;
        }

        private void AddProjectWarnings()
        {
            var nonFatalWarningsOrNull = Conversion.GetWarningsOrNull(_compilation);
            if (!string.IsNullOrWhiteSpace(nonFatalWarningsOrNull))
            {
                var warningsDescription = Path.Combine(_compilation.Compilation.AssemblyName, "ConversionWarnings.txt");
                _errors.TryAdd(warningsDescription, nonFatalWarningsOrNull);
            }
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
