// Copyright(c) 2018 Francesco Pretto
// This file is subject to the MIT license
using CodeTranslator.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeTranslator
{
    public class GeneratorArgs
    {
        public string SourceRootPath;
    }

    public abstract class Converter
    {
        public LanguageConversion Conversion
        {
            get { return GetConversion(); }
        }

        protected abstract LanguageConversion GetConversion();

        public abstract IEnumerable<ConversionDelegate> Convert();

        public void ConvertAndWrite(GeneratorArgs args)
        {
            foreach (var conversion in Convert().Concat(Conversion.DefaultResults))
            {
                var basepath = Path.Combine(args.SourceRootPath, conversion.TargetBasePath);
                Directory.CreateDirectory(basepath);
                var filepath = Path.Combine(basepath, conversion.TargetFileName);
                File.WriteAllText(filepath, conversion.ToFullString());
                /* ENABLE Later
                using (var filestream = new FileStream(filepath, FileMode.OpenOrCreate))
                {
                    conversion.Write(filestream);
                }
                */
            }
        }
    }
}
