using CodeTranslator.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeTranslator.Shared
{
    public interface IConversionBuilder
    {
        void Write(CodeBuilder builder);

        string FileName { get; }

        string BasePath { get; }
    }
}
