using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Util
{
    public class StringConversionBuilder : ConversionBuilder
    {
        string m_filename;
        Func<string> m_resourceString;

        public StringConversionBuilder(string filename, Func<string> resourceString)
        {
            m_filename = filename;
            m_resourceString = resourceString;
        }

        public override string FileName
        {
            get { return m_filename; }
        }

        public override void Write(CodeBuilder builder)
        {
            builder.Append(m_resourceString());
        }
    }
}
