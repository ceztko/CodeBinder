using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    /// <summary>
    /// Partially contextualized class to write a conversion on a builder
    /// </summary>
    /// <seealso cref="ConversionDelegate"/>
    public abstract class ConversionBuilder : IConversionBuilder
    {
        public void Write(CodeBuilder builder)
        {
            string? preamble = GetGeneratedPreamble();
            if (!string.IsNullOrEmpty(preamble))
                builder.AppendLine(preamble);

            write(builder);
        }

        protected abstract void write(CodeBuilder builder);

        protected abstract string GetFileName();

        protected virtual string? GetBasePath() => null;

        protected virtual string? GetGeneratedPreamble() => null;

        public string FileName => GetFileName();

        public string? BasePath => GetBasePath();

        public string? GeneratedPreamble => GetGeneratedPreamble();
    }

    /// <summary>
    /// Partially contextualized interface to write a conversion on a builder
    /// </summary>
    /// <seealso cref="ConversionDelegate"/>
    public interface IConversionBuilder
    {
        void Write(CodeBuilder builder);

        string FileName { get; }

        string? BasePath { get; }
    }
}
