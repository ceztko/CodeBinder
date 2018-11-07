using CodeTranslator.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Util
{
    public abstract class ConversionBuilder : IConversionBuilder
    {
        public virtual string GeneratedPreamble
        {
            get { return null; }
        }

        public virtual string BasePath
        {
            get { return null; }
        }

        public abstract string FileName { get; }

        public abstract void Write(CodeBuilder builder);
    }

    public interface IConversionBuilder
    {
        void Write(CodeBuilder builder);

        string GeneratedPreamble { get; }

        string FileName { get; }

        string BasePath { get; }
    }
}
