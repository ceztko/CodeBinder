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

namespace CodeBinder
{
    class Program
    {
        static int Main(string[] args)
        {
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

        static void main(string[] cmdArgs)
        {
            string? projectPath = null;
            string? solutionPath = null;
            string? targetRootPath = null;
            string? language = null;
            var definitionsToAdd = new List<string>();
            var definitionsToRemove = new List<string>();
            var namespaceMappings = new List<string>();
            bool shouldShowHelp = false;
            var options = new OptionSet {
                { "p|project=", "The project to be converted", p => projectPath = p },
                { "s|solution=", "The solution to be converted", s => solutionPath = s },
                { "d|def=", "Preprocessor definition to be added during conversion", d => definitionsToAdd.Add(d) },
                { "n|nodef=", "Preprocessor definition to be removed during conversion", d => definitionsToRemove.Add(d) },
                { "ns|nsmapping=", "Mapping for the namespace, must be a colon separated", ns => namespaceMappings.Add(ns) },
                { "l|language=", "The target language for the conversion", l => language = l },
                { "r|rootpath=", "The target root path for the conversion", r => targetRootPath = r },
                { "h|help", "Show this message and exit", h => shouldShowHelp = h != null },
            };

            List<string> extra = options.Parse(cmdArgs);
            if (shouldShowHelp)
            {
                ShowHelp(options);
                return;
            }

            if (projectPath == null && solutionPath == null)
                throw new Exception("A project or a solution must be specified");

            if (namespaceMappings.Count == 0)
                throw new Exception("Namespace mappings must be specified");

            if (targetRootPath == null)
                throw new Exception("A target root path must be specified");

            var conversions = GetConverterInfos();
            ConversionInfo conversionInfo;
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
            Converter converter = (Converter)createForMethod.MakeGenericMethod(conversionInfo.Type).Invoke(null, null)!;

            if (extra.Count != 0 && !converter.Conversion.TryParseExtraArgs(extra))
            {
                ShowHelp(options);
                throw new Exception("Could not parse extra args: " + extra);
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

            // Registering MSBuild defaults is necessary otherwise projects will not compile
            MSBuildLocator.RegisterDefaults();
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            if (projectPath != null)
            {
                var project = workspace.OpenProjectAsync(projectPath).Result;
                converter.ConvertAndWrite(project, genargs);
            }
            else if (solutionPath != null)
            {
                var solution = workspace.OpenSolutionAsync(solutionPath).Result;
                converter.ConvertAndWrite(solution, genargs);
            }
            else
                throw new Exception();
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
                        (attr = type.CustomAttributes.First((data) => { return data.AttributeType == typeof(ConversionLanguageName); })) == null)
                    {
                        continue;
                    }

                    types.Add(new ConversionInfo() { Type = type, LanguageName = (string)attr.ConstructorArguments[0].Value! });
                }
            }

            return types;
        }

        [DebuggerDisplay("LanguageName = {LanguageName}")]
        struct ConversionInfo
        {
            public Type Type;
            public string LanguageName;
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
