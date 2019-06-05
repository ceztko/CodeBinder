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

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            GeneratorOptions genargs = new GeneratorOptions();

            //Solution solution = workspace.OpenSolutionAsync(@"D:\Staging\Euronovate\ENLibPdf\ENLibPdfNet.sln").Result;
            //var conv = SolutionConverter.CreateFor<CSToJavaConversion>(solution);
            //Project project = workspace.OpenProjectAsync(@"D:\Staging\TestJava\ENLibPdfNet.csproj").Result;
            //Project project = workspace.OpenProjectAsync(@"D:\Staging\TestJava\ENLibPdfNetLite.csproj").Result;
            Project project = workspace.OpenProjectAsync(@"D:\Staging\Euronovate\ENLibPdf\ENLibPdfNet\ENLibPdfNet.csproj").Result;


            if (false)
            {
                // Java conversion
                var conv = Converter.CreateFor<CSToJavaConversion>(project);
                conv.Options.PreprocessorDefinitionsRemoved = new string[] { "CSHARP", "NET_FRAMEWORK" };
                conv.Conversion.SkipBody = false;
                conv.Conversion.NamespaceMapping.PushMapping("Euronovate.LibPdf", "com.euronovate.libpdf");
                conv.Conversion.NamespaceMapping.PushMapping("Euronovate.LibPdf.Java", "com.euronovate.libpdf");
                //genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdf\ENLibPdfJar\src\alt\java";
                genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdfJar\src\main\java";
                genargs.EagerStringConversion = true;
                conv.ConvertAndWrite(genargs);

                // Android
                //conv.Options.PreprocessorDefinitionsAdded = new string[] { "ANDROID" };
                //genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdfJar\src\android\java";
                //conv.ConvertAndWrite(genargs);
            }
            else
            {
                // JNI conversion
                var conv = Converter.CreateFor<CSToJNIConversion>(project);
                conv.Options.PreprocessorDefinitionsRemoved = new string[] { "CSHARP", "NET_FRAMEWORK" };
                conv.Conversion.NamespaceMapping.PushMapping("Euronovate.LibPdf", "com.euronovate.libpdf");
                genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdf\ENLibPdfJni";
                genargs.EagerStringConversion = true;
                conv.ConvertAndWrite(genargs);
            }
        }
    }
}
