// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeBinder
{
    public class ConverterOptions
    {
        /// <summary>Plaform specific preprocessor symbols that will be added during compilation</summary>
        public IReadOnlyList<string>? PreprocessorDefinitionsAdded { get; set; }

        /// <summary>Preprocessor symbols that will be be removed during compilation</summary>
        public IReadOnlyList<string>? PreprocessorDefinitionsRemoved { get; set; }

        public bool IgnoreCompilationErrors { get; set; }
    }

    public class GeneratorOptions
    {
        public string? TargetRootPath;
        /// <summary>Create string before writing to file. For DEBUG</summary>
        public bool EagerStringConversion;
    }

    public abstract class Converter
    {
        const string DefineConstantsName = "DefineConstants";

        public ConverterOptions Options { get; private set; }

        internal Converter()
        {
            Options = new ConverterOptions();
        }

        public static Converter<TLanguageConversion> CreateFor<TLanguageConversion>()
            where TLanguageConversion : LanguageConversion, new()
        {
            return new Converter<TLanguageConversion>(new TLanguageConversion());
        }

        public void ConvertAndWrite(Project project, GeneratorOptions args, IProgress<string>? progress = null)
        {
            if (args.TargetRootPath == null)
                throw new ArgumentNullException("args.TargtetRootPath");

            Microsoft.CodeAnalysis.Project caproject;
            var workspace = createWorkspace();
            if (project.Solution == null)
            {
                caproject = workspace.OpenProjectAsync(project.FilePath).Result;
            }
            else
            {
                var casolution = workspace.OpenSolutionAsync(project.Solution.FilePath).Result;
                caproject = casolution.Projects.First((proj) => proj.Id.Id == project.Id);
            }
            
            convertAndWrite(new ProjectConverter(this, caproject), args);
        }

        public void ConvertAndWrite(Solution solution, GeneratorOptions args, IProgress<string>? progress = null)
        {
            if (args.TargetRootPath == null)
                throw new ArgumentNullException("args.TargtetRootPath");

            convertAndWrite(solution, solution.Projects, args);
        }

        public void ConvertAndWrite(IEnumerable<Project> projectsToConvert, GeneratorOptions args, IProgress<string>? progress = null)
        {
            if (args.TargetRootPath == null)
                throw new ArgumentNullException("args.TargtetRootPath");

            Solution? solution = null;
            foreach (var project in projectsToConvert)
            {
                if (solution == null)
                    solution = project.Solution;
                else if (solution != project.Solution)
                    throw new Exception("Projects must be afferent to the same solution");
            }

            if (solution == null)
                throw new Exception("Projects must be afferent to a solution");

            convertAndWrite(solution, projectsToConvert, args);
        }

        void convertAndWrite(Solution solution, IEnumerable<Project> projectsToConvert, GeneratorOptions args)
        {
            var workspace = createWorkspace();
            var casolution = workspace.OpenSolutionAsync(solution.FilePath).Result;
            foreach (var project in projectsToConvert)
            {
                var newproject = casolution.Projects.First((proj) => proj.Id.Id == project.Id);
                convertAndWrite(new ProjectConverter(this, newproject), args);
            }
        }

        void convertAndWrite(ProjectConverter converter, GeneratorOptions args)
        {
            Debug.Assert(args.TargetRootPath != null);
            foreach (var conversion in converter.GetConversionDelegates().Concat(Conversion.DefaultConversionDelegates))
            {
                if (conversion.Skip)
                    continue;

                var targetBasePath = conversion.TargetBasePath ?? string.Empty;
                targetBasePath = targetBasePath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                var basepath = Path.Combine(args.TargetRootPath, targetBasePath);
                Directory.CreateDirectory(basepath);
                var filepath = Path.Combine(basepath, conversion.TargetFileName);
                if (args.EagerStringConversion)
                {
                    File.WriteAllText(filepath, conversion.ToFullString(), new UTF8Encoding(Conversion.UseUTF8Bom));
                }
                else
                {
                    using (var filestream = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        conversion.Write(filestream, new UTF8Encoding(Conversion.UseUTF8Bom));
                    }
                }
            }
        }

        MSBuildWorkspace createWorkspace()
        {
            if (Options.PreprocessorDefinitionsAdded?.Count != 0)
            {
                var builder = new StringBuilder();
                bool first = true;
                foreach (var definition in Options.PreprocessorDefinitionsAdded!)
                {
                    if (first)
                        first = false;
                    else
                        builder.Append(";");

                    builder.Append(definition);
                }

                return MSBuildWorkspace.Create(new Dictionary<string, string>() { { DefineConstantsName, builder.ToString() } });
            }
            else
            {
                return MSBuildWorkspace.Create();
            }
        }

        public LanguageConversion Conversion
        {
            get { return GetConversion(); }
        }

        protected abstract LanguageConversion GetConversion();
    }

    public sealed class Converter<TConversion> : Converter
        where TConversion : LanguageConversion
    {
        public new TConversion Conversion { get; private set; }

        internal Converter(TConversion conversion)
        {
            Conversion = conversion;
        }

        protected override LanguageConversion GetConversion()
        {
            return Conversion;
        }
    }
}
