using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Util
{
    public abstract class ConversionBuilderBase : IConversionBuilder
    {
        protected virtual string? GetBasePath()
        {
            return null;
        }

        public string? BasePath
        {
            get { return GetBasePath(); }
        }

        public void Write(CodeBuilder builder)
        {
            string? preamble = GetGeneratedPreamble();
            if (!string.IsNullOrEmpty(preamble))
                builder.AppendLine(preamble);

            write(builder);
        }

        public abstract void write(CodeBuilder builder);

        protected virtual string? GetGeneratedPreamble() => null;

        public abstract string FileName { get; }

        string? IConversionBuilder.BasePath
        {
            get { return GetBasePath(); }
        }
    }

    public abstract class ConversionBuilder : ConversionBuilderBase
    {
        public new string? BasePath { get; set; }

        protected override string? GetBasePath() => BasePath;
    }

    public interface IConversionBuilder
    {
        void Write(CodeBuilder builder);

        string FileName { get; }

        string? BasePath { get; }
    }
}
