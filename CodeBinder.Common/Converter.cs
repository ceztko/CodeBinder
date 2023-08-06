// SPDX-FileCopyrightText: (C) 2018 Francesco Pretto <ceztko@gmail.com>
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
    public string? TargetRootPath;
    /// <summary>Create string before writing to file. For DEBUG</summary>
    public bool EagerStringConversion;
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
            var newproject = casolution.Projects.First((proj) => proj.FilePath == project.FilePath);
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
        // Creat a workspace with all the preprocessor definition
        // also available as a valorized (the value is arbitrarily 1) property
        var properties = new Dictionary<string, string>{ { "CODE_BINDER", "1" } };
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
