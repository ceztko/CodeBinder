using CodeBinder.Apple;
using CodeBinder;
using Microsoft.Build.Locator;
using CodeBinder.CLang;
using CodeBinder.JNI;
using CodeBinder.Java;
using System.Runtime.CompilerServices;
using CodeBinder.JavaScript;
using System;
using System.IO;
using System.Linq;
using CodeBinder.NativeAOT;

namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        _ = args;
        // RegisterDefaults is needed
        MSBuildLocator.RegisterDefaults();
        DoWork();
    }

    // Move work to separate not inline method as pointed here:
    // https://github.com/microsoft/MSBuildLocator/issues/104#issuecomment-790226981
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void DoWork()
    {
        var exedir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        var rootPath = exedir.Parent!.Parent!;
        var testSolution = Path.Combine("..", "..", "Test", "CodeBinder.Test.sln");
        var solution = Solution.Open(testSolution);
        var project = solution.Projects.First((project) => project.Name == "SampleLibrary");
        var targetPath = Path.Combine(rootPath.Parent!.ToString(), "CodeBinder-TestCodeGen");
        GeneratorOptions genargs = new GeneratorOptions();

        {
            // TypeScript conversion
            var conv = Converter.CreateFor<ConversionCSharpToTypeScript>();
            conv.Conversion.GenerationFlags = TypeScriptGenerationFlags.None;
            conv.Conversion.NamespaceMapping.PushMapping("SampleLibrary", "SampleLibrary");
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryMTS");
            genargs.EagerStringConversion = true;
            conv.ConvertAndWrite(project, genargs);

            conv.Conversion.GenerationFlags = TypeScriptGenerationFlags.CommonJSCompat;
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryTS");
            conv.ConvertAndWrite(project, genargs);
        }

        {
            // NAPI conversion
            var conv = Converter.CreateFor<ConversionCSharpToNAPI>();
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryNAPI");
            genargs.EagerStringConversion = true;
            conv.ConvertAndWrite(project, genargs);
        }

        {
            // ObjC conversion
            var conv = Converter.CreateFor<ConversionCSharpToObjC>();
            conv.Conversion.SkipBody = false;
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryObjC");
            genargs.EagerStringConversion = true;
            conv.ConvertAndWrite(project, genargs);
        }

        {
            // Java conversion
            var conv = Converter.CreateFor<ConversionCSharpToJava>();
            conv.Conversion.SkipBody = false;
            conv.Conversion.NamespaceMapping.PushMapping("SampleLibrary", "SampleLibrary");
            genargs.EagerStringConversion = true;

            // JDK
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryJDK");
            conv.ConvertAndWrite(project, genargs);

            // Android
            conv.Conversion.JavaPlatform = JavaPlatform.Android;
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryAndroid");
            conv.ConvertAndWrite(project, genargs);
        }

        {
            // JNI conversion
            var conv = Converter.CreateFor<ConversionCSharpToJNI>();
            conv.Conversion.NamespaceMapping.PushMapping("Euronovate.LibPdf", "com.euronovate.libpdf");
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryJNI");
            genargs.EagerStringConversion = true;
            conv.ConvertAndWrite(project, genargs);
        }

        {
            // CLang conversion
            var conv = Converter.CreateFor<ConversionCSharpToCLang>();
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryCLang", "sgen");
            genargs.EagerStringConversion = true;
            conv.ConvertAndWrite(project, genargs);
        }

        {
            // CLang conversion
            var conv = Converter.CreateFor<ConversionCSharpToNativeAOT>();
            genargs.TargetRootPath = Path.Combine(targetPath, "SampleLibraryNAOT", "sgen");
            genargs.EagerStringConversion = true;
            conv.ConvertAndWrite(project, genargs);
        }
    }
}
