// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeBinder
{
    public class ConverterOptions
    {
        /// <summary>Plaform specific preprocessor symbols that will be added during compilation</summary>
        public IReadOnlyList<string>? PreprocessorDefinitionsAdded { get; set; }

        /// <summary>Preprocessor symbols that will be be removed during compilation</summary>
        public IReadOnlyList<string>? PreprocessorDefinitionsRemoved { get; set; }

        public bool IgnoreCompilationErrors { get; set; }
    }

    public class GeneratorOptions
    {
        public string? TargetRootPath;
        /// <summary>Create string before writing to file. For DEBUG</summary>
        public bool EagerStringConversion;
    }

    public abstract class Converter
    {
        public ConverterOptions Options { get; private set; }

        internal Converter()
        {
            Options = new ConverterOptions();
        }

        public static Converter<TLanguageConversion> CreateFor<TLanguageConversion>()
            where TLanguageConversion : LanguageConversion, new()
        {
            return new Converter<TLanguageConversion>(new TLanguageConversion());
        }

        public void ConvertAndWrite(Project project, GeneratorOptions args, IProgress<string>? progress = null)
        {
            ConvertAndWrite(new ProjectConverter(this, project), args);
        }

        public void ConvertAndWrite(Solution solution, GeneratorOptions args, IProgress<string>? progress = null)
        {
            ConvertAndWrite(new SolutionConverter(this, solution, solution.Projects, progress), args);
        }

        public void ConvertAndWrite(IEnumerable<Project> projectsToConvert, GeneratorOptions args, IProgress<string>? progress = null)
        {
            ConvertAndWrite(new SolutionConverter(this, projectsToConvert.First().Solution, projectsToConvert, progress), args);
        }

        void ConvertAndWrite(ConverterActual converter, GeneratorOptions args)
        {
            if (args.TargetRootPath == null)
                throw new ArgumentNullException("args.TargtetRootPath");

            foreach (var conversion in converter.GetConversionDelegates().Concat(Conversion.DefaultConversionDelegates))
            {
                if (conversion.Skip)
                    continue;

                var targetBasePath = conversion.TargetBasePath ?? string.Empty;
                targetBasePath = targetBasePath.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                var basepath = Path.Combine(args.TargetRootPath, targetBasePath);
                Directory.CreateDirectory(basepath);
                var filepath = Path.Combine(basepath, conversion.TargetFileName);
                if (args.EagerStringConversion)
                {
                    File.WriteAllText(filepath, conversion.ToFullString(), new UTF8Encoding(Conversion.UseUTF8Bom));
                }
                else
                {
                    using (var filestream = new FileStream(filepath, FileMode.OpenOrCreate))
                    {
                        conversion.Write(filestream, new UTF8Encoding(Conversion.UseUTF8Bom));
                    }
                }
            }
        }
        public LanguageConversion Conversion
        {
            get { return GetConversion(); }
        }

        protected abstract LanguageConversion GetConversion();
    }

    public sealed class Converter<TConversion> : Converter
        where TConversion : LanguageConversion
    {
        public new TConversion Conversion { get; private set; }

        internal Converter(TConversion conversion)
        {
            Conversion = conversion;
        }

        protected override LanguageConversion GetConversion()
        {
            return Conversion;
        }
    }

    internal abstract class ConverterActual
    {
        public Converter Converter { get; private set; }

        public ConverterActual(Converter converter)
        {
            Converter = converter;
        }

        /// <summary>
        /// Get delegates to be iterated to write conversions
        /// </summary>
        internal protected abstract IEnumerable<ConversionDelegate> GetConversionDelegates();
    }
}
