using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Util
{
    public class StringConversionBuilder : ConversionBuilder
    {
        string m_filename;
        Func<string> m_resourceString;

        public string? GeneratedPreamble { get; set; }

        public StringConversionBuilder(string filename, Func<string> resourceString)
        {
            m_filename = filename;
            m_resourceString = resourceString;
        }

        public override string FileName
        {
            get { return m_filename; }
        }

        protected sealed override string? GetGeneratedPreamble() => GeneratedPreamble;

        public override void write(CodeBuilder builder)
        {
            builder.Append(m_resourceString());
        }
    }
}
