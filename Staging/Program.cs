using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeTranslator.Java;
using CodeTranslator.Shared;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            GeneratorArgs genargs = new GeneratorArgs();
            genargs.SourceRootPath = @"D:\Staging\Euronovate\ENLibPdf\ENLibPdfJar\src\alt\java";

            //Solution solution = workspace.OpenSolutionAsync(@"D:\Staging\Euronovate\ENLibPdf\ENLibPdfNet.sln").Result;
            //var conv = SolutionConverter.CreateFor<CSToJavaConversion>(solution);
            Project project = workspace.OpenProjectAsync(@"D:\Staging\Euronovate\ENLibPdf\ENLibPdfNet\ENLibPdfNetTest.csproj").Result;
            var conv = ProjectConverter.CreateFor<CSToJavaConversion>(project);
            conv.Conversion.BaseNamespace = "com.euronovate.libpdf";
            conv.ConvertAndWrite(genargs);
        }
    }
}
