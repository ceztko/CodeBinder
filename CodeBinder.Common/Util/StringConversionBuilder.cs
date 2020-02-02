using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Util
{
    public class StringConversionBuilder : ConversionBuilder
    {
        Func<string> m_resourceString;

        public new string FileName { get; private set; }

        public new string? GeneratedPreamble { get; set; }

        public new string? BasePath { get; set; }

        public StringConversionBuilder(string filename, Func<string> resourceString)
        {
            FileName = filename;
            m_resourceString = resourceString;
        }

        protected override string GetFileName() => FileName;

        protected override string? GetBasePath() => BasePath;

        protected sealed override string? GetGeneratedPreamble() => GeneratedPreamble;

        protected override void write(CodeBuilder builder)
        {
            builder.Append(m_resourceString());
        }
    }
}
