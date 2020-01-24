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
using CodeBinder.CLang;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            GeneratorOptions genargs = new GeneratorOptions();

            //Solution solution = workspace.OpenSolutionAsync(@"D:\Staging\Euronovate\ENLibPdf\ENLibPdfNet.sln").Result;
            //var conv = SolutionConverter.CreateFor<CSToJavaConversion>(solution);
            //Project project = await workspace.OpenProjectAsync(@"D:\Staging\Euronovate\CodeBinderInternal\Test\SimpleLibrary\SimpleLibrary.csproj");
            //Project project = await workspace.OpenProjectAsync(@"D:\Staging\Euronovate\CodeBinderInternal\Test\ENLibPdfNetLite.csproj");
            Project project = await workspace.OpenProjectAsync(@"D:\Staging\Euronovate\ENLibPdf\ENLibPdfNet\ENLibPdfNet.csproj");
            genargs.TargetRootPath = @"D:\Staging\Euronovate\CodeBinderInternal\Test\";

            if (false)
            {
                // Java conversion
                var conv = Converter.CreateFor<ConversionCSharpToJava>(project);
                conv.Options.PreprocessorDefinitionsRemoved = new string[] { "CSHARP", "DOTNET", "NET_FRAMEWORK" };
                conv.Conversion.SkipBody = false;
                conv.Conversion.NamespaceMapping.PushMapping("Euronovate.LibPdf", "com.euronovate.libpdf");
                conv.Conversion.NamespaceMapping.PushMapping("Euronovate.LibPdf.Java", "com.euronovate.libpdf");
                //genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdf\ENLibPdfJar\src\alt\java";
                //genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdfJar\src\main\java";
                genargs.EagerStringConversion = true;
                conv.ConvertAndWrite(genargs);

                // Android
                conv.Options.PreprocessorDefinitionsAdded = new string[] { "ANDROID" };
                genargs.TargetRootPath = @"D:\Staging\Euronovate\ENLibPdfJar\src\android\java";
                conv.ConvertAndWrite(genargs);
            }
            else
            {
                {
                    // CLang covnersion
                    var conv = Converter.CreateFor<ConversionCSharpToCLang>(project);
                    conv.Options.PreprocessorDefinitionsRemoved = new string[] { "CSHARP", "DOTNET", "NET_FRAMEWORK" };
                    genargs.TargetRootPath = @"D:\Staging\Euronovate\ENLibPdf\ENLibPdf";
                    genargs.EagerStringConversion = true;
                    conv.ConvertAndWrite(genargs);
                }
                {
                    // JNI conversion
                    var conv = Converter.CreateFor<ConversionCSharpToJNI>(project);
                    conv.Options.PreprocessorDefinitionsRemoved = new string[] { "CSHARP", "DOTNET", "NET_FRAMEWORK" };
                    conv.Conversion.NamespaceMapping.PushMapping("Euronovate.LibPdf", "com.euronovate.libpdf");
                    genargs.TargetRootPath = @"D:\Staging\Euronovate\ENLibPdf\ENLibPdfJni";
                    genargs.EagerStringConversion = true;
                    conv.ConvertAndWrite(genargs);
                }
            }
        }
    }
}
