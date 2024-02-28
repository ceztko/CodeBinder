// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Mono.Options;
using System.Reflection;
using System.IO;
using CodeBinder.Shared;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeBinder;

class Program
{
    static int Main(string[] args)
    {
        // Registering MSBuild defaults is necessary otherwise projects will not compile
        MSBuildLocator.RegisterDefaults();

        try
        {
            main(args);
        }
        catch (OptionException e)
        {
            Console.Write("CodeBinder: ");
            Console.WriteLine(e.Message);
            Console.WriteLine("Try `CodeBinder --help' for more information.");
            return -1;
        }
        catch (Exception ex)
        {
            Console.Write("ERROR: ");
            Console.Error.WriteLine(ex);
            return -1;
        }

        return 0;
    }

    // Move work to separate not inline method as suggested:
    // https://github.com/microsoft/MSBuildLocator/issues/104#issuecomment-790226981
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void main(string[] cmdArgs)
    {
        HashSet<string> projects = new();
        string? solutionPath = null;
        string? targetPath = null;
        string? language = null;
        List<string> namespaceMappings = new();
        bool shouldShowHelp = false;
        bool shouldListLanguages = false;
        var conversions = GetConverterInfos();
        List<string> properties = new();

        var options = new OptionSet {
            { "p|project=", "The project to be converted", p => projects.Add(p) },
            { "s|solution=", "The solution to be converted", s => solutionPath = s },
            { "e|property=", "Property for the msbuild workspace, must be colon separated key:value", pr => properties.Add(pr) },
            { "m|nsmapping=", "Mapping for the given, must be colon separated ns1:ns2", ns => namespaceMappings.Add(ns) },
            { "l|language=", "The target language for the conversion", l => language = l },
            { "t|targetpath=", "The target root path for the conversion", t => targetPath = t },
            { "h|help", "Show this message and exit", h => shouldShowHelp = h != null },
            { "L|list", "List all supported languages and exit", L => shouldListLanguages = L != null },
        };

        var supportedExtraArgs = new List<string>();
        foreach (var conversion in conversions)
        {
            foreach (var swtch in conversion.ConfigurationSwitches)
                options.Add(swtch.Name, swtch.Description, arg => supportedExtraArgs.Add(arg));
        }

        List<string> extra = options.Parse(cmdArgs);
        if (shouldShowHelp)
        {
            ShowHelp(options);
            return;
        }

        if (shouldListLanguages)
        {
            Console.WriteLine("CodeBinder supported languages:");
            Console.WriteLine();
            foreach (var conversion in conversions)
                Console.WriteLine(conversion.LanguageName);

            return;
        }

        ConversionInfo conversionInfo;
        Converter converter;
        try
        {
            if (projects.Count == 0 && solutionPath == null)
                throw new Exception("A project or a solution must be specified");

            if (targetPath == null)
                throw new Exception("A target root path must be specified");

            try
            {
                conversionInfo = conversions.First((info) => language == info.LanguageName);
            }
            catch
            {
                throw new Exception($"Target language {language} is missing or unsupported");
            }

            // Find all Converter.CreateFor method
            var createForMethod = typeof(Converter).GetMethod("CreateFor")!;

            // Istantiate the generic method with the desired conversion type
            converter = (Converter)createForMethod.MakeGenericMethod(conversionInfo.Type).Invoke(null, null)!;

            if (converter.Conversion.NeedNamespaceMapping && namespaceMappings.Count == 0)
                throw new Exception("Namespace mappings must be specified");

            if (extra.Count != 0)
                throw new Exception("Could not parse extra args: " + extra);

            if (supportedExtraArgs.Count != 0 && !converter.Conversion.TryParseExtraArgs(supportedExtraArgs))
                throw new Exception("Could not parse extra args: " + string.Join(", ", extra));
        }
        catch
        {
            ShowHelp(options);
            throw;
        }

        GeneratorOptions genargs = new GeneratorOptions();
        // Set the properties for the msbuild workspace creation
        foreach (var prop in properties)
        {
            var splitted = prop.Split(':');
            if (splitted.Length != 2)
                throw new Exception("Property must be in the form key:value");

            genargs.Properties.Add(splitted[0], splitted[1]);
        }

        genargs.TargetPath = targetPath;

        // Set the namespace mappings in the conversion
        foreach (var nsmapping in namespaceMappings)
        {
            var splitted = nsmapping.Split(':');
            if (splitted.Length != 2)
                throw new Exception("Mapping must be in the form ns:mapping");

            converter.Conversion.NamespaceMapping.PushMapping(splitted[0], splitted[1]);
        }

        // TODO: Handle multiple projects
        if (solutionPath != null)
        {
            var solution = Solution.Open(solutionPath);
            if (projects.Count == 0)
            {
                converter.ConvertAndWrite(solution, genargs);
            }
            else
            {
                var filtered = solution.Projects.Where((project) => projects.Contains(project.Name)).ToList();
                converter.ConvertAndWrite(filtered, genargs);
            }
        }
        else if (projects.Count != 0)
        {
            var project = Project.Open(projects.First());
            converter.ConvertAndWrite(project, genargs);
        }
        else
            throw new NotSupportedException();
    }

    static IReadOnlyList<ConversionInfo> GetConverterInfos()
    {
        var exclusionList = new string[] { "CodeBinder.Common.dll", "CodeBinder.Redist.dll" };

        var types = new List<ConversionInfo>();
        string exepath = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
        foreach (var dllpath in Directory.GetFiles(exepath, "CodeBinder.*.dll"))
        {
            string filename = Path.GetFileName(dllpath);
            if (exclusionList.Contains(filename))
                continue;

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(dllpath);
            }
            catch
            {
                continue;
            }

            foreach (var type in assembly.GetTypes())
            {
                CustomAttributeData? attr = null;
                if (!type.IsSubclassOf(typeof(LanguageConversion)) ||
                    (attr = type.CustomAttributes.First((data) => data.AttributeType == typeof(ConversionLanguageName))) == null)
                {
                    continue;
                }
                var switches = new List<ConfigurationSwitch>();
                foreach (var data in from data in type.CustomAttributes where data.AttributeType == typeof(ConfigurationSwitch) select data)
                    switches.Add(new ConfigurationSwitch((string)data.ConstructorArguments[0].Value!, (string)data.ConstructorArguments[1].Value!));

                types.Add(new ConversionInfo() { Type = type, LanguageName = (string)attr.ConstructorArguments[0].Value!, ConfigurationSwitches = switches });
            }
        }

        return types;
    }

    [DebuggerDisplay("LanguageName = {LanguageName}")]
    struct ConversionInfo
    {
        public Type Type;
        public string LanguageName;
        public List<ConfigurationSwitch> ConfigurationSwitches;
    }

    static void ShowHelp(OptionSet options)
    {
        Console.WriteLine("Usage: CodeBinder [OPTIONS]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        options.WriteOptionDescriptions(Console.Out);
    }
}
