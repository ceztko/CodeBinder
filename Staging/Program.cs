using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeTranslator.Java;
using CodeTranslator;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using CodeTranslator.JNI;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            GeneratorArgs genargs = new GeneratorArgs();

            //Solution solution = workspace.OpenSolutionAsync(@"D:\Staging\Euronovate\ENLibPdf\ENLibPdfNet.sln").Result;
            //var conv = SolutionConverter.CreateFor<CSToJavaConversion>(solution);
            Project project = workspace.OpenProjectAsync(@"D:\Staging\Euronovate\ENLibPdfNetTest.csproj").Result;

            var conv = ProjectConverter.CreateFor<CSToJavaConversion>(project);
            conv.Conversion.BaseNamespace = "com.euronovate.libpdf";
            genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdf\ENLibPdfJar\src\alt\java";
            conv.ConvertAndWrite(genargs);

            //var conv2 = ProjectConverter.CreateFor<CSToJNIConversion>(project);
            //conv2.Conversion.BaseNamespace = "com.euronovate.libpdf.reserved";
            //genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdf\ENLibPdfJni";
            //conv2.ConvertAndWrite(genargs);
        }
    }
}
