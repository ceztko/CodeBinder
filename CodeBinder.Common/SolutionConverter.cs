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
    class SolutionConverter : ConverterActual
    {
        readonly IReadOnlyCollection<Project> _projectsToConvert;
        readonly IProgress<string> _progress;

        internal SolutionConverter(Converter converter, Solution solution, IEnumerable<Project> projectsToConvert,
            IProgress<string>? showProgressMessage)
            : base(converter)
        {
            _projectsToConvert = projectsToConvert.ToList();
            _progress = showProgressMessage ?? new Progress<string>();
        }

        internal protected override IEnumerable<ConversionDelegate> GetConversionDelegates()
        {
            return _projectsToConvert.SelectMany(project => ConvertProject(project));
        }

        private IEnumerable<ConversionDelegate> ConvertProject(Project project)
        {
            _progress.Report($"Converting {project.Name}, this may take a some time...");
            return new ProjectConverter(Converter, project).GetConversionDelegates();
        }
    }
}
