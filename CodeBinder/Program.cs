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
                System.Console.Error.WriteLine(ex.Message);
                return -1;
            }

            return 0;
        }

        static void main(string[] args)
        {
            string projectPath = null;
            string solutionPath = null;
            string namespaceStr = null;
            string targetRootPath = null;
            string language = null;
            var definitionsToAdd = new List<string>();
            var definitionsToRemove = new List<string>();
            bool shouldShowHelp = false;
            var options = new OptionSet {
                { "p|project=", "The project to be converted", p => projectPath = p },
                { "s|solution=", "The solution to be converted", s => solutionPath = s },
                { "d|def=", "Preprocessor definition to be added during conversion", d => definitionsToAdd.Add(d) },
                { "n|nodef=", "Preprocessor definition to be removed during conversion", d => definitionsToRemove.Add(d) },
                { "ns|namespace=", "The base namespace of the converted project", n => namespaceStr = n },
                { "l|language=", "The target language for the conversion", l => language = l },
                { "r|rootpath=", "The target root path for the conversion", r => targetRootPath = r },
                { "h|help", "Show this message and exit", h => shouldShowHelp = h != null },
            };

            List<string> extra = options.Parse(args);
            if (shouldShowHelp)
            {
                ShowHelp(options);
                return;
            }

            if (projectPath == null && solutionPath == null)
                throw new Exception("A project or a solution must be specified");

            if (namespaceStr == null)
                throw new Exception("A base namespace must be specified");

            if (targetRootPath == null)
                throw new Exception("A target root path must be specified");

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
                        throw new Exception();
                    break;
                }
                case "JNI":
                {
                    if (project != null)
                        jniConversion = Converter.CreateFor<CSToJNIConversion>(project);
                    else if (solution != null)
                        jniConversion = Converter.CreateFor<CSToJNIConversion>(solution);
                    else
                        throw new Exception();
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

            converter.Options.PreprocessorDefinitionsAdded = definitionsToAdd;
            converter.Options.PreprocessorDefinitionsRemoved = definitionsToRemove;

            GeneratorOptions genargs = new GeneratorOptions();
            genargs.SourceRootPath = targetRootPath;
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
