// Copyright(c) 2020 Francesco Pretto
// This file is subject to the MIT license
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBinder;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Mono.Options;
using System.Reflection;
using System.IO;
using CodeBinder.CLang;
using CodeBinder.Shared;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeBinder
{
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
                Console.Error.WriteLine(ex.Message);
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
            string? targetRootPath = null;
            string? language = null;
            List<string> definitionsToAdd = new();
            List<string> definitionsToRemove = new();
            List<string> namespaceMappings = new();
            bool shouldShowHelp = false;
            var conversions = GetConverterInfos();

            var options = new OptionSet {
                { "p|project=", "The project to be converted", p => projects.Add(p) },
                { "s|solution=", "The solution to be converted", s => solutionPath = s },
                { "d|def=", "Preprocessor definition to be added during conversion", d => definitionsToAdd.Add(d) },
                { "n|nodef=", "Preprocessor definition to be removed during conversion", d => definitionsToRemove.Add(d) },
                { "m|nsmapping=", "Mapping for the given, must be colon separated ns1:ns2", ns => namespaceMappings.Add(ns) },
                { "l|language=", "The target language for the conversion", l => language = l },
                { "r|rootpath=", "The target root path for the conversion", r => targetRootPath = r },
                { "h|help", "Show this message and exit", h => shouldShowHelp = h != null },
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
            ConversionInfo conversionInfo;
            Converter converter;
            try
            {
                if (projects.Count == 0 && solutionPath == null)
                    throw new Exception("A project or a solution must be specified");

                if (targetRootPath == null)
                    throw new Exception("A target root path must be specified");

                try
                {
                    conversionInfo = conversions.First((info) => language == info.LanguageName);
                }
                catch
                {
                    throw new Exception("Target language is missing or unsupported");
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
                    throw new Exception("Could not parse extra args: " + extra);
            }
            catch
            {
                ShowHelp(options);
                throw;
            }

            // Set the namespace mappings in the conversion
            foreach (var nsmapping in namespaceMappings)
            {
                var splitted = nsmapping.Split(':');
                if (splitted.Length != 2)
                    throw new Exception("Mapping must be in the form ns:mappens");

                converter.Conversion.NamespaceMapping.PushMapping(splitted[0], splitted[1]);
            }

            converter.Options.PreprocessorDefinitionsAdded = definitionsToAdd;
            converter.Options.PreprocessorDefinitionsRemoved = definitionsToRemove;

            GeneratorOptions genargs = new GeneratorOptions();
            genargs.TargetRootPath = targetRootPath;

            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            // TODO: Handle multiple projects
            if (solutionPath != null)
            {
                var solution = workspace.OpenSolutionAsync(solutionPath).Result;
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
                var project = workspace.OpenProjectAsync(projects.First()).Result;
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
}
