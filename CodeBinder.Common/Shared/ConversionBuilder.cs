using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Shared
{
    /// <summary>
    /// Class to write a conversion with a context
    /// </summary>
    /// <seealso cref="ConversionDelegate"/>
    /// <remarks>Not coupled to CodeBuilder</remarks>
    public abstract class ConversionBuilder<TContext> : ConversionBuilder
    {
        public TContext Context { get; private set; }

        public ConversionBuilder(TContext context)
        {
            Context = context;
        }
    }

    /// <summary>
    /// Class to write a conversion 
    /// </summary>
    /// <seealso cref="ConversionDelegate"/>
    /// <remarks>Not coupled to CodeBuilder</remarks>
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
    /// Interface to write a conversion
    /// </summary>
    /// <seealso cref="ConversionDelegate"/>
    /// <remarks>Not coupled to CodeBuilder</remarks>
    public interface IConversionBuilder
    {
        void Write(CodeBuilder builder);

        string FileName { get; }

        string? BasePath { get; }
    }
}
