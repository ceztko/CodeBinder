// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CodeTranslator.Shared;
using Microsoft.CodeAnalysis;

namespace CodeTranslator
{
    public abstract class SolutionConverter : Converter
    {
        readonly string _solutionFilePath;
        readonly IReadOnlyCollection<Project> _projectsToConvert;
        readonly IProgress<string> _progress;

        protected SolutionConverter(Solution solution, IEnumerable<Project> projectsToConvert,
            IProgress<string> showProgressMessage)
        {
            _solutionFilePath = solution.FilePath;
            _projectsToConvert = projectsToConvert.ToList();
            _progress = showProgressMessage ?? new Progress<string>();
        }

        public static SolutionConverter<TLanguageConversion> CreateFor<TLanguageConversion>(Solution solution, IProgress<string> progress = null)
            where TLanguageConversion : LanguageConversion, new()
        {
            return CreateFor<TLanguageConversion>(solution, solution.Projects, progress);
        }

        public static SolutionConverter<TLanguageConversion> CreateFor<TLanguageConversion>(IEnumerable<Project> projectsToConvert,
            IProgress<string> progress = null)
            where TLanguageConversion : LanguageConversion, new()
        {
            return CreateFor<TLanguageConversion>(null, projectsToConvert, progress);
        }

        private static SolutionConverter<TLanguageConversion> CreateFor<TLanguageConversion>(Solution solution,
            IEnumerable<Project> projectsToConvert, IProgress<string> progress)
            where TLanguageConversion : LanguageConversion, new()
        {
            if (solution == null)
                solution = projectsToConvert.First().Solution;

            return new SolutionConverter<TLanguageConversion>(solution, projectsToConvert, progress, new TLanguageConversion());
        }

        internal protected override IEnumerable<ConversionDelegate> Convert()
        {
            return _projectsToConvert.SelectMany(project => ConvertProject(project));
        }

        private IEnumerable<ConversionDelegate> ConvertProject(Project project)
        {
            _progress.Report($"Converting {project.Name}, this may take a some time...");
            return new ProjectConverterSimple(project, Conversion).Convert();
        }
    }

    public class SolutionConverter<TLanguageConversion> : SolutionConverter
        where TLanguageConversion : LanguageConversion
    {
        TLanguageConversion _Conversion;

        internal SolutionConverter(Solution solution, IEnumerable<Project> projectsToConvert,
            IProgress<string> showProgressMessage, TLanguageConversion conversion)
            : base(solution, projectsToConvert, showProgressMessage)
        {
            _Conversion = conversion;
        }

        protected override LanguageConversion GetConversion()
        {
            return _Conversion;
        }

        public new TLanguageConversion Conversion => _Conversion;
    }
}
