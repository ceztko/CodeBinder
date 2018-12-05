// Copyright (c) 2017-2018 ICSharpCode
// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CodeBinder.Shared;
using Microsoft.CodeAnalysis;

namespace CodeBinder
{
    public class SolutionConverter<TConversion> : Converter<TConversion>
         where TConversion : LanguageConversion
    {
        readonly string _solutionFilePath;
        readonly IReadOnlyCollection<Project> _projectsToConvert;
        readonly IProgress<string> _progress;

        internal SolutionConverter(Solution solution, IEnumerable<Project> projectsToConvert,
            TConversion conversion, IProgress<string> showProgressMessage)
            : base(conversion)
        {
            _solutionFilePath = solution.FilePath;
            _projectsToConvert = projectsToConvert.ToList();
            _progress = showProgressMessage ?? new Progress<string>();
        }

        internal protected override IEnumerable<ConversionDelegate> Convert()
        {
            return _projectsToConvert.SelectMany(project => ConvertProject(project));
        }

        private IEnumerable<ConversionDelegate> ConvertProject(Project project)
        {
            _progress.Report($"Converting {project.Name}, this may take a some time...");
            return new ProjectConverter<LanguageConversion>(project, Conversion).Convert();
        }
    }
}
