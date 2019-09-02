using CodeBinder.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeBinder.Util
{
    public abstract class ConversionBuilderBase : IConversionBuilder
    {
        public virtual string GeneratedPreamble
        {
            get { return null; }
        }

        protected virtual string GetBasePath()
        {
            return null;
        }

        public string BasePath
        {
            get { return GetBasePath(); }
        }

        public abstract string FileName { get; }

        public abstract void Write(CodeBuilder builder);

        string IConversionBuilder.BasePath
        {
            get { return GetBasePath(); }
        }
    }

    public abstract class ConversionBuilder : ConversionBuilderBase
    {
        protected override string GetBasePath()
        {
            return BasePath;
        }

        public new string BasePath { get; set; }
    }

    public interface IConversionBuilder
    {
        void Write(CodeBuilder builder);

        string GeneratedPreamble { get; }

        string FileName { get; }

        string BasePath { get; }
    }
}
