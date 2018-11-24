// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeBinder.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeBinder
{
    public class GeneratorArgs
    {
        public string SourceRootPath;
        /// <summary>Create string before writing to file. For DEBUG</summary>
        public bool EagerStringConversion;
    }

    public abstract class Converter
    {
        internal Converter() { }

        public LanguageConversion Conversion
        {
            get { return GetConversion(); }
        }

        public bool IgnoreCompilationErrors { get; set; }

        protected abstract LanguageConversion GetConversion();

        internal protected abstract IEnumerable<ConversionDelegate> Convert();

        public void ConvertAndWrite(GeneratorArgs args)
        {
            foreach (var conversion in Convert().Concat(Conversion.DefaultConversionDelegates))
            {
                var basepath = Path.Combine(args.SourceRootPath, conversion.TargetBasePath ?? "");
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
    }
}
