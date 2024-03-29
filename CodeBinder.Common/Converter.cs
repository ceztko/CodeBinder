﻿// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis.MSBuild;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeBinder;

public class ConverterOptions
{
    public bool IgnoreCompilationErrors { get; set; }
}

public class GeneratorOptions
{
    public string? TargetPath { get; set; }
    /// <summary>Create string before writing to file. For DEBUG</summary>
    public bool EagerStringConversion { get; set; }

    /// <summary>
    /// Properties that will be used to create the msbuild workspace
    /// </summary>
    public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
}

public abstract class Converter
{
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
        if (args.TargetPath == null)
            throw new ArgumentNullException("args.TargtetRootPath");

        Microsoft.CodeAnalysis.Project caproject;
        var workspace = createWorkspace(args.Properties);
        if (project.Solution == null)
        {
            caproject = workspace.OpenProjectAsync(project.FilePath).Result;
        }
        else
        {
            var casolution = workspace.OpenSolutionAsync(project.Solution.FilePath).Result;
            caproject = casolution.Projects.First((proj) => proj.FilePath == project.FilePath);
        }
        
        convertAndWrite(new ProjectConverter(this, caproject), args);
    }

    public void ConvertAndWrite(Solution solution, GeneratorOptions args, IProgress<string>? progress = null)
    {
        if (args.TargetPath == null)
            throw new ArgumentNullException("args.TargtetRootPath");

        convertAndWrite(solution, solution.Projects, args);
    }

    public void ConvertAndWrite(IEnumerable<Project> projectsToConvert, GeneratorOptions args, IProgress<string>? progress = null)
    {
        if (args.TargetPath == null)
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
        var workspace = createWorkspace(args.Properties);
        var casolution = workspace.OpenSolutionAsync(solution.FilePath).Result;
        foreach (var project in projectsToConvert)
        {
            var newproject = casolution.Projects.First((proj) => proj.FilePath == project.FilePath);
            convertAndWrite(new ProjectConverter(this, newproject), args);
        }
    }

    void convertAndWrite(ProjectConverter converter, GeneratorOptions args)
    {
        Debug.Assert(args.TargetPath != null);
        foreach (var conversion in converter.GetConversionDelegates().Concat(Conversion.DefaultConversionDelegates))
        {
            if (conversion.Skip)
                continue;

            var targetBasePath = conversion.TargetBasePath ?? string.Empty;
            targetBasePath = targetBasePath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            var basepath = Path.Combine(args.TargetPath, targetBasePath);
            Directory.CreateDirectory(basepath);
            var filepath = Path.Combine(basepath, conversion.TargetFileName);
            bool useUTF8Bom;
            if (conversion.UseUTF8Bom is bool value)
                useUTF8Bom = value;
            else
                useUTF8Bom = Conversion.UseUTF8Bom;

            if (args.EagerStringConversion)
            {
                File.WriteAllText(filepath, conversion.ToFullString(), new UTF8Encoding(useUTF8Bom));
            }
            else
            {
                using (var filestream = new FileStream(filepath, FileMode.Create))
                {
                    conversion.Write(filestream, new UTF8Encoding(useUTF8Bom));
                }
            }
        }
    }

    MSBuildWorkspace createWorkspace(IReadOnlyDictionary<string, string> extProperties)
    {
        // Creat a workspace with all the preprocessor definition
        // also available as a valorized (the value is arbitrarily 1) property
        var properties = new Dictionary<string, string>{ { "CODE_BINDER", "1" } };
        foreach (var property in extProperties)
            properties.Add(property.Key, property.Value);

        if (Conversion.PreprocessorDefinitions.Count != 0)
        {
            foreach (var definition in Conversion.PreprocessorDefinitions)
                properties[definition] = "1";
        }

        return MSBuildWorkspace.Create(properties);
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
