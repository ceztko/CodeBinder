using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBinder.Java;
using CodeBinder;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using CodeBinder.JNI;
using Mono.Options;

namespace CodeBinder
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectPath = null;
            string solutionPath = null;
            string namespaceStr = null;
            string sourceRootPath = null;
            string language = null;
            var definitions = new List<string>();
            bool shouldShowHelp = false;
            var options = new OptionSet {
                { "p|project=", "The project to be converted", p => projectPath = p },
                { "s|solution=", "The solution to be converted", s => solutionPath = s },
                { "n|namespace=", "The base namespace of the converted project", n => namespaceStr = n },
                { "l|language=", "The target language for the conversion", l => namespaceStr = l },
                { "r|rootpath=", "The root path for the conversion", r => sourceRootPath = r },
                { "d|definition=", "Preprocessor definition to be removed during conversion", d => definitions.Add(d) },
                { "h|help", "Show this message and exit", h => shouldShowHelp = h != null },
            };

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("CodeBinder: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `CodeBinder --help' for more information.");
                return;
            }

            if (shouldShowHelp)
            {
                ShowHelp(options);
                return;
            }

            MSBuildLocator.RegisterDefaults();
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            Project project = null;
            Solution solution = null;
            if (projectPath != null)
                project = workspace.OpenProjectAsync(projectPath).Result;
            else if (solutionPath != null)
                solution = workspace.OpenSolutionAsync(solutionPath).Result;
            else
                throw new Exception();

            Converter<CSToJavaConversion> javaConversion = null;
            Converter<CSToJNIConversion> jniConversion = null;
            switch (language)
            {
                case "Java":
                {
                    if (project != null)
                        javaConversion = Converter.CreateFor<CSToJavaConversion>(project);
                    else if (solution != null)
                        javaConversion = Converter.CreateFor<CSToJavaConversion>(solution);
                    else
                        throw new Exception("A project or a solution must be specified");
                    break;
                }
                case "JNI":
                {
                    if (project != null)
                        jniConversion = Converter.CreateFor<CSToJNIConversion>(project);
                    else if (solution != null)
                        jniConversion = Converter.CreateFor<CSToJNIConversion>(solution);
                    else
                        throw new Exception("A project or a solution must be specified");
                    break;
                }
                default:
                    throw new Exception("Target language is missing or unsupported");
            }

            Converter converter;
            if (javaConversion != null)
            {
                javaConversion.Conversion.BaseNamespace = namespaceStr;
                converter = javaConversion;
            }
            else if (jniConversion != null)
            {
                jniConversion.Conversion.BaseNamespace = namespaceStr;
                converter = jniConversion;
            }
            else
                throw new Exception();

            converter.Options.PlatformPreprocessorDefinitions = definitions;

            GeneratorOptions genargs = new GeneratorOptions();
            genargs.SourceRootPath = sourceRootPath;
            converter.ConvertAndWrite(genargs);
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: CodeBinder [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
